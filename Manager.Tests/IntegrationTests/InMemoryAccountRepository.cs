using FluentResults;
using Manager.Api.Features;

namespace Manager.Tests.IntegrationTests;

public class InMemoryAccountRepository : IAccountRepository
{
    public async Task<Result<Account>> Insert(Account account)
    {
        try
        {
            throw new Exception();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}