using Manager.Api.Features.Accounts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Manager.Tests.BaseTests;

public class ApiFixture : WebApplicationFactory<Program>
{
    static ApiFixture()
        => Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Test");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
        => builder.UseEnvironment("Testing")
                  .ConfigureTestServices(services =>
                  {
                      //Authentication
                      services.AddAuthentication(defaultScheme: "TestScheme")
                              .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });
                      //DataContext
                      services.AddDbContext<AccountContext>(x =>
                      {
                          x.UseInMemoryDatabase(databaseName: "InMemoryDatabase");
                      });
                  });

    private class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] {
                new Claim(ClaimTypes.Name, "Test user"),
                new Claim("preferred_username", "user@email.com.br")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestScheme");

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}