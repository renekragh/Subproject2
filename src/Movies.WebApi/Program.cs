
using System.Text;
using System.Text.Json.Serialization;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Movies.Application.Common.Behaviors;
using Movies.Application.Common.Interfaces;
using Movies.Application.Features.Titles.Handlers;
using Movies.Domain.Interfaces;
using Movies.Infrastructure;
using Movies.Infrastructure.DataContext;
using Movies.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Dependency Injection
var connectionString = builder.Configuration.GetSection("ConnectionString").Value;
builder.Services.AddDbContext<PostgresDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ITitlesHandler, TitlesHandler>();
builder.Services.AddScoped<IUsersHandler, UsersHandler>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMapster();
builder.Services.AddSingleton(new Hashing());

var secret = builder.Configuration.GetSection("Auth:Secret").Value;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ClockSkew = TimeSpan.Zero
        }
    );

// if the object depth is larger than the maximum allowed depth of 32
// then using ReferenceHandler.Preserve on JsonSerializerOptions to support cycles
builder.Services.AddControllers()
.AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();