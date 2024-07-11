using Manager.Api.Features.Accounts;

namespace Manager.Api.Features;

public static class Dependencies
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();
        //services.AddSingleton<WriteDatabase>();
        return services;
    }
}