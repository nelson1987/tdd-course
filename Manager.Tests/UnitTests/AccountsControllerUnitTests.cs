using AutoFixture;
using FluentAssertions;
using FluentResults;
using Manager.Api.Features.Accounts;
using Manager.Tests.BaseTests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace Manager.Tests.UnitTests;

/*
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
}*/

//using Microsoft.Extensions.Logging;
//using Moq;
//using System;
//using Xunit;

//namespace Manager.Api.Features.Accounts.Tests
//{
//    public class AccountsControllerTests
//    {
//        private Mock<IAccountRepository> _accountRepositoryMock;
//        private Mock<ILogger<AccountsController>> _loggerMock;
//        private AccountsController _sut;

// public AccountsControllerTests() { _accountRepositoryMock = new Mock<IAccountRepository>();
// _loggerMock = new Mock<ILogger<AccountsController>>(); _sut = new
// AccountsController(_accountRepositoryMock.Object, _loggerMock.Object); }

// [Fact] public void Post_LogsInformation_WhenCalled() { // Arrange var request = new
// CreateAccountRequest { Description = "Test Account" };

// // Act _sut.Post(request);

//            // Assert
//            _loggerMock.Verify(l => l.Log(
//                LogLevel.Information,
//                It.IsAny<EventId>(),
//                It.Is<FormattedLogValues>((v, t) => v.ToString().Contains("Started")),
//                It.IsAny<Exception>(),
//                It.Is<Func<FormattedLogValues, Exception, string>>((v, t) => true)),
//                Times.Once);
//        }
//    }
//}

public class AccountsControllerUnitTests : UnitTestsBase
{
    private readonly AccountsController _sut;
    private readonly CreateAccountRequest _request;
    private readonly Mock<ILogger<AccountsController>> _logger;

    public AccountsControllerUnitTests()
    {
        _request = _fixture.Build<CreateAccountRequest>()
            .Create();

        _logger = _fixture.Freeze<Mock<ILogger<AccountsController>>>();

        var account = _fixture.Build<Account>()
            .With(x => x.Description, _request.Description)
            .Create();

        _fixture.Freeze<Mock<IAccountRepository>>()
            .Setup(x => x.Insert(It.IsAny<Account>()))
            .ReturnsAsync(Result.Ok(account));

        _sut = _fixture.Build<AccountsController>()
            .OmitAutoProperties()
            .Create();
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Request_Valido_Then_Retorna_Created()
    {
        // Act
        var response = await _sut.Post(_request);
        // Assert
        var result = response as ObjectResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be((int)HttpStatusCode.Created);

        _logger
            .Verify(LogMessage(LogLevel.Information, "Started"),
            Times.Once);

        _fixture.Freeze<Mock<IAccountRepository>>()
            .Verify(x => x.Insert(It.IsAny<Account>())
            , Times.Once);

        _logger
            .Verify(LogMessage(LogLevel.Information, "Ended"),
            Times.Once);
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Empty_Description_Then_Retorna_BadRequest()
    {
        // Arrange
        var request = _request with { Description = string.Empty };
        // Act
        var retorno = await _sut.Post(request);
        // Assert
        var statusCodeResult = retorno as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        statusCodeResult.Value.Should().Be("Description is required");

        _logger
            .Verify(LogMessage(LogLevel.Information, "Started"),
            Times.Once);

        _fixture.Freeze<Mock<IAccountRepository>>()
            .Verify(x => x.Insert(It.IsAny<Account>())
            , Times.Never);

        _logger
            .Verify(LogMessage(LogLevel.Information, "Ended"),
            Times.Never);
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Null_Description_Then_Retorna_BadRequest()
    {
        // Arrange
        var request = _request with { Description = null };
        // Act
        var result = await _sut.Post(request);
        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        statusCodeResult.Value.Should().Be("Description is required");

        _logger
            .Verify(LogMessage(LogLevel.Information, "Started"),
            Times.Once);

        _fixture.Freeze<Mock<IAccountRepository>>()
            .Verify(x => x.Insert(It.IsAny<Account>())
            , Times.Never);

        _logger
            .Verify(LogMessage(LogLevel.Information, "Ended"),
            Times.Never);
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Empty_String_Description_Then_Retorna_BadRequest()
    {
        // Arrange
        var request = _request with { Description = "" };
        // Act
        var result = await _sut.Post(request);
        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        statusCodeResult.Value.Should().Be("Description is required");

        _logger
            .Verify(
            LogMessage(LogLevel.Information, "Started"),
            Times.Once);

        _fixture.Freeze<Mock<IAccountRepository>>()
            .Verify(x => x.Insert(It.IsAny<Account>())
            , Times.Never);

        _logger
            .Verify(
            LogMessage(LogLevel.Information, "Ended"),
            Times.Never);
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Repositorio_Exception_Thrown_Then_Retorna_InternalServerError()
    {
        // Arrange
        _fixture.Freeze<Mock<IAccountRepository>>()
            .Setup(x => x.Insert(It.IsAny<Account>()))
            .Throws(new Exception());
        // Act
        var result = await _sut.Post(_request);
        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);

        _logger
            .Verify(LogMessage(LogLevel.Information, "Started"),
            Times.Once);

        _fixture.Freeze<Mock<IAccountRepository>>()
            .Verify(x => x.Insert(It.IsAny<Account>())
            , Times.Once);

        _logger
            .Verify(LogMessage(LogLevel.Error, "Exception"),
            Times.Once);
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Repositorio_Exception_Async_Thrown_Then_Retorna_InternalServerError()
    {
        // Arrange
        _fixture.Freeze<Mock<IAccountRepository>>()
            .Setup(repo => repo.Insert(It.IsAny<Account>()))
            .ThrowsAsync(new Exception());
        // Act
        var result = await _sut.Post(_request);
        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);

        _logger
            .Verify(LogMessage(LogLevel.Information, "Started"),
            Times.Once);

        _fixture.Freeze<Mock<IAccountRepository>>()
            .Verify(x => x.Insert(It.IsAny<Account>())
            , Times.Once);

        _logger
            .Verify(LogMessage(LogLevel.Error, "Exception"),
            Times.Once);
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Insert_Account_Fails_Then_Returns_BadRequest()
    {
        // Arrange
        _fixture.Freeze<Mock<IAccountRepository>>()
            .Setup(repo => repo.Insert(It.IsAny<Account>()))
            .ReturnsAsync(Result.Fail<Account>("Mensagem"));
        // Act
        var result = await _sut.Post(_request);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        statusCodeResult.Value.Should().Be("Fail to insert account");

        _logger
            .Verify(LogMessage(LogLevel.Information, "Started"),
            Times.Once);

        _fixture.Freeze<Mock<IAccountRepository>>()
            .Verify(x => x.Insert(It.IsAny<Account>())
            , Times.Once);

        _logger
            .Verify(LogMessage(LogLevel.Information, "Ended"),
            Times.Never);
    }
}