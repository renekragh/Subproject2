using System.Security.Claims;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Movies.Application.Common.Behaviors;
using Movies.Application.Common.Interfaces;
using Movies.Application.Features.Bookmarks.Models;
using Movies.Domain.Entities;

namespace Movies.Application.Features.Bookmarks.Handlers;

public class BookmarkNamesHandler : BaseHandler, IBookmarkNamesHandler
{
    public BookmarkNamesHandler(IUnitOfWork unitOfWork, 
                                LinkGenerator generator, 
                                IHttpContextAccessor httpContextAccessor, 
                                IMapper mapper) : base (unitOfWork, generator, httpContextAccessor, mapper) {}
                                
    public ObjectResult BookmarkName(string id, string key, string note, string endpointName)
    {
        if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
        {

            var idempotencyKey = _unitOfWork
                                    .GetRepository<Idempotency>()
                                    .RetrieveEntity(x => x.Key.ToString() == key);

            if (idempotencyKey != null)
            {
                var existingEntity = _unitOfWork
                                        .UsersRepository
                                        .GetUserWithNameBookmarks(userId)
                                        .UserBookmarkNames
                                        .FirstOrDefault(x => x.Nconst.Trim() == id);
                                      
                var badRequest = new ObjectResult(new 
                                                    { 
                                                        message = "Request has already been processed", 
                                                        item = CreateBookmarkNameModel(endpointName, existingEntity) 
                                                    }
                                                );
                badRequest.StatusCode = 400;
                return badRequest;
            }

            _unitOfWork
                .GetRepository<Idempotency>()
                .CreateEntity(new Idempotency { Key = Guid.Parse(key), CreatedAt = DateTimeOffset.Now.ToUnixTimeSeconds() });

            _unitOfWork
                .UsersRepository
                .GetUserWithNameBookmarks(userId)
                .UserBookmarkNames.Add(new UserBookmarkName { Nconst = id, Userid = userId, Note = note });

            if (_unitOfWork.Save()) {
                var newEntity = _unitOfWork
                                    .GetRepository<UserBookmarkName>()
                                    .RetrieveEntity(x => x.Nconst.Trim() == id);
                var created = new ObjectResult(new { endpointName, item = CreateBookmarkNameModel(endpointName, newEntity) });
                created.StatusCode = 201;
                return created;
            }
        }

        var internalServerError = new ObjectResult(new { message = "Request has not been processed" });
        internalServerError.StatusCode = 500;
        return internalServerError;
    }

    public object GetBookmarkedNames(string endpointName, Paging pagingParams)
    {
         if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
         {
            var nameBookmarks = _unitOfWork
                                    .UsersRepository
                                    .GetUserWithNameBookmarks(userId)
                                    .UserBookmarkNames
                                    .Select(x => CreateBookmarkNameModel(endpointName, x));
                            
            var numOfItems = nameBookmarks.Count();
            pagingParams.EndpointName = endpointName;

            return CreatePaging(searchQuery: null, nameBookmarks, numOfItems, pagingParams);
         }
         return null;
    }

    public object GetBookmarkedName(string endpointName, string nameId)
    {
         if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
         {
            var nameBookmark = _unitOfWork
                                 .UsersRepository
                                 .GetUserWithNameBookmarks(userId)
                                 .UserBookmarkNames
                                 .FirstOrDefault(x => x.Nconst.Trim() == nameId);

            if (nameBookmark == null) return null;
            var nameBookmarkModel = CreateBookmarkNameModel(endpointName, nameBookmark);
            return CreateLinks(endpointName, new { id = nameId }, nameBookmarkModel);
         }
         return null;
    }

    public ObjectResult UpdateBookmarkedName(string nameId, string note, string endpointName)
    {
        if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
        {
            var user = _unitOfWork
                        .UsersRepository
                        .GetUserWithNameBookmarks(userId);
            user
                .UserBookmarkNames
                .FirstOrDefault(x => x.Nconst.Trim() == nameId)
                .Note = note;               

            _unitOfWork
                .GetRepository<ImdbUser>()
                .UpdateEntity(user);
              
            if (_unitOfWork.Save()) 
            {                              
                var updatedBookmarkedName = _unitOfWork
                                                .UsersRepository
                                                .GetUserWithNameBookmarks(userId)
                                                .UserBookmarkNames
                                                .FirstOrDefault(x => x.Nconst.Trim() == nameId);

                var updated = new ObjectResult(new { endpointName, item = CreateBookmarkNameModel(endpointName, updatedBookmarkedName) });
                updated.StatusCode = 200;
                return updated;   
            }
        }
        var internalServerError = new ObjectResult(new { message = "Request has not been processed" });
        internalServerError.StatusCode = 500;
        return internalServerError;
    }

    public ObjectResult DeleteBookmarkedName(string nameId, string endpointName)
    {
        if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
        {
            var user = _unitOfWork
                        .UsersRepository
                        .GetUserWithNameBookmarks(userId);

            var bookmarkedNameByUser = user
                                        .UserBookmarkNames
                                        .FirstOrDefault(x => x.Nconst.Trim() == nameId);

            if (bookmarkedNameByUser != null)
            {
                _unitOfWork
                    .GetRepository<UserBookmarkName>()
                    .DeleteEntity(bookmarkedNameByUser);
              
                if (_unitOfWork.Save()) 
                {                              
                    var deleted = new ObjectResult(new { endpointName, Message = "Bookmarked name has been deleted", NameId = nameId });
                    deleted.StatusCode = 200;
                    return deleted;  
                }
            }
            
            var alreadyDeleted = new ObjectResult(new { endpointName, Message = "Bookmarked name has already been deleted", NameId = nameId });
            alreadyDeleted.StatusCode = 404;
            return alreadyDeleted; 

        }
        var internalServerError = new ObjectResult(new { message = "Request has not been processed" });
        internalServerError.StatusCode = 500;
        return internalServerError;
    }

    public ObjectResult DeleteAllBookmarkedNames(string endpointName)
    {
        if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
        {
            var user = _unitOfWork
                        .UsersRepository
                        .GetUserWithNameBookmarks(userId);

            var allBookmarkedNamesByUser = user
                                        .UserBookmarkNames;

            if (allBookmarkedNamesByUser.Count != 0)
            {
                _unitOfWork
                    .GetRepository<UserBookmarkName>()
                    .DeleteEntities(allBookmarkedNamesByUser);
              
                if (_unitOfWork.Save()) 
                {                              
                    var deleted = new ObjectResult(new { endpointName, Message = "All bookmarked names has been deleted" });
                    deleted.StatusCode = 200;
                    return deleted;  
                }
            }
            
            var alreadyDeleted = new ObjectResult(new { endpointName, Message = "All bookmarked names has already been deleted" });
            alreadyDeleted.StatusCode = 404;
            return alreadyDeleted; 

        }
        var internalServerError = new ObjectResult(new { message = "Request has not been processed" });
        internalServerError.StatusCode = 500;
        return internalServerError;
    }

    private BookmarkNameModel CreateBookmarkNameModel(string endpointName, UserBookmarkName entity)
    {
        TypeAdapterConfig<UserBookmarkName, BookmarkNameModel>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Nconst);
            
        var model = _mapper.Map<BookmarkNameModel>(entity);

        model.Url = GetUrl(endpointName, new { id = entity.Nconst });
        return model;
    }
}
