using AutoFixture;
using FluentAssertions;
using FluentResults;
using Manager.Api.Features.Accounts;
using Manager.Tests.BaseTests;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace Manager.Tests.UnitTests;

public class AccountsControllerUnitTests : UnitTestsBase
{
    private readonly AccountsController _sut;
    private readonly CreateAccountRequest _request;

    public AccountsControllerUnitTests()
    {
        _request = _fixture.Build<CreateAccountRequest>()
            .Create();

        var account = _fixture.Build<Account>()
            .With(x => x.Id, It.IsAny<int>())
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
    public void Given_When_Then()
    {
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Request_Valido_Then_Retorna_Created()
    {
        // Act
        var response = await _sut.Post(_request);
        // Assert
        var result = response as StatusCodeResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be((int)HttpStatusCode.Created);
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
    }
}