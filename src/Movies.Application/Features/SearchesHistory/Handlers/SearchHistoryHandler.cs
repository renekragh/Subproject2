using System.Security.Claims;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Movies.Application.Common.Behaviors;
using Movies.Application.Common.Interfaces;
using Movies.Application.Features.SearchesHistory.Models;
using Movies.Domain.Entities;

namespace Movies.Application.Features.SearchesHistory.Handlers;

public class SearchHistoryHandler : BaseHandler, ISearchHistoryHandler
{
   
    public SearchHistoryHandler(IUnitOfWork unitOfWork, 
                                 LinkGenerator generator, 
                                 IHttpContextAccessor httpContextAccessor, 
                                 IMapper mapper) : base(unitOfWork, generator, httpContextAccessor, mapper) {}
                                

    public object GetAllSearchHistory(string endpointName, Paging pagingParams)
    {
        if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
         {
            var allSearchHistory = _unitOfWork
                                    .UsersRepository
                                    .GetUserWithSearchHistory(userId)
                                    .SearchHistories
                                    .Select(x => CreateSearchHistoryModel(endpointName, x));

            var numOfItems = allSearchHistory.Count();

            return CreatePaging(searchQuery: null, allSearchHistory, numOfItems, pagingParams);
         }
         return null;
    }

    public object GetSearchHistory(string endpointName, int id)
    {
        if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
        {
            var searchHistory = _unitOfWork
                                    .UsersRepository
                                    .GetUserWithSearchHistory(userId)
                                    .SearchHistories
                                    .FirstOrDefault(x => x.Searchid == id);
       
            if (searchHistory == null) return null;
            var SearchHistoryModel = CreateSearchHistoryModel(endpointName, searchHistory);
            return CreateLinks(endpointName, new { id = searchHistory.Searchid }, SearchHistoryModel);
        }
        return null;
    }

        public object DeleteSearchHistory(string endpointName, int id)
    {
        if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
        {
            var searchHistory = _unitOfWork
                                    .UsersRepository
                                    .GetUserWithSearchHistory(userId)
                                    .SearchHistories
                                    .FirstOrDefault(x => x.Searchid == id);
            
            if (searchHistory is null)
            {
                var badRequest = new ObjectResult(new 
                                                    { 
                                                        endpointName,
                                                        Message = "Request has already been processed", 
                                                        SearchId = id
                                                    }
                                              );
                badRequest.StatusCode = 400;
                return badRequest;
            }

            bool isSearchHistoryRemoved = _unitOfWork
                                            .UsersRepository
                                            .GetUserWithSearchHistory(userId)
                                            .SearchHistories.Remove(searchHistory);
/*
            _unitOfWork
                .GetRepository<SearchHistory>()
                .DeleteEntity(searchHistory);
*/

            if (isSearchHistoryRemoved && _unitOfWork.Save()) 
            {
                var deleted = new ObjectResult(new { endpointName, Message = "Search history has been deleted", Id = id });
                deleted.StatusCode = 200;
                return deleted;  
            }
            var internalServerError = new ObjectResult(new { message = "Request has not been processed" });
            internalServerError.StatusCode = 500;
            return internalServerError;
        }
        return null;
    }

    public object DeleteAllSearchHistory(string endpointName)
    {
        if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
        {
            var allSearchHistory = _unitOfWork
                                    .UsersRepository
                                    .GetUserWithSearchHistory(userId)
                                    .SearchHistories;
            
            if (allSearchHistory.Count == 0)
            {
                var badRequest = new ObjectResult(new 
                                                    { 
                                                        endpointName,
                                                        Message = "Request has already been processed", 
                                                    }
                                              );
                badRequest.StatusCode = 400;
                return badRequest;
            }

            _unitOfWork
                .UsersRepository
                .DeleteAllSearchHistoryFromUser(userId);

            if (_unitOfWork.Save())
            {
                var deleted = new ObjectResult(new { endpointName, Message = "All search histories has been deleted" });
                deleted.StatusCode = 200;
                return deleted;  
            }

            var internalServerError = new ObjectResult(new { message = "Request has not been processed" });
            internalServerError.StatusCode = 500;
            return internalServerError;
        }
        return null;
    }

    private SearchHistoryModel CreateSearchHistoryModel(string endpointName, SearchHistory entity)
    {
        TypeAdapterConfig<SearchHistory, SearchHistoryModel>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Searchid);

        var model = _mapper.Map<SearchHistoryModel>(entity);
        model.Url = GetUrl(endpointName, new { id = entity.Searchid });
        return model;
    }
}
