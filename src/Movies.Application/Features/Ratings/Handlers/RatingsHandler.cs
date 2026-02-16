using System.Security.Claims;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Movies.Application.Common.Behaviors;
using Movies.Application.Common.Interfaces;
using Movies.Application.Features.Ratings.Models;
using Movies.Domain.Entities;

namespace Movies.Application.Features.Ratings.Handlers;

public class RatingsHandler : BaseHandler, IRatingsHandler
{    
    public RatingsHandler(IUnitOfWork unitOfWork, 
                          LinkGenerator generator, 
                          IHttpContextAccessor httpContextAccessor, 
                          IMapper mapper) : base(unitOfWork, generator, httpContextAccessor, mapper) {}

    public ObjectResult RateTitle(string titleId, string key, int rate, string endpointName)
    {
        if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
        {
            var idempotencyKey = _unitOfWork
                                    .GetRepository<Idempotency>()
                                    .RetrieveEntity(x => x.Key.ToString() == key);

            if (idempotencyKey != null)
            {
                var isTitleInRateHistory = _unitOfWork
                                            .UsersRepository
                                            .GetUserWithRatingHistory(userId)
                                            .RatingHistories
                                            .Any(x => x.Tconst == titleId);
                if (isTitleInRateHistory)
                {
                    var existingEntity = _unitOfWork
                                            .GetRepository<Title>()
                                            .RetrieveEntity(x => x.Tconst == titleId)
                                            .Ratings
                                            .FirstOrDefault();
                                            
                    var badRequest = new ObjectResult(new 
                                                    { 
                                                        message = "Request has already been processed", 
                                                        item = CreateRateModel(endpointName, existingEntity) 
                                                    }
                                                );
                    badRequest.StatusCode = 400;
                    return badRequest;
                }
            }

            //Use stored psql function rate(...) through EF
            var isRated = _unitOfWork.TitlesRepository.CreateTitleRating(titleId, rate, userId);
            if (isRated) {
                _unitOfWork
                    .GetRepository<Idempotency>()
                    .CreateEntity(new Idempotency { Key = Guid.Parse(key), CreatedAt = DateTimeOffset.Now.ToUnixTimeSeconds() });
                
                if (_unitOfWork.Save()) {
                    var newRating = _unitOfWork
                                        .TitlesRepository
                                        .GetTitleWithRating(titleId)
                                        .Ratings.FirstOrDefault(x => x.Tconst == titleId);

                    var created = new ObjectResult(new { endpointName, item = CreateRateModel(endpointName, newRating) });
                    created.StatusCode = 201;
                    return created;   
                }
            } 
        }        

        var internalServerError = new ObjectResult(new { message = "Request has not been processed" });
        internalServerError.StatusCode = 500;
        return internalServerError;
    }

    public ObjectResult UpdateRate(string titleId, int updatedRate, string endpointName)
    {
        if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
        {
            var existingRate =_unitOfWork
                                .UsersRepository
                                .GetUserWithRatingHistory(userId)
                                .RatingHistories
                                .FirstOrDefault(x => x.Tconst == titleId)
                                .Rating;

            //Use stored psql function update_rate(...) through EF
            var isRateUpdated = _unitOfWork.TitlesRepository
                                    .UpdateTitleRating(titleId, existingRate, updatedRate, userId);

            if (isRateUpdated) 
            {
               // _unitOfWork.Save() returns false as psql fuction update_rate(...) already have updated and saved entity to DB!                              
                var updatedRatingEntity = _unitOfWork
                                            .TitlesRepository
                                            .GetTitleWithRating(titleId)
                                            .Ratings
                                            .FirstOrDefault(x => x.Tconst == titleId);

                var updated = new ObjectResult(new { endpointName, item = CreateRateModel(endpointName, updatedRatingEntity) });
                updated.StatusCode = 200;
                return updated;   
            }
        }
        var internalServerError = new ObjectResult(new { message = "Request has not been processed" });
        internalServerError.StatusCode = 500;
        return internalServerError;
    }

    public ObjectResult DeleteRating(string titleId, string endpointName)
    {
        if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
        {
            var rate = _unitOfWork
                            .UsersRepository
                            .GetUserWithRatingHistory(userId)
                            .RatingHistories
                            .FirstOrDefault(x => x.Tconst == titleId);

            if (rate is null)
            {
                var badRequest = new ObjectResult(new 
                                                    { 
                                                        endpointName,
                                                        Message = "Request has already been processed", 
                                                        TitleId = titleId 
                                                    }
                                              );
                badRequest.StatusCode = 400;
                return badRequest;
            }

            //Use stored psql function delete_rating(...) through EF
            var isRateDeleted = _unitOfWork.TitlesRepository.DeleteTitleRating(titleId, rate.Rating, userId);
            if (isRateDeleted)
            {
                // _unitOfWork.Save() returns false as psql fuction delete_rating(...) already have deleted and updated DB!   
                var deleted = new ObjectResult(new { endpointName, Message = "Rating vote has been deleted", TitleId = titleId });
                deleted.StatusCode = 200;
                return deleted;   
            }
        }

        var internalServerError = new ObjectResult(new { message = "Request has not been processed" });
        internalServerError.StatusCode = 500;
        return internalServerError;
    }

    private RatingModel CreateRateModel(string endpointName, Rating entity)
    {
        var model = _mapper.Map<RatingModel>(entity);
        model.Url = GetUrl(endpointName, new { id = entity.Ratingid });
        return model;
    }
}
