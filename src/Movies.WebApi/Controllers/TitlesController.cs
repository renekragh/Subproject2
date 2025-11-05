using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [Authorize]
    public IActionResult GetTitles([FromQuery(Name = "name")] string? search, int page = 0, int pageSize = 10)
    {
        if (search != null) return GetTitlesByName(search, page, pageSize);
        var titleListModel = _titlesHandler.RetrieveTitles(page, pageSize);
        return Ok(titleListModel);
    }

    [HttpGet("{id}", Name = nameof(GetTitle))]
    public IActionResult GetTitle(string id)
    {
        var titleModel = _titlesHandler.RetrieveTitle(id);
        if (titleModel == null) return NotFound();
        return Ok(titleModel);
    }
  
    
    [HttpGet("name/{name}", Name = nameof(GetTitlesByPath))]
    public IActionResult GetTitlesByPath(string name, int page = 0, int pageSize = 25)
    {
        return GetTitlesByName(name, page, pageSize);
    }

    private IActionResult GetTitlesByName(string name, int page = 0, int pageSize = 25)
    {
        var titleListModel = _titlesHandler.FindTitles(name, page, pageSize);
        return Ok(titleListModel);
    }
}
