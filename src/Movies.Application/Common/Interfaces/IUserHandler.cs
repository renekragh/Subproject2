using Movies.Application.Features.Users.Models;

namespace Movies.Application.Common.Interfaces;

public interface IUserHandler
{
    bool CreateUser(CreateUserModel model);
    object Login(LoginUserModel model);
}
