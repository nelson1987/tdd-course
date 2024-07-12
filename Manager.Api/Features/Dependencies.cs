using Manager.Api.Features.Accounts;
using Microsoft.EntityFrameworkCore;

namespace Manager.Api.Features;

public static class Dependencies
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();

        services.AddDbContext<AccountContext>(x =>
        {
            x.UseInMemoryDatabase(databaseName: "InMemoryDatabase");
#if DEBUG
            x.ConfiguredLog(services);
#endif
        });
        return services;
    }

    private static DbContextOptionsBuilder ConfiguredLog(this DbContextOptionsBuilder dbContextOptionsBuilder, IServiceCollection services)
    {
        ILogger logger = services.BuildServiceProvider().GetRequiredService<ILogger<DbContextOptionsBuilder>>();
        logger.LogInformation("DbContextOptionsBuilder configured");
        dbContextOptionsBuilder.EnableDetailedErrors();
        dbContextOptionsBuilder.EnableSensitiveDataLogging();
        dbContextOptionsBuilder.LogTo(x => logger.LogInformation(x));
        return dbContextOptionsBuilder;
    }
}