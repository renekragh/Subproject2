using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Movies.Infrastructure.DataContext;

namespace Movies.Presentation.IntegrationTests.Util;

public static class ServiceCollectionExtensions
{
    public static void RemoveDbContext<T>(this IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<PostgresDbContext>)); 
        if (descriptor != null) services.Remove(descriptor);
    }

    public static void EnsureCreated<T>(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<PostgresDbContext>();
        //db.Database.Migrate(); 
        db.Database.EnsureCreated(); 

        DbHelper.InitDbForTests(db);
    }
}
