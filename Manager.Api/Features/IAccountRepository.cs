using FluentResults;

namespace Manager.Api.Features;

public interface IAccountRepository
{
    Task<Result<Account>> Insert(Account account);
}