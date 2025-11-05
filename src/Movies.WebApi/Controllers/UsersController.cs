using Microsoft.AspNetCore.Mvc;
using Movies.Application.Common.Interfaces;
using Movies.Application.Features.Users.Models;

namespace Movies.WebApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IUsersHandler _usersHandler;

        public UsersController(IUsersHandler usersHandler)
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
            var isUserLoggedIn = _usersHandler.Login(model);
            if (isUserLoggedIn) return Ok(new { username = model.UserName });
            return BadRequest();
        }
    }
}
