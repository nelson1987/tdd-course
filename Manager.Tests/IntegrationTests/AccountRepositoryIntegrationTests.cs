using AutoFixture;
using FluentAssertions;
using Manager.Api.Features.Accounts;
using Manager.Tests.BaseTests;

namespace Manager.Tests.IntegrationTests;

public class AccountRepositoryIntegrationTests : IntegrationTestBase
{
    private readonly Account _entity;

    public AccountRepositoryIntegrationTests() : base()
    {
        _entity = _fixture.Build<Account>()
            .Without(x => x.Id)
            .Create();
    }

    [Fact]
    public async Task Given_Account_Correto_Retorna_Ok()
    {
        // Act
        var inserido = await _accountRepository.Insert(_entity);

        // Assert
        inserido.Should().NotBeNull();
        inserido.IsSuccess.Should().BeTrue();
        inserido.Value.Should().NotBeNull();
        inserido.Value.Id.Should().Be(_entity.Id);
        inserido.Value.Description.Should().Be(_entity.Description);
    }

    [Fact]
    public async Task Given_Account_Sem_Description_Retorna_Erro_Configuracao()
    {
        // Arrange
        var entity = new Account();

        // Act
        var inserido = await _accountRepository.Insert(entity);

        // Assert
        inserido.Should().NotBeNull();
        inserido.IsFailed.Should().BeTrue();
        inserido.Reasons[0].Message.Should().Contain("Required properties '{'Description'}' are missing");
    }

    [Fact]
    public async Task Given_Account_Com_Mesmo_id_Inserida_Varias_Vezes_Retorna_Erro_Duplicidade()
    {
        // Act
        var antes = await _accountRepository.Insert(_entity);
        var depois = await _accountRepository.Insert(_entity);

        // Assert
        antes.IsSuccess.Should().BeTrue();
        antes.Value.Id.Should().Be(_entity.Id);
        depois.IsFailed.Should().BeTrue();
        depois.Reasons[0].Message.Should().Contain("Error on insert account in base of duplicate key");
    }

    [Fact]
    public async Task Given_Account__ComId_Diferente_Inserida_Varias_Vezes_Retorna_Sucesso()
    {
        // Arrange
        var retrievedEntity = new Account() { Description = _entity.Description };

        // Act
        var antes = await _accountRepository.Insert(_entity);
        var depois = await _accountRepository.Insert(retrievedEntity);

        // Assert
        antes.Should().NotBeNull();
        antes.IsSuccess.Should().BeTrue();
        antes.Value.Id.Should().Be(_entity.Id);
        antes.Value.Description.Should().Be(_entity.Description);

        depois.Should().NotBeNull();
        depois.IsSuccess.Should().BeTrue();
        depois.Value.Id.Should().Be(retrievedEntity.Id);
        depois.Value.Description.Should().Be(retrievedEntity.Description);
    }

    [Fact]
    public async Task Given_Insert_Novo_Account_When_GetById_Novo_Account_Then_Retorna_Novo_Account_2()
    {
        // Arrange
        await _accountRepository.Insert(_entity);

        // Act
        var encontradaAccount = await _accountRepository.GetById(_entity.Id);

        // Assert
        encontradaAccount.Should().NotBeNull();
        encontradaAccount.Value.Should().NotBeNull();
        encontradaAccount.Value.Id.Should().Be(_entity.Id);
        encontradaAccount.Value.Description.Should().Be(_entity.Description);
    }
}