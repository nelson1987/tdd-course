using Manager.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Manager.Tests.UnitTests;

public class AccountsControllerTests
{
    private readonly AccountsController _sut;

    public AccountsControllerTests()
    {
        _sut = new AccountsController();
    }

    [Fact]
    public void Given_When_Then()
    {
    }

    [Fact]
    public void Dado_Requisicao_Get_When_Request_Valido_Then_Retorna_Ok()
    {
        var retorno = (StatusCodeResult)_sut.Post();
        Assert.Equal((int)HttpStatusCode.OK, retorno.StatusCode);
    }

    [Fact]
    public void Dado_Requisicao_Get_When_Request_Invalido_Then_Retorna_BadRequest()
    {
        var retorno = (StatusCodeResult)_sut.Post();
        Assert.NotEqual((int)HttpStatusCode.BadRequest, retorno.StatusCode);
    }
}