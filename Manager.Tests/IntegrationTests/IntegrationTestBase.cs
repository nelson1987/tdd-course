using AutoFixture;
using AutoFixture.AutoMoq;

namespace Manager.Tests.IntegrationTests;

public class IntegrationTestBase : IAsyncLifetime
{
    public readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    public readonly CancellationToken _token = CancellationToken.None;
    public readonly HttpClient _client;
    public readonly ApiFixture _server;

    //public readonly ITodoRepository todoRepository;
    public IntegrationTestBase()
    {
        _server = new ApiFixture();
        _client = _server.CreateClient();
        //todoRepository = Server.Services.GetRequiredService<ITodoRepository>();
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