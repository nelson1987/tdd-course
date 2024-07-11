using AutoFixture;
using FluentAssertions;
using Manager.Api.Features.Accounts;
using Manager.Tests.BaseTests;
using Manager.Tests.Exceptions;
using Manager.Tests.InMemoryInfrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Manager.Tests.IntegrationTests;

public class AccountIntegrationTests : IntegrationTestBase
{
    public AccountIntegrationTests() : base()
    {
    }

    [Fact]
    public async Task Given_Insert_Novo_Account_When_GetById_Novo_Account_Then_Retorna_Novo_Account()
    {
        // Arrange
        var product = new Account();
        var dbContextOptions = new DbContextOptionsBuilder<ProductContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;

        using var postContext = new ProductContext(dbContextOptions);
        var productRepository = new AccountRepository(postContext);
        await productRepository.Insert(product);

        // Act
        using var getContext = new ProductContext(dbContextOptions);
        var getProductRepository = new AccountRepository(getContext);
        var retrievedProduct = await getProductRepository.GetById(product.Id);

        // Assert
        retrievedProduct.Should().NotBeNull();
        retrievedProduct.Value.Should().NotBeNull();
        retrievedProduct.Value.Id.Should().Be(product.Id);
        retrievedProduct.Value.Description.Should().Be(product.Description);
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
        HttpResponseMessage result = await CreateAccount(_request, _client);
        var jsonResponse = await result.Content.ReadAsStringAsync();
        //var response = JsonSerializer.Deserialize<int>(jsonResponse);
        var response = Convert.ToInt32(jsonResponse);
        // Act
        var getAccount = await todoRepository.GetById(response);// .FirstOrDefault(x => x.Description == _request.Description);
        var account = getAccount.Value;
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