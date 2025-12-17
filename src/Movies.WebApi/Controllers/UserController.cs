using Microsoft.AspNetCore.Mvc;
using Movies.Application.Common.Interfaces;
using Movies.Application.Features.Users.Models;

namespace Movies.WebApi.Controllers;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{

    private readonly IUserHandler _usersHandler;

    public UserController(IUserHandler usersHandler)
    {
         _usersHandler = usersHandler;
    }

    [HttpPost]
    public IActionResult CreateUser(CreateUserModel model)
    {
        var isUserCreated = _usersHandler.CreateUser(model);
        if (isUserCreated) return Ok();
        return BadRequest();
    }

    [HttpPut]
    public ActionResult Login(LoginUserModel model)
    {
        var user = _usersHandler.Login(model);
        if (user is null) return BadRequest();
        return Ok(user);
    }
}

