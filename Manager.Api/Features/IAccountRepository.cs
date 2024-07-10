namespace Manager.Api.Features;

public interface IAccountRepository
{
    Task<Response<Account>> Insert(Account account);
}