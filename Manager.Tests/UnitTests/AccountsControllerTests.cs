using Manager.Api.Controllers;

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
    }

    [Fact]
    public void Dado_Requisicao_Get_When_Request_Invalido_Then_Retorna_BadRequest()
    {
    }
}