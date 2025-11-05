using System;
using Movies.Application.Features.Users.Models;
using Movies.Domain.Entities;

namespace Movies.Application.Common.Interfaces;

public interface IUsersHandler
{
    bool CreateUser(CreateUserModel model);
    bool Login(LoginUserModel model);
}
