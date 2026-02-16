using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application.Common.Interfaces;

namespace Movies.WebApi.Controllers;

[ApiController]
public class RatingsController : ControllerBase
{
    private readonly IRatingsHandler _ratingsHandler;
    private const string GetRating = "GetRating";

    public RatingsController(IRatingsHandler ratingsHandler)
    {
        _ratingsHandler = ratingsHandler;
    }

    [Authorize]
    [HttpPost("api/titles/{id}/ratings", Name = nameof(RateTitle))] 
    public IActionResult RateTitle(string id, [FromHeader(Name = "Idempotency-Key")] string key, [FromBody] int rate)
    {
        return _ratingsHandler.RateTitle(id, key, rate, nameof(GetRating));
    }

    [Authorize]
    [HttpPut("api/titles/{id}/ratings", Name = nameof(UpdateRate))] 
    public IActionResult UpdateRate(string id, [FromBody] int rate)
    {
        return _ratingsHandler.UpdateRate(id, rate, nameof(GetRating));
    }

    [Authorize]
    [HttpDelete("api/titles/{id}/ratings", Name = nameof(DeleteRating))] 
    public IActionResult DeleteRating(string id)
    {
        return _ratingsHandler.DeleteRating(id, nameof(GetRating));
    }
}