using Manager.Api.Features;

namespace Manager.Tests.IntegrationTests;

public class InMemoryAccountRepository : IAccountRepository
{
    public Task<Account> Insert(Account account)
    {
        throw new Exception();
    }
}