using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Manager.Api.Features.Accounts;

public class AccountRepository : IAccountRepository
{
    private readonly AccountContext _writeDatabase;

    public AccountRepository(AccountContext writeDatabase)
    {
        _writeDatabase = writeDatabase;
    }

    public async Task<Result<Account>> GetById(int accountId)
    {
        var account = await _writeDatabase.Accounts.FirstOrDefaultAsync(x => x.Id == accountId);
        return Result.Ok(account);
    }

    public async Task<Result<Account>> Insert(Account account)
    {
        try
        {
            await _writeDatabase.Accounts.AddAsync(account);
            await _writeDatabase.SaveChangesAsync();
            return Result.Ok(account);
        }
        catch (ArgumentException ex) when (ex.Message.Contains("An item with the same key has already been added."))
        {
            return Result.Fail("Error on insert account in base of duplicate key");
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}