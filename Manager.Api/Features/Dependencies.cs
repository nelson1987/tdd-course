using Manager.Api.Features.Accounts;
using Microsoft.EntityFrameworkCore;

namespace Manager.Api.Features;

public static class Dependencies
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();
        //services.AddSingleton<WriteDatabase>();
        services.AddDbContext<AccountContext>(x =>
        {
            x.UseInMemoryDatabase(databaseName: "InMemoryDatabase");
        });
        return services;
    }
}