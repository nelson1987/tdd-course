using FluentResults;

namespace Manager.Api.Features.Accounts;

public class AccountRepository : IAccountRepository
{
    private readonly WriteDatabase _writeDatabase;

    public AccountRepository(WriteDatabase writeDatabase)
    {
        _writeDatabase = writeDatabase;
    }

    public async Task<Result<Account>> Insert(Account account)
    {
        account.Id = 1;
        _writeDatabase.Accounts.Add(account);
        return Result.Ok(account);
    }
}

public class WriteDatabase //: IWriteDatabase
{
    public List<Account> Accounts { get; set; }

    public WriteDatabase()
    {
        Accounts = new List<Account>();
    }
}