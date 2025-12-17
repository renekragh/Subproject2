using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application.Common.Behaviors;
using Movies.Application.Common.Interfaces;

namespace Movies.WebApi.Controllers;

[Route("api/search-history")]
[Authorize]
public class SearchHistoryController : ControllerBase
{
    private readonly ISearchHistoryHandler _searchHistoryHandler;

    public SearchHistoryController(ISearchHistoryHandler searchHistoryHandler)
    {
       _searchHistoryHandler = searchHistoryHandler;
    }

    [HttpGet(Name = nameof(GetAllSearchHistory))]
    public IActionResult GetAllSearchHistory([FromQuery] Paging pagingParams)
    {
        pagingParams.EndpointName = nameof(GetAllSearchHistory);
        var searchHistoryModel = _searchHistoryHandler.GetAllSearchHistory(nameof(GetSearchHistory), pagingParams);
        if (searchHistoryModel is null) return StatusCode(500);
        return Ok(searchHistoryModel);
    }

    [HttpGet("{id}", Name = nameof(GetSearchHistory))]
    public IActionResult GetSearchHistory(int id)
    {
        var searchHistoryModel = _searchHistoryHandler.GetSearchHistory(nameof(GetSearchHistory), id);
        if (searchHistoryModel == null) return NotFound();
        return Ok(searchHistoryModel);
    }

    [HttpDelete("{id}", Name = nameof(DeleteSearchHistory))] 
    public IActionResult DeleteSearchHistory(int id)
    {
        var searchHistoryModel = _searchHistoryHandler.DeleteSearchHistory(nameof(GetSearchHistory), id);
        if (searchHistoryModel == null) return NotFound();
        return Ok(searchHistoryModel);
    }

    [HttpDelete(Name = nameof(DeleteAllSearchHistory))] 
    public IActionResult DeleteAllSearchHistory()
    {
        var searchHistoryModel = _searchHistoryHandler.DeleteAllSearchHistory(nameof(GetSearchHistory));
        if (searchHistoryModel == null) return NotFound();
        return Ok(searchHistoryModel);
    }
}

