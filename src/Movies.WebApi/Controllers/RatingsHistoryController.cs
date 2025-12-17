using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application.Common.Behaviors;
using Movies.Application.Common.Interfaces;

namespace Movies.WebApi.Controllers;

[Route("api/ratings-history")]
[Authorize]
public class RatingHistoryController : ControllerBase
{
    private readonly IRatingHistoryHandler _ratingHistoryHandler;

    public RatingHistoryController(IRatingHistoryHandler ratingHistoryHandler)
    {
       _ratingHistoryHandler = ratingHistoryHandler; 
    }

    [HttpGet(Name = nameof(GetRatingHistories))]
    public IActionResult GetRatingHistories([FromQuery] Paging pagingParams)
    {
        pagingParams.EndpointName = nameof(GetRatingHistories);
        var ratingHistoryModel = _ratingHistoryHandler.RetrieveRatingHistories(nameof(GetRatingHistory), pagingParams);
        if (ratingHistoryModel is null) return StatusCode(500);
        return Ok(ratingHistoryModel);
    }

    [HttpGet("{id}", Name = nameof(GetRatingHistory))]
    public IActionResult GetRatingHistory(string id)
    {
        var ratingHistoryModel = _ratingHistoryHandler.RetrieveRatingHistory(nameof(GetRatingHistory), id);
        if (ratingHistoryModel == null) return NotFound();
        return Ok(ratingHistoryModel);

    }
}

