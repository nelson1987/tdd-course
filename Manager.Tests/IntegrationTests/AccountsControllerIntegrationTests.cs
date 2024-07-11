using AutoFixture;
using FluentAssertions;
using Manager.Api.Features;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Manager.Tests.IntegrationTests;

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

public class AccountsControllerIntegrationTests : IntegrationTestBase
{
    private readonly CreateAccountRequest _request;

    public AccountsControllerIntegrationTests() : base()
    {
        _request = _fixture.Build<CreateAccountRequest>()
            .Create();
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Request_Valido_Then_Retorna_Unathorized()
    {
        // Arrange
        var client = _server
            .WithWebHostBuilder(x =>
            {
                x.ConfigureTestServices(services =>
                 {
                     services.AddAuthentication(defaultScheme: "TestSchemeException")
                             .AddScheme<AuthenticationSchemeOptions, TestAuthHandlerException>("TestSchemeException", options => { });
                 });
            })
            .CreateClient();
        // Act
        HttpResponseMessage response = await CreateAccount(_request, client);
        // Assert
        response.Should().BeOfType<HttpResponseMessage>();
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Request_Valido_Then_Retorna_Created_And_Account_Create()
    {
        // Act
        HttpResponseMessage response = await CreateAccount(_request, _client);
        // Assert
        response.EnsureSuccessStatusCode();
        response.Should().BeOfType<HttpResponseMessage>();
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Request_Valido_Then_Account_In_Database()
    {
        //Act
        HttpResponseMessage response = await CreateAccount(_request, _client);
        Account? account = todoRepository.Accounts.FirstOrDefault(x => x.Description == _request.Description);
        // Assert
        account.Should().NotBeNull();
        account!.Id.Should().NotBe(0);
        account!.Description.Should().Be(_request.Description);
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Empty_Description_Then_Retorna_BadRequest()
    {
        // Arrange
        var request = _request with { Description = string.Empty };
        //Act
        HttpResponseMessage response = await CreateAccount(request, _client);
        // Assert
        response.Should().BeOfType<HttpResponseMessage>();
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Null_Description_Then_Retorna_BadRequest()
    {
        // Arrange
        var request = _request with { Description = null };
        //Act
        HttpResponseMessage response = await CreateAccount(request, _client);
        // Assert
        response.Should().BeOfType<HttpResponseMessage>();
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Empty_String_Description_Then_Retorna_BadRequest()
    {
        // Arrange
        var request = _request with { Description = "" };
        //Act
        HttpResponseMessage response = await CreateAccount(request, _client);
        // Assert
        response.Should().BeOfType<HttpResponseMessage>();
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Repositorio_Exception_Thrown_Then_Retorna_BadRequest()
    {
        // Arrange
        var client = _server
            .WithWebHostBuilder(x =>
            {
                x.ConfigureTestServices(services =>
                {
                    services.RemoveAll<IAccountRepository>();
                    services.AddScoped<IAccountRepository, InMemoryAccountRepository>();
                });
            })
            .CreateClient();
        // Act
        HttpResponseMessage response = await CreateAccount(_request, client);
        // Assert
        response.Should().BeOfType<HttpResponseMessage>();
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<HttpResponseMessage> CreateAccount(CreateAccountRequest request, HttpClient client)
    {
        var jsonContent = JsonConvert.SerializeObject(request);
        // Act
        var response = await client.PostAsync("/accounts",
            new StringContent(jsonContent, Encoding.UTF8, "application/json"));
        return response;
    }
}