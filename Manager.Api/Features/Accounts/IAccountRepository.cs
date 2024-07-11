using FluentResults;

namespace Manager.Api.Features.Accounts;

public interface IAccountRepository
{
    Task<Result<Account>> Insert(Account account);

    Task<Result<Account>> GetById(int accountId);
}