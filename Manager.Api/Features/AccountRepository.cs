namespace Manager.Api.Features;

public class AccountRepository : IAccountRepository
{
    private readonly WriteDatabase _writeDatabase;

    public AccountRepository(WriteDatabase writeDatabase)
    {
        _writeDatabase = writeDatabase;
    }

    public Task<Account> Insert(Account account)
    {
        account.Id = 1;
        _writeDatabase.Accounts.Add(account);
        return Task.FromResult(account);
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