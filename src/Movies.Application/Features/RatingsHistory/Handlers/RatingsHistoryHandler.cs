using System.Security.Claims;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Movies.Application.Common.Behaviors;
using Movies.Application.Common.Interfaces;
using Movies.Application.Features.RatingsHistory.Models;
using Movies.Domain.Entities;

namespace Movies.Application.Features.RatingsHistory.Handlers;

public class RatingHistoryHandler : BaseHandler, IRatingHistoryHandler
{
    private const string GetTitle = "GetTitle";

       public RatingHistoryHandler(IUnitOfWork unitOfWork, 
                          LinkGenerator generator, 
                          IHttpContextAccessor httpContextAccessor, 
                          IMapper mapper) : base(unitOfWork, generator, httpContextAccessor, mapper) {}

    
    public object RetrieveRatingHistories(string endpointName, Paging pagingParams)
    {
         if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
         {
            var ratingHistories = _unitOfWork
                                    .UsersRepository
                                    .GetUserWithRatingHistory(userId)
                                    .RatingHistories
                                    .Select(x => CreateRatingHistoryModel(GetTitle, x));
                            
            var numOfItems = ratingHistories.Count();

            return CreatePaging(searchQuery: null, ratingHistories, numOfItems, pagingParams);
         }
         return null;
        
    }

    public object RetrieveRatingHistory(string endpointName, string titleId)
    {
         if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
         {
            var ratingHistory = _unitOfWork
                                 .UsersRepository
                                 .GetUserWithRatingHistory(userId)
                                 .RatingHistories
                                 .FirstOrDefault(x => x.Tconst == titleId);

            if (ratingHistory == null) return null;
            var ratingHistoryModel = CreateRatingHistoryModel(GetTitle, ratingHistory);
            return CreateLinks(GetTitle, new { id = titleId }, ratingHistoryModel);
         }
         return null;
    }

    private RatingHistoryModel CreateRatingHistoryModel(string endpointName, RatingHistory entity)
    {
        var ratingHistoryModel = _mapper.Map<RatingHistoryModel>(entity);
        ratingHistoryModel.Url = GetUrl(endpointName, new { id = entity.Tconst });
        return ratingHistoryModel;
    }
}
