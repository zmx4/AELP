using Microsoft.EntityFrameworkCore;

namespace AELP.UnitTest.Helper;

public sealed class TestDbContextFactory<TContext>(DbContextOptions<TContext> options) : IDbContextFactory<TContext>
    where TContext : DbContext
{
    public TContext CreateDbContext()
    {
        return (TContext)Activator.CreateInstance(typeof(TContext), options)!;
    }

    public Task<TContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(CreateDbContext());
    }
}
