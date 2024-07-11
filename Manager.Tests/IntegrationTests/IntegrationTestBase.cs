using AutoFixture;
using AutoFixture.AutoMoq;
using Manager.Api.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Manager.Tests.IntegrationTests;

public class IntegrationTestBase : IAsyncLifetime
{
    public readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    public readonly CancellationToken _token = CancellationToken.None;
    public readonly HttpClient _client;
    public readonly ApiFixture _server;
    public readonly WriteDatabase todoRepository;
    public readonly ITokenService tokenService;

    public IntegrationTestBase()
    {
        _server = new ApiFixture();
        _client = _server.CreateClient();
        todoRepository = _server.Services.GetRequiredService<WriteDatabase>();
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