using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text;

namespace Manager.Api.Features.Accounts;

public class AuditLog
{
    public int Id { get; set; }
    public string? UserEmail { get; set; }
    public string? EntityName { get; set; }
    public string? Action { get; set; }
    public DateTime TimeStamp { get; set; }
    public string? Changes { get; set; }
}

public class AccountContext : DbContext
{
    private readonly IHttpContextAccessor _contextAccessor;

    public AccountContext(DbContextOptions<AccountContext> options, IHttpContextAccessor contextAccessor) : base(options)
    {
        _contextAccessor = contextAccessor;
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductEntityTypeConfiguration).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>()
            .HavePrecision(10, 2);
        base.ConfigureConventions(configurationBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var modifiedEntries = ChangeTracker.Entries()
            .Where(x => x.State is EntityState.Added
            or EntityState.Modified
            or EntityState.Deleted)
            .ToList();
        foreach (var modifiedEntry in modifiedEntries)
        {
            //var user = _contextAccessor.HttpContext.User != null
            //    ? _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name)
            //    : "robot";
            var auditLog = new AuditLog
            {
                EntityName = modifiedEntry.Entity.GetType().Name,
                UserEmail = "robo",
                Action = modifiedEntry.State.ToString(),
                TimeStamp = DateTime.UtcNow,
                Changes = GetChanges(modifiedEntry),
            };
            AuditLogs.Add(auditLog);
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    private string GetChanges(EntityEntry modifiedEntry)
    {
        var changes = new StringBuilder();
        foreach (var property in modifiedEntry.OriginalValues.Properties)
        {
            var originalValue = modifiedEntry.OriginalValues[property];
            var currentValue = modifiedEntry.CurrentValues[property];
            if (!Equals(originalValue, currentValue))
            {
                changes.AppendLine($"{property.Name}: From '{originalValue}' to '{currentValue}'");
            }
        }
        return changes.ToString();
    }
}