using FluentResults;
using Manager.Api.Features.Accounts;

namespace Manager.Tests.InMemoryInfrastructure;

public class InMemoryAccountRepository : IAccountRepository
{
    public Task<Result<Account>> GetById(int accountId)
    {
        throw new NotImplementedException();
    }

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