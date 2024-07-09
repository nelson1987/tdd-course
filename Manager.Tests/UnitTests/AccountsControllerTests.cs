using Manager.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Manager.Tests.UnitTests;

public class AccountsControllerTests
{
    private readonly AccountsController _sut;
    private readonly Request _request;

    public AccountsControllerTests()
    {
        _sut = new AccountsController();
        _request = new Request("Conta criada com sucesso");
    }

    [Fact]
    public void Given_When_Then()
    {
    }

    [Fact]
    public void Given_Requisicao_Post_When_Request_Valido_Then_Retorna_Created()
    {
        var retorno = (StatusCodeResult)_sut.Post(_request);
        Assert.Equal((int)HttpStatusCode.Created, retorno.StatusCode);
    }

    [Fact]
    public void Given_Requisicao_Post_When_Request_Invalido_Then_Retorna_BadRequest()
    {
        var request = _request with { Description = string.Empty };
        var retorno = (ObjectResult)_sut.Post(request);
        Assert.Equal((int)HttpStatusCode.BadRequest, retorno.StatusCode);
    }
}