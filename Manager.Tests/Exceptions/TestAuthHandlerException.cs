using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Manager.Tests.Exceptions;

public class TestAuthHandlerException : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandlerException(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] {
                new Claim(ClaimTypes.Name, "Test user"),
                new Claim("preferred_username", "user@email.com.br")
            };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestSchemeException");

        var result = AuthenticateResult.Fail(new Exception());

        return Task.FromResult(result);
    }
}