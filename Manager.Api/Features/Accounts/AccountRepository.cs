using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq.Expressions;

namespace Manager.Api.Features.Accounts;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Filter { get; }
    Expression<Func<T, object>> OrderBy { get; }
    Expression<Func<T, object>> OrderByDescending { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    Expression<Func<T, object>> GroupBy { get; }
}

public class Specification<TEntity> : ISpecification<TEntity> where TEntity : class
{
    public Expression<Func<TEntity, bool>> Filter { get; }
    public Expression<Func<TEntity, object>> OrderBy { get; set; } = null!;
    public Expression<Func<TEntity, object>> OrderByDescending { get; set; } = null!;
    public Expression<Func<TEntity, object>> GroupBy { get; set; } = null!;
    public List<Expression<Func<TEntity, object>>> Includes { get; } = null!;

    public Specification(Expression<Func<TEntity, bool>> filter)
    {
        Filter = filter;
    }
}

public static class SpecificationBuilder<TEntity> where TEntity : class
{
    public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery,
        ISpecification<TEntity> specification)
    {
        var query = inputQuery;
        if (specification == null)
        {
            return query;
        }
        if (specification.Filter != null)
        {
            query = query.Where(specification.Filter);
        }
        if (specification.Includes != null
        && specification.Includes.Any())
        {
            foreach (var include in specification.Includes)
            {
                query = query.Include(include);
            }
        }
        if (specification.OrderBy != null)
        {
            query = query
            .OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query
            .OrderByDescending(specification.OrderByDescending);
        }
        if (specification.GroupBy != null)
        {
            query = query
            .GroupBy(specification.GroupBy)
            .SelectMany(x => x);
        }
        return query;
    }
}

public abstract class BaseSpecificationService<TEntity> where TEntity : class
{
    private readonly AccountContext _context;

    protected BaseSpecificationService(AccountContext context)
    {
        _context = context;
    }

    protected ISpecification<TEntity> Specification { get; set; } = null!;

    protected IQueryable<TEntity> GetQuery()
    {
        return SpecificationBuilder<TEntity>
        .GetQuery(_context.Set<TEntity>().AsQueryable(),
        Specification);
    }
}

public class GetProductsLessThanFiveDollars : BaseSpecificationService<Account>
{
    public GetProductsLessThanFiveDollars(AccountContext context, int accountId)
   : base(context)
    {
        Specification = new Specification<Account>(product => product.Id == accountId);
    }
}

public static class AttractionExtensions
{
    public static async Task<List<Account>> GetAttractions(this IAccountContext context, CancellationToken cancellation)
    {
        return await context.Accounts
            .ToListAsync(cancellation)!;
    }

    public static async Task<Account?> GetAttraction(this IAccountContext context, int id, CancellationToken cancellation)
    {
        return await context.Accounts
        //.Include(t => t.Location)
        .FirstOrDefaultAsync(e => e!.Id == id, cancellation)!;
    }
}

public interface IAccountContext
{
    DbSet<Account> Accounts { get; set; }

    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    DatabaseFacade Database { get; }
}

public class AccountRepository : IAccountRepository
{
    private readonly AccountContext _context;

    public AccountRepository(AccountContext writeDatabase)
    {
        _context = writeDatabase;
    }

    public async Task<Result<Account>> GetById(int accountId)
    {
        return await GetAccount(accountId);
    }

    private async Task<Result<Account>> GetAccount(int accountId)
    {
        //var productsBelowFiveDollarsSpecification = new GetProductsLessThanFiveDollars(_writeDatabase, accountId);
        //var results = productsBelowFiveDollarsSpecification.GetQuery().ToList();
        var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == accountId);
        if (account == null) return Result.Fail("User not found");
        return Result.Ok(account);
    }

    public async Task<Result<Account>> Insert(Account account)
    {
        try
        {
            if (_context.Accounts.Any(x => x.Description == account.Description))
                return Result.Fail("This account already exists");

            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
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