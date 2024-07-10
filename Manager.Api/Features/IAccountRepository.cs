namespace Manager.Api.Features;

public interface IAccountRepository
{
    Task<Account> Insert(Account account);
}