using System.Security.Claims;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Movies.Application.Common.Behaviors;
using Movies.Application.Common.Interfaces;
using Movies.Application.Features.Bookmarks.Models;
using Movies.Domain.Entities;

namespace Movies.Application.Features.Bookmarks.Handlers;

public class BookmarkTitlesHandler : BaseHandler, IBookmarkTitlesHandler
{
    public BookmarkTitlesHandler(IUnitOfWork unitOfWork, 
                                 LinkGenerator generator, 
                                 IHttpContextAccessor httpContextAccessor, 
                                 IMapper mapper) : base(unitOfWork, generator, httpContextAccessor, mapper) {}
    public ObjectResult BookmarkTitle(string id, string key, string note, string endpointName)
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
                                        .GetUserWithTitleBookmarks(userId)
                                        .UserBookmarkTitles
                                        .FirstOrDefault(x => x.Tconst == id);
                                      
                var badRequest = new ObjectResult(new 
                                                    { 
                                                        message = "Request has already been processed", 
                                                        item = CreateBookmarkTitleModel(endpointName, existingEntity) 
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
                .GetUserWithTitleBookmarks(userId)
                .UserBookmarkTitles.Add(new UserBookmarkTitle { Tconst = id, Userid = userId, Note = note });

            if (_unitOfWork.Save()) {
                var newEntity = _unitOfWork
                                    .GetRepository<UserBookmarkTitle>()
                                    .RetrieveEntity(x => x.Tconst == id);
                var created = new ObjectResult(new { endpointName, item = CreateBookmarkTitleModel(endpointName, newEntity) });
                created.StatusCode = 201;
                return created;
            }
        }

        var internalServerError = new ObjectResult(new { message = "Request has not been processed" });
        internalServerError.StatusCode = 500;
        return internalServerError;
    }

    public object GetBookmarkedTitles(string endpointName, Paging pagingParams)
    {
         if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
         {
            var titleBookmarks = _unitOfWork
                                    .UsersRepository
                                    .GetUserWithTitleBookmarks(userId)
                                    .UserBookmarkTitles
                                    .Select(x => CreateBookmarkTitleModel(endpointName, x));
                            
            var numOfItems = titleBookmarks.Count();
            pagingParams.EndpointName = endpointName;

            return CreatePaging(searchQuery: null, titleBookmarks, numOfItems, pagingParams);
         }
         return null;
    }

    public object GetBookmarkedTitle(string endpointName, string titleId)
    {
         if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
         {
            var titleBookmark = _unitOfWork
                                 .UsersRepository
                                 .GetUserWithTitleBookmarks(userId)
                                 .UserBookmarkTitles
                                 .FirstOrDefault(x => x.Tconst == titleId);

            if (titleBookmark == null) return null;
            var titleBookmarkModel = CreateBookmarkTitleModel(endpointName, titleBookmark);
            return CreateLinks(endpointName, new { id = titleId }, titleBookmarkModel);
         }
         return null;
    }

    public ObjectResult UpdateBookmarkedTitle(string titleId, string note, string endpointName)
    {
        if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
        {
            var user = _unitOfWork
                        .UsersRepository
                        .GetUserWithTitleBookmarks(userId);
            user
                .UserBookmarkTitles
                .FirstOrDefault(x => x.Tconst == titleId)
                .Note = note;               

            _unitOfWork
                .GetRepository<ImdbUser>()
                .UpdateEntity(user);
              
            if (_unitOfWork.Save()) 
            {                              
                var updatedBookmarkedTitle = _unitOfWork
                                                .UsersRepository
                                                .GetUserWithTitleBookmarks(userId)
                                                .UserBookmarkTitles
                                                .FirstOrDefault(x => x.Tconst == titleId);

                var updated = new ObjectResult(new { endpointName, item = CreateBookmarkTitleModel(endpointName, updatedBookmarkedTitle) });
                updated.StatusCode = 200;
                return updated;   
            }
        }
        var internalServerError = new ObjectResult(new { message = "Request has not been processed" });
        internalServerError.StatusCode = 500;
        return internalServerError;
    }

    public ObjectResult DeleteBookmarkedTitle(string titleId, string endpointName)
    {
        if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
        {
            var user = _unitOfWork
                        .UsersRepository
                        .GetUserWithTitleBookmarks(userId);

            var bookmarkedTitleByUser = user
                                        .UserBookmarkTitles
                                        .FirstOrDefault(x => x.Tconst == titleId);

            if (bookmarkedTitleByUser != null)
            {
                _unitOfWork
                    .GetRepository<UserBookmarkTitle>()
                    .DeleteEntity(bookmarkedTitleByUser);
              
                if (_unitOfWork.Save()) 
                {                              
                    var deleted = new ObjectResult(new { endpointName, Message = "Bookmarked title has been deleted", TitleId = titleId });
                    deleted.StatusCode = 200;
                    return deleted;  
                }
            }
            
            var alreadyDeleted = new ObjectResult(new { endpointName, Message = "Bookmarked title has already been deleted", TitleId = titleId });
            alreadyDeleted.StatusCode = 404;
            return alreadyDeleted; 

        }
        var internalServerError = new ObjectResult(new { message = "Request has not been processed" });
        internalServerError.StatusCode = 500;
        return internalServerError;
    }

    public ObjectResult DeleteAllBookmarkedTitles(string endpointName)
    {
        if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
        {
            var user = _unitOfWork
                        .UsersRepository
                        .GetUserWithTitleBookmarks(userId);

            var allBookmarkedTitlesByUser = user
                                        .UserBookmarkTitles;

            if (allBookmarkedTitlesByUser.Count != 0)
            {
                _unitOfWork
                    .GetRepository<UserBookmarkTitle>()
                    .DeleteEntities(allBookmarkedTitlesByUser);
              
                if (_unitOfWork.Save()) 
                {                              
                    var deleted = new ObjectResult(new { endpointName, Message = "All bookmarked titles has been deleted" });
                    deleted.StatusCode = 200;
                    return deleted;  
                }
            }
            
            var alreadyDeleted = new ObjectResult(new { endpointName, Message = "All bookmarked titles has already been deleted" });
            alreadyDeleted.StatusCode = 404;
            return alreadyDeleted; 

        }
        var internalServerError = new ObjectResult(new { message = "Request has not been processed" });
        internalServerError.StatusCode = 500;
        return internalServerError;
    }

    private BookmarkTitleModel CreateBookmarkTitleModel(string endpointName, UserBookmarkTitle entity)
    {
        var model = _mapper.Map<BookmarkTitleModel>(entity);
        model.Url = GetUrl(endpointName, new { id = entity.Tconst });
        return model;
    }
}
