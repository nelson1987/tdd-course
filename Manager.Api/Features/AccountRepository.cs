namespace Manager.Api.Features;

public class Response<T> where T : class
{
    public Response(T value)
    {
        Value = value;
    }

    public T Value { get; set; }
}

public class AccountRepository : IAccountRepository
{
    private readonly WriteDatabase _writeDatabase;

    public AccountRepository(WriteDatabase writeDatabase)
    {
        _writeDatabase = writeDatabase;
    }

    public async Task<Response<Account>> Insert(Account account)
    {
        account.Id = 1;
        _writeDatabase.Accounts.Add(account);
        return await Task.FromResult(new Response<Account>(account));
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