using Manager.Api.Controllers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Manager.Tests.IntegrationTests;

public class AccountsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly HttpClient _client;
    private readonly CreateAccountRequest _request;

    public AccountsControllerIntegrationTests(WebApplicationFactory<Program> factory)// : base()
    {
        _request = new CreateAccountRequest("Descrição");
        _webApplicationFactory = factory;
        _client = _webApplicationFactory.CreateClient();
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Request_Valido_Then_Retorna_Created()
    {
        var jsonContent = JsonConvert.SerializeObject(_request);
        StringContent content = new StringContent(jsonContent,
            Encoding.UTF8, "application/json");
        // Act
        var response = await _client.PostAsync("/accounts", content);
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Empty_Description_Then_Retorna_BadRequest()
    {
        // Arrange
        var request = _request with { Description = string.Empty };
        var jsonContent = JsonConvert.SerializeObject(request);
        // Act
        var response = await _client.PostAsync("/accounts",
            new StringContent(jsonContent, Encoding.UTF8, "application/json"));
        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Null_Description_Then_Retorna_BadRequest()
    {
        // Arrange
        var request = _request with { Description = null };
        var jsonContent = JsonConvert.SerializeObject(request);
        // Act
        var response = await _client.PostAsync("/accounts",
            new StringContent(jsonContent, Encoding.UTF8, "application/json"));
        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Empty_String_Description_Then_Retorna_BadRequest()
    {
        // Arrange
        var request = _request with { Description = "" };
        var jsonContent = JsonConvert.SerializeObject(request);
        // Act
        var response = await _client.PostAsync("/accounts",
            new StringContent(jsonContent, Encoding.UTF8, "application/json"));
        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Repositorio_Exception_Thrown_Then_Retorna_BadRequest()
    {
        var client = _webApplicationFactory
            .WithWebHostBuilder(x =>
            {
                x.ConfigureServices(services =>
                {
                    services.RemoveAll<IAccountRepository>();
                    services.AddScoped<IAccountRepository, InMemoryAccountRepository>();
                });
            })
            .CreateClient();
        // Arrange
        var jsonContent = JsonConvert.SerializeObject(_request);
        // Act
        var response = await client.PostAsync("/accounts",
            new StringContent(jsonContent, Encoding.UTF8, "application/json"));
        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Repositorio_Exception_Async_Thrown_Then_Retorna_BadRequest()
    {
        // Arrange
        var client = _webApplicationFactory
            .WithWebHostBuilder(x =>
            {
                x.ConfigureServices(services =>
                {
                    services.RemoveAll<IAccountRepository>();
                    services.AddScoped<IAccountRepository, InMemoryAccountRepository>();
                });
            })
            .CreateClient();
        var jsonContent = JsonConvert.SerializeObject(_request);
        // Act
        var response = await client.PostAsync("/accounts",
            new StringContent(jsonContent, Encoding.UTF8, "application/json"));
        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

public class InMemoryAccountRepository : IAccountRepository
{
    public Task<Account> Insert(Account account)
    {
        throw new Exception();
    }
}