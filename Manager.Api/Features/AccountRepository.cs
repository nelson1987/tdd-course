namespace Manager.Api.Features;

public class AccountRepository : IAccountRepository
{
    public Task<Account> Insert(Account account)
    {
        return Task.FromResult(account);
    }
}