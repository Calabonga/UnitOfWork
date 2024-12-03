using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Calabonga.UnitOfWork;

/// <summary>
/// Represents a default generic repository implements the <see cref="IRepository{TEntity}"/> interface.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public sealed partial class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly DbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{TEntity}"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public Repository(DbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = _dbContext.Set<TEntity>();
    }

    /// <summary>
    /// Change entity state for patch method on web api.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// /// <param name="state">The entity state.</param>
    public void ChangeEntityState(TEntity entity, EntityState state) => _dbContext.Entry(entity).State = state;

    /// <summary>
    /// Changes the table name. This require the tables in the same database.
    /// </summary>
    /// <param name="table"></param>
    /// <remarks>
    /// This only been used for supporting multiple tables in the same model. This require the tables in the same database.
    /// </remarks>
    public void ChangeTable(string table)
    {
        if (_dbContext.Model.FindEntityType(typeof(TEntity)) is IConventionEntityType relational)
        {
            relational.SetTableName(table);
        }
    }

    /// <summary>
    /// Uses raw SQL queries to fetch the specified <typeparamref name="TEntity" /> data.
    /// </summary>
    /// <param name="sql">The raw SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>An <see cref="IQueryable{TEntity}" /> that contains elements that satisfy the condition specified by raw SQL.</returns>
    public IQueryable<TEntity> FromSql(string sql, params object[] parameters) => _dbSet.FromSqlRaw(sql, parameters);

    /// <summary>
    /// Finds an entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
    /// </summary>
    /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
    /// <returns>The found entity or null.</returns>
    public TEntity? Find(params object[] keyValues) => _dbSet.Find(keyValues);

    /// <summary>
    /// Finds an entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
    /// </summary>
    /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
    /// <returns>A <see cref="Task{TEntity}" /> that represents the asynchronous insert operation.</returns>
    public ValueTask<TEntity?> FindAsync(params object[] keyValues) => _dbSet.FindAsync(keyValues);

    /// <summary>
    /// Finds an entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
    /// </summary>
    /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task{TEntity}"/> that represents the asynchronous find operation. The task result contains the found entity or null.</returns>
    public ValueTask<TEntity?> FindAsync(object[] keyValues, CancellationToken cancellationToken) => _dbSet.FindAsync(keyValues, cancellationToken);

    /// <summary>
    /// Gets the count based on a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int Count(Expression<Func<TEntity, bool>>? predicate = null) =>
        predicate is null
            ? _dbSet.Count()
            : _dbSet.Count(predicate);

    /// <summary>
    /// Gets async the count based on a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns></returns>
    public async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate is null
            ? await _dbSet.CountAsync(cancellationToken)
            : await _dbSet.CountAsync(predicate, cancellationToken);

    /// <summary>
    /// Gets the long count based on a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public long LongCount(Expression<Func<TEntity, bool>>? predicate = null) =>
        predicate is null
            ? _dbSet.LongCount()
            : _dbSet.LongCount(predicate);

    /// <summary>
    /// Gets async the long count based on a predicate.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public async Task<long> LongCountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate is null
            ? await _dbSet.LongCountAsync(cancellationToken)
            : await _dbSet.LongCountAsync(predicate, cancellationToken);

    /// <summary>
    /// Gets the exists based on a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Exists(Expression<Func<TEntity, bool>>? predicate = null) =>
        predicate is null
            ? _dbSet.Any()
            : _dbSet.Any(predicate);

    /// <summary>
    /// Gets the Async Exists record based on a predicate.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    public async Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>>? selector = null,
        CancellationToken cancellationToken = default) =>
        selector is null
            ? await _dbSet.AnyAsync(cancellationToken)
            : await _dbSet.AnyAsync(selector, cancellationToken);

    /// <summary>
    /// Gets the max based on a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    ///  /// <param name="selector"></param>
    /// <returns>decimal</returns>
    public T? Max<T>(
        Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null) =>
        predicate is null
            ? _dbSet.Max(selector)
            : _dbSet.Where(predicate).Max(selector);

    /// <summary>
    /// Gets the async max based on a predicate.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="predicate"></param>
    /// <returns>decimal</returns>
    public Task<T> MaxAsync<T>(
        Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate is null
            ? _dbSet.MaxAsync(selector, cancellationToken)
            : _dbSet.Where(predicate).MaxAsync(selector, cancellationToken);

    /// <summary>
    /// Gets the min based on a predicate.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="predicate"></param>
    /// <returns>decimal</returns>
    public T? Min<T>(
        Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null) =>
        predicate is null
            ? _dbSet.Min(selector)
            : _dbSet.Where(predicate).Min(selector);

    /// <summary>
    /// Gets the async min based on a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="selector"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>decimal</returns>
    public Task<T> MinAsync<T>(
        Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate is null
            ? _dbSet.MinAsync(selector, cancellationToken)
            : _dbSet.Where(predicate).MinAsync(selector, cancellationToken);

    /// <summary>
    /// Gets the average based on a predicate.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="predicate"></param>
    /// <returns>decimal</returns>
    public decimal Average(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null) =>
        predicate is null
            ? _dbSet.Average(selector)
            : _dbSet.Where(predicate).Average(selector);

    /// <summary>
    /// Gets the async average based on a predicate.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>decimal</returns>
    public Task<decimal> AverageAsync(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate is null
            ? _dbSet.AverageAsync(selector, cancellationToken)
            : _dbSet.Where(predicate).AverageAsync(selector, cancellationToken);

    /// <summary>
    /// Gets the sum based on a predicate.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="predicate"></param>
    /// <returns>decimal</returns>
    public decimal Sum(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null) =>
        predicate is null
            ? _dbSet.Sum(selector)
            : _dbSet.Where(predicate).Sum(selector);

    /// <summary>
    /// Gets the async sum based on a predicate.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    /// ///
    /// <returns>decimal</returns>
    public Task<decimal> SumAsync(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate is null
            ? _dbSet.SumAsync(selector, cancellationToken)
            : _dbSet.Where(predicate).SumAsync(selector, cancellationToken);

    #region Insert

    /// <summary>
    /// Inserts a new entity synchronously.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    public TEntity Insert(TEntity entity) => _dbSet.Add(entity).Entity;

    /// <summary>
    /// Inserts a range of entities synchronously.
    /// </summary>
    /// <param name="entities">The entities to insert.</param>
    public void Insert(params TEntity[] entities) => _dbSet.AddRange(entities);

    /// <summary>
    /// Inserts a range of entities synchronously.
    /// </summary>
    /// <param name="entities">The entities to insert.</param>
    public void Insert(IEnumerable<TEntity> entities) => _dbSet.AddRange(entities);

    /// <summary>
    /// Inserts a new entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous insert operation.</returns>
    public ValueTask<EntityEntry<TEntity>> InsertAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        _dbSet.AddAsync(entity, cancellationToken);

    /// <summary>
    /// Inserts a range of entities asynchronously.
    /// </summary>
    /// <param name="entities">The entities to insert.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous insert operation.</returns>
    public Task InsertAsync(params TEntity[] entities) => _dbSet.AddRangeAsync(entities);

    /// <summary>
    /// Inserts a range of entities asynchronously.
    /// </summary>
    /// <param name="entities">The entities to insert.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous insert operation.</returns>
    public Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => _dbSet.AddRangeAsync(entities, cancellationToken);

    #endregion

    #region Update

    /// <summary>
    /// Updates the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    public void Update(TEntity entity) => _dbSet.Update(entity);

    /// <summary>
    /// Updates the specified entities.
    /// </summary>
    /// <param name="entities">The entities.</param>
    public void Update(params TEntity[] entities) => _dbSet.UpdateRange(entities);

    /// <summary>
    /// Updates the specified entities.
    /// </summary>
    /// <param name="entities">The entities.</param>
    public void Update(IEnumerable<TEntity> entities) => _dbSet.UpdateRange(entities);

    /// <summary>
    ///     Updates all database rows for the entity instances which match the LINQ query from the database.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This operation executes immediately against the database, rather than being deferred until
    ///         <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges" /> is called. It also does not interact with the EF change tracker in any way:
    ///         entity instances which happen to be tracked when this operation is invoked aren't taken into account, and aren't updated
    ///         to reflect the changes.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-bulk-operations">Executing bulk operations with EF Core</see>
    ///         for more information and examples.
    ///     </para>
    /// </remarks>
    /// <param name="predicate">Predicate</param>
    /// <returns>The total number of rows updated in the database.</returns>
    public int ExecuteUpdate(Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> predicate)
        => _dbSet.ExecuteUpdate(predicate);

    /// <summary>
    ///     Asynchronously updates database rows for the entity instances which match the LINQ query from the database.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This operation executes immediately against the database, rather than being deferred until
    ///         <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges" /> is called. It also does not interact with the EF change tracker in any way:
    ///         entity instances which happen to be tracked when this operation is invoked aren't taken into account, and aren't updated
    ///         to reflect the changes.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-bulk-operations">Executing bulk operations with EF Core</see>
    ///         for more information and examples.
    ///     </para>
    /// </remarks>
    /// <param name="predicate">Predicate</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>The total number of rows updated in the database.</returns>
    public Task<int> ExecuteUpdateAsync(
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> predicate,
        CancellationToken cancellationToken)
        => _dbSet.ExecuteUpdateAsync(predicate, cancellationToken);

    #endregion

    #region Delete


    /// <summary>
    /// Deletes the specified entities.
    /// </summary>
    /// <param name="entities">The entities.</param>
    public void Delete(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);

    /// <summary>
    /// Deletes the specified entities.
    /// </summary>
    /// <param name="entities">The entities.</param>
    public void Delete(params TEntity[] entities) => _dbSet.RemoveRange(entities);

    /// <summary>
    /// Deletes the specified entity.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    public void Delete(TEntity entity) => _dbSet.Remove(entity);

    /// <summary>
    /// Deletes the entity by the specified primary key.
    /// </summary>
    /// <param name="id">The primary key value.</param>
    public void Delete(object id)
    {
        // using a stub entity to mark for deletion
        var typeInfo = typeof(TEntity).GetTypeInfo();
        var key = _dbContext.Model.FindEntityType(typeInfo)?.FindPrimaryKey()?.Properties.FirstOrDefault();
        if (key is null)
        {
            return;
        }

        var property = typeInfo.GetProperty(key.Name);
        if (property != null)
        {
            var entity = Activator.CreateInstance<TEntity>();
            property.SetValue(entity, id);
            _dbContext.Entry(entity).State = EntityState.Deleted;
        }
        else
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                Delete(entity);
            }
        }
    }

    /// <summary>
    ///     Deletes all database rows for the entity instances which match the LINQ query from the database.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This operation executes immediately against the database, rather than being deferred until
    ///         <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges" /> is called. It also does not interact with the EF change tracker in any way:
    ///         entity instances which happen to be tracked when this operation is invoked aren't taken into account, and aren't updated
    ///         to reflect the changes.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-bulk-operations">Executing bulk operations with EF Core</see>
    ///         for more information and examples.
    ///     </para>
    /// </remarks>
    /// <returns>The total number of rows deleted in the database.</returns>
    public int ExecuteDelete() => _dbSet.ExecuteDelete();

    /// <summary>
    ///     Asynchronously deletes database rows for the entity instances which match the LINQ query from the database.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This operation executes immediately against the database, rather than being deferred until
    ///         <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges" /> is called. It also does not interact with the EF change tracker in any way:
    ///         entity instances which happen to be tracked when this operation is invoked aren't taken into account, and aren't updated
    ///         to reflect the changes.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-bulk-operations">Executing bulk operations with EF Core</see>
    ///         for more information and examples.
    ///     </para>
    /// </remarks>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>The total number of rows deleted in the database.</returns>
    public Task<int> ExecuteDeleteAsync(CancellationToken cancellationToken = default) => _dbSet.ExecuteDeleteAsync(cancellationToken);

    #endregion
}