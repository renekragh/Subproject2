using Microsoft.AspNetCore.Mvc;
using Movies.Application.Common.Interfaces;
using Movies.Domain.Entities;

namespace Movies.WebApi.Controllers;

[Route("api/names")]
[ApiController]
public class NamesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    public NamesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
/*
    [HttpGet]
    public IActionResult GetNames([FromQuery] string? name)
    {
        if (name != null) return GetNamesByName(name);
        var names = _unitOfWork.GetRepository<Name>().RetrieveEntities();
        return Ok(names);
    }
*/
    [HttpGet("{id}")]
    public IActionResult GetName(string id)
    {
        var name = _unitOfWork.GetRepository<Name>().FindEntity(id);
        if (name == null) return NotFound();
        // var model = CreateCategoryModel(category);
        return Ok(name);
    }
 /*
    [HttpGet("name/{name}")]
    public IActionResult GetNamesByPath(string name)
    {
        return GetNamesByName(name);
    }
   
    private IActionResult GetNamesByName(string name)
    {
        var results = _unitOfWork.GetRepository<Name>().FindEntities(x => x.Primaryname!.Contains(name));
        if (results.Any()) return Ok(results);
        return NotFound(results);
    }
    */
}

