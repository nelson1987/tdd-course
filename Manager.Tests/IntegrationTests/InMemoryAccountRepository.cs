using Manager.Api.Features;

namespace Manager.Tests.IntegrationTests;

public class InMemoryAccountRepository : IAccountRepository
{
    public Task<Response<Account>> Insert(Account account)
    {
        throw new Exception();
    }
}