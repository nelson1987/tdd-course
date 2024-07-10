using AutoFixture;
using AutoFixture.AutoMoq;
using Manager.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace Manager.Tests.UnitTests;

public class AccountsControllerUnitTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly AccountsController _sut;
    private readonly CreateAccountRequest _request;

    public AccountsControllerUnitTests()
    {
        _request = _fixture.Build<CreateAccountRequest>()
            .With(x => x.Description, "Conta criada com sucesso")
            .Create();

        var account = _fixture.Build<Account>()
            .With(x => x.Id, It.IsAny<int>())
            .With(x => x.Description, _request.Description)
            .Create();
        _fixture.Freeze<Mock<IAccountRepository>>()
            .Setup(x => x.Insert(It.IsAny<Account>()))
            .ReturnsAsync(account);

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
        var retorno = await _sut.Post(_request);
        // Assert
        var statusCodeResult = retorno as StatusCodeResult;
        Assert.NotNull(statusCodeResult);
        Assert.Equal((int)HttpStatusCode.Created, statusCodeResult.StatusCode);
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
        Assert.NotNull(statusCodeResult);
        Assert.Equal((int)HttpStatusCode.BadRequest, statusCodeResult.StatusCode);
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
        Assert.NotNull(statusCodeResult);
        Assert.Equal((int)HttpStatusCode.BadRequest, statusCodeResult.StatusCode);
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
        Assert.NotNull(statusCodeResult);
        Assert.Equal((int)HttpStatusCode.BadRequest, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Repositorio_Exception_Thrown_Then_Retorna_BadRequest()
    {
        // Arrange
        _fixture.Freeze<Mock<IAccountRepository>>()
            .Setup(x => x.Insert(It.IsAny<Account>()))
            .Throws(new Exception("Exception"));
        // Act
        var result = await _sut.Post(_request);
        // Assert
        var statusCodeResult = result as ObjectResult;
        Assert.NotNull(statusCodeResult);
        Assert.Equal((int)HttpStatusCode.BadRequest, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Repositorio_Exception_Async_Thrown_Then_Retorna_BadRequest()
    {
        // Arrange
        _fixture.Freeze<Mock<IAccountRepository>>()
            .Setup(repo => repo.Insert(It.IsAny<Account>()))
            .ThrowsAsync(new Exception());
        // Act
        var result = await _sut.Post(_request);
        // Assert
        var statusCodeResult = result as ObjectResult;
        Assert.NotNull(statusCodeResult);
        Assert.Equal((int)HttpStatusCode.BadRequest, statusCodeResult.StatusCode);
    }
}