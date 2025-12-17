using Microsoft.AspNetCore.Mvc;
using Movies.Application.Common.Behaviors;
using Movies.Application.Common.Interfaces;

namespace Movies.WebApi.Controllers;

[Route("api/names")]
[ApiController]
public class NamesController : ControllerBase
{
    private readonly INamesHandler _namesHandler;

    public NamesController(INamesHandler namesHandler)
    {
        _namesHandler = namesHandler;
    }

    [HttpGet(Name = nameof(GetNames))]
    public IActionResult GetNames([FromQuery] Paging pagingParams)
    {
        pagingParams.EndpointName = nameof(GetNames);
        var nameListModel = _namesHandler.GetNames(nameof(GetName), pagingParams);
        return Ok(nameListModel);
    }
    
    [HttpGet("{id}", Name = nameof(GetName))]
    public IActionResult GetName(string id)
    {
        var nameModel = _namesHandler.GetName(nameof(GetName), id);
        if (nameModel == null) return NotFound();
        return Ok(nameModel);
    }
 
    [HttpGet("name/{name}", Name = nameof(FindNamesByName))]
    public IActionResult FindNamesByName(string name, [FromQuery] Paging pagingParams)
    {
        pagingParams.EndpointName = nameof(FindNamesByName);
        var nameListModel = _namesHandler.FindNames(nameof(GetName), name, pagingParams);
        if (nameListModel == null) return NotFound();
        return Ok(nameListModel);
    }
}

