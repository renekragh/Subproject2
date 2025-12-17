using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application.Common.Behaviors;
using Movies.Application.Common.Interfaces;

namespace Movies.WebApi.Controllers;

[Route("api/titles")]
[ApiController]
public class TitlesController : ControllerBase
{
    private readonly ITitlesHandler _titlesHandler;

    public TitlesController(ITitlesHandler titlesHandler)
    {
        _titlesHandler = titlesHandler;
    }

    [HttpGet(Name = nameof(GetTitles))]
    public IActionResult GetTitles([FromQuery] Paging pagingParams)
    {
       pagingParams.EndpointName = nameof(GetTitles);
        var titleListModel = _titlesHandler.RetrieveTitles(nameof(GetTitle), pagingParams);
        return Ok(titleListModel);
    }
    
    [HttpGet("{id}", Name = nameof(GetTitle))]
    public IActionResult GetTitle(string id)
    {
        var titleModel = _titlesHandler.RetrieveTitle(nameof(GetTitle), id);
        if (titleModel == null) return NotFound();
        return Ok(titleModel);
    }

    [AllowAnonymous] 
    [HttpGet("name/{name}", Name = nameof(GetTitlesByName))]
    public IActionResult GetTitlesByName(string name, [FromQuery] Paging pagingParams)
    {
        pagingParams.EndpointName = nameof(GetTitlesByName);
        var titleListModel = _titlesHandler.FindTitles(nameof(GetTitle), name, pagingParams);
        if (titleListModel == null) return NotFound();
        return Ok(titleListModel);
    }
}

