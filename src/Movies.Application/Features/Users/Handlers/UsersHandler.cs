using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Movies.Application.Common.Behaviors;
using Movies.Application.Common.Interfaces;
using Movies.Application.Features.Users;
using Movies.Application.Features.Users.Models;
using Movies.Domain.Entities;

namespace Movies.Application.Features.Titles.Handlers;

public class UsersHandler : IUsersHandler
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly LinkGenerator _generator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly Hashing _hashing;

    public UsersHandler(IConfiguration configuration, IUnitOfWork unitOfWork, LinkGenerator generator, IHttpContextAccessor httpContextAccessor, IMapper mapper, Hashing hashing)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _generator = generator;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _hashing = hashing;
    }

    public bool CreateUser(CreateUserModel model)
    {
        if (string.IsNullOrEmpty(model.Password)) return false;
        var entity = _unitOfWork
                        .GetRepository<ImdbUser>()
                        .RetrieveEntity(x => x.UserName == model.UserName);
        if (entity != null) return false;

        (var hashedPwd, var salt) = _hashing.Hash(model.Password);
        var user = _mapper.Map<ImdbUser>(model);
        user.Password = hashedPwd; // don't save real user Password, only the hashed one!
        user.Salt = salt; // TO DO: change this to an extra attribute 'salt' :)
        user.Role = Role.User.ToString();
        _unitOfWork.GetRepository<ImdbUser>()
                    .CreateEntity(user);
        return _unitOfWork.Save();
    }

    public bool Login(LoginUserModel model)
    {
        var entity = _unitOfWork.GetRepository<ImdbUser>()
                                .RetrieveEntity(x => x.UserName == model.UserName);
        if (entity == null) return false;

        var isPwdVerified = _hashing.Verify(model.Password, entity.Password, entity.Salt); // TO DO add extra attribute Salt instead of Email!
        if (!isPwdVerified) return false;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, entity.Name),
            new Claim(ClaimTypes.Role, entity.Role) //--> Role TO DO add extra attribute Role and keep Timestamp as is!
        };

        var secret = _configuration.GetSection("Auth:Secret").Value;

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddSeconds(45),
                signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        Console.WriteLine("**** jwt: " + jwt);
        return true;
    }
}
