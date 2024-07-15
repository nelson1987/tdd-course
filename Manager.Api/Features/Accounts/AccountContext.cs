using Microsoft.EntityFrameworkCore;

namespace Manager.Api.Features.Accounts;

public class AccountContext : DbContext
{
    private readonly ILoggerFactory _loggerFactory;

    public AccountContext(DbContextOptions<AccountContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new ProductEntityTypeConfiguration().Configure(modelBuilder.Entity<Account>());
    }
}