using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Movies.Infrastructure.DataContext;
using Movies.Presentation.IntegrationTests.Util;
using Testcontainers.PostgreSql;
using WebMotions.Fake.Authentication.JwtBearer;

namespace Movies.Presentation.IntegrationTests.Fixtures;

public class CustomWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{

    private PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
           services.RemoveDbContext<PostgresDbContext>();
           services.AddDbContext<PostgresDbContext>(x => x.UseNpgsql(_postgreSqlContainer.GetConnectionString()));
           services.EnsureCreated<PostgresDbContext>();
           services.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme)
                   .AddFakeJwtBearer(opt => opt.BearerValueType = FakeJwtBearerBearerValueType.Jwt);
        });
    }

    Task IAsyncLifetime.DisposeAsync() => _postgreSqlContainer.DisposeAsync().AsTask();
}
