namespace Manager.Tests.IntegrationTests;

public class IntegrationTestBase : IAsyncLifetime
{
    public readonly CancellationToken token = CancellationToken.None;
    public readonly HttpClient _client;
    public readonly ApiFixture Server;

    //public readonly ITodoRepository todoRepository;
    public IntegrationTestBase()
    {
        Server = new ApiFixture();
        _client = Server.CreateClient();
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
