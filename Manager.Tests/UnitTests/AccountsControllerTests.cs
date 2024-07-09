using AutoFixture;
using AutoFixture.AutoMoq;
using Manager.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace Manager.Tests.UnitTests;

public class AccountsControllerTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly AccountsController _sut;
    private readonly CreateAccountRequest _request;

    public AccountsControllerTests()
    {
        _fixture.Freeze<Mock<IAccountRepository>>()
            .Setup(x => x.Insert(It.IsAny<Account>()))
            .ReturnsAsync(new Account() { });
        _request = _fixture.Build<CreateAccountRequest>()
            .With(x => x.Description, "Conta criada com sucesso")
            .Create();
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
        var retorno = (StatusCodeResult)await _sut.Post(_request);
        Assert.Equal((int)HttpStatusCode.Created, retorno.StatusCode);
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Request_Invalido_Then_Retorna_BadRequest()
    {
        var request = _request with { Description = string.Empty };
        var retorno = (ObjectResult)await _sut.Post(request);
        Assert.Equal((int)HttpStatusCode.BadRequest, retorno.StatusCode);
    }

    [Fact]
    public async Task Given_Requisicao_Post_When_Repositorio_Exception_Then_Retorna_BadRequest()
    {
        _fixture.Freeze<Mock<IAccountRepository>>()
            .Setup(x => x.Insert(It.IsAny<Account>()))
            .Throws(new Exception("Exception"));

        var retorno = (ObjectResult)await _sut.Post(_request);
        Assert.Equal((int)HttpStatusCode.BadRequest, retorno.StatusCode);
    }
}