using FluentAssertions;
using Manager.Api.Features.Accounts;
using Manager.Tests.BaseTests;

namespace Manager.Tests.IntegrationTests;

public class AccountRepositoryIntegrationTests : IntegrationTestBase
{
    public AccountRepositoryIntegrationTests() : base()
    {
    }

    [Fact]
    public async Task Given_Account_Sem_Description_Retorna_Erro_Configuracao()
    {
        // Arrange
        var product = new Account();

        // Act
        var inserido = await _accountRepository.Insert(product);

        // Assert
        inserido.Should().NotBeNull();
        inserido.IsFailed.Should().BeTrue();
        inserido.Reasons[0].Message.Should().Contain("Required properties '{'Description'}' are missing");
    }

    [Fact]
    public async Task Given_Account_Correto_Retorna_Ok()
    {
        // Arrange
        var product = new Account() { Description = "Description" };

        // Act
        var inserido = await _accountRepository.Insert(product);

        // Assert
        inserido.Should().NotBeNull();
        inserido.IsSuccess.Should().BeTrue();
        inserido.Value.Should().NotBeNull();
        inserido.Value.Id.Should().Be(product.Id);
        inserido.Value.Description.Should().Be(product.Description);
    }

    [Fact]
    public async Task Given_Account_Com_Mesmo_id_Inserida_Varias_Vezes_Retorna_Erro_Duplicidade()
    {
        // Arrange
        var product = new Account() { Description = "Description" };

        // Act
        var antes = await _accountRepository.Insert(product);
        var depois = await _accountRepository.Insert(product);

        // Assert
        antes.IsSuccess.Should().BeTrue();
        antes.Value.Id.Should().Be(product.Id);
        depois.IsFailed.Should().BeTrue();
        depois.Reasons[0].Message.Should().Contain("Error on insert account in base of duplicate key");
    }

    [Fact]
    public async Task Given_Account__ComId_Diferente_Inserida_Varias_Vezes_Retorna_Sucesso()
    {
        // Arrange
        var product = new Account() { Description = "Description" };
        var product2 = new Account() { Description = "Description" };

        // Act
        var antes = await _accountRepository.Insert(product);
        var depois = await _accountRepository.Insert(product2);

        // Assert
        antes.Should().NotBeNull();
        antes.IsSuccess.Should().BeTrue();
        antes.Value.Id.Should().Be(product.Id);
        antes.Value.Description.Should().Be(product.Description);

        depois.Should().NotBeNull();
        depois.IsSuccess.Should().BeTrue();
        depois.Value.Id.Should().Be(product2.Id);
        depois.Value.Description.Should().Be(product2.Description);
    }

    [Fact]
    public async Task Given_Insert_Novo_Account_When_GetById_Novo_Account_Then_Retorna_Novo_Account_2()
    {
        // Arrange
        var product = new Account() { Description = "Description" };
        await _accountRepository.Insert(product);

        // Act
        var retrievedProduct = await _accountRepository.GetById(product.Id);

        // Assert
        retrievedProduct.Should().NotBeNull();
        retrievedProduct.Value.Should().NotBeNull();
        retrievedProduct.Value.Id.Should().Be(product.Id);
        retrievedProduct.Value.Description.Should().Be(product.Description);
    }
}
