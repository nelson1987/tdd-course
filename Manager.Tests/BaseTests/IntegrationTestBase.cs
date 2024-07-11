using AutoFixture;
using AutoFixture.AutoMoq;
using Manager.Api.Features.Accounts;
using Microsoft.Extensions.DependencyInjection;

namespace Manager.Tests.BaseTests;

public class IntegrationTestBase : IAsyncLifetime
{
    public readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    public readonly CancellationToken _token = CancellationToken.None;
    public readonly HttpClient _client;
    public readonly ApiFixture _server;
    public readonly IAccountRepository _accountRepository;

    public IntegrationTestBase()
    {
        _server = new ApiFixture();
        _client = _server.CreateClient();
        _accountRepository = _server.Services.GetRequiredService<IAccountRepository>();
    }

    public async Task InitializeAsync()
    {
        //await todoRepository.DeleteAll(token);
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        //await todoRepository.DeleteAll(token);
        await Task.CompletedTask;
    }
}