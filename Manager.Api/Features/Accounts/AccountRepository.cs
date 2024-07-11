using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Manager.Api.Features.Accounts;

public class AccountRepository : IAccountRepository
{
    private readonly ProductContext _writeDatabase;

    public AccountRepository(ProductContext writeDatabase)
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
        //account.Id = 1;
        await _writeDatabase.Accounts.AddAsync(account);
        await _writeDatabase.SaveChangesAsync();
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

public class ProductContext : DbContext
{
    public ProductContext(DbContextOptions<ProductContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=BancoDB;Trusted_Connection=True;");
    }
}