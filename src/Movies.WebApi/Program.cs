
using System.Text;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Movies.Application.Common.Behaviors;
using Movies.Application.Common.Interfaces;
using Movies.Application.Features.Bookmarks.Handlers;
using Movies.Application.Features.RatingsHistory.Handlers;
using Movies.Application.Features.Ratings.Handlers;
using Movies.Application.Features.Titles.Handlers;
using Movies.Domain.Interfaces;
using Movies.Infrastructure;
using Movies.Infrastructure.DataContext;
using Movies.Infrastructure.Repositories;
using Movies.Application.Features.SearchesHistory.Handlers;
using Movies.Application.Features.Names.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Dependency Injection
var connectionString = builder.Configuration.GetSection("ConnectionString").Value;
builder.Services.AddDbContext<PostgresDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<INamesRepository, NamesRepository>();
builder.Services.AddScoped<ITitlesRepository, TitlesRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<INamesHandler, NamesHandler>();
builder.Services.AddScoped<ITitlesHandler, TitlesHandler>();
builder.Services.AddScoped<IRatingsHandler, RatingsHandler>();
builder.Services.AddScoped<IBookmarkNamesHandler, BookmarkNamesHandler>();
builder.Services.AddScoped<IBookmarkTitlesHandler, BookmarkTitlesHandler>();
builder.Services.AddScoped<IUserHandler, UserHandler>();
builder.Services.AddScoped<IRatingHistoryHandler, RatingHistoryHandler>();
builder.Services.AddScoped<ISearchHistoryHandler, SearchHistoryHandler>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(new Hashing());
builder.Services.AddMapster();

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

builder.Services.AddControllers();
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod()
.WithOrigins("http://localhost:3000", "https://localhost:3000"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

public partial class Program {}