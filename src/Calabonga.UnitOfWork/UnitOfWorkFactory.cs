using Microsoft.EntityFrameworkCore;

namespace Calabonga.UnitOfWork;

/// <summary>
/// CALABONGA Warning: do not remove sealed
/// Represents the default implementation of the <see cref="T:IUnitOfWork"/> and <see cref="T:IUnitOfWork{TContext}"/> interface.
/// </summary>
/// <typeparam name="TContext">The type of the db context.</typeparam>
public sealed class UnitOfWorkFactory<TContext> : IUnitOfWorkFactory<TContext>
    where TContext : DbContext
{
    public UnitOfWorkFactory(IDbContextFactory<TContext> factory) => DbContext = factory.CreateDbContext();

    public TContext DbContext { get; }

    public IUnitOfWork CreateUnitOfWork() => new UnitOfWork<TContext>(DbContext);

    public void Dispose() => DbContext.Dispose();
}