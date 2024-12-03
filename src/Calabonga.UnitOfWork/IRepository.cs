using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Calabonga.UnitOfWork;

/// <summary>
/// Defines the interfaces for generic repository.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public partial interface IRepository<TEntity> where TEntity : class
{


    #region Find

    /// <summary>
    /// Finds an entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
    /// </summary>
    /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
    /// <returns>The found entity or null.</returns>
    TEntity? Find(params object[] keyValues);

    /// <summary>
    /// Finds an entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
    /// </summary>
    /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
    /// <returns>A <see cref="Task{TEntity}"/> that represents the asynchronous find operation. The task result contains the found entity or null.</returns>
    ValueTask<TEntity?> FindAsync(params object[] keyValues);

    /// <summary>
    /// Finds an entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
    /// </summary>
    /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task{TEntity}"/> that represents the asynchronous find operation. The task result contains the found entity or null.</returns>
    ValueTask<TEntity?> FindAsync(object[] keyValues, CancellationToken cancellationToken);

    #endregion

    #region Insert

    /// <summary>
    /// Inserts a new entity synchronously.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    TEntity Insert(TEntity entity);

    /// <summary>
    /// Inserts a range of entities synchronously.
    /// </summary>
    /// <param name="entities">The entities to insert.</param>
    void Insert(params TEntity[] entities);

    /// <summary>
    /// Inserts a range of entities synchronously.
    /// </summary>
    /// <param name="entities">The entities to insert.</param>
    void Insert(IEnumerable<TEntity> entities);

    /// <summary>
    /// Inserts a new entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous insert operation.</returns>
    ValueTask<EntityEntry<TEntity>> InsertAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

    /// <summary>
    /// Inserts a range of entities asynchronously.
    /// </summary>
    /// <param name="entities">The entities to insert.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous insert operation.</returns>
    Task InsertAsync(params TEntity[] entities);

    /// <summary>
    /// Inserts a range of entities asynchronously.
    /// </summary>
    /// <param name="entities">The entities to insert.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous insert operation.</returns>
    Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));

    #endregion

    #region Update

    /// <summary>
    /// Updates the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Updates the specified entities.
    /// </summary>
    /// <param name="entities">The entities.</param>
    void Update(params TEntity[] entities);

    /// <summary>
    /// Updates the specified entities.
    /// </summary>
    /// <param name="entities">The entities.</param>
    void Update(IEnumerable<TEntity> entities);

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
    int ExecuteUpdate(Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> predicate);

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
        CancellationToken cancellationToken);

    #endregion

    #region Delete

    /// <summary>
    /// Deletes the entity by the specified primary key.
    /// </summary>
    /// <param name="id">The primary key value.</param>
    void Delete(object id);

    /// <summary>
    /// Deletes the specified entity.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    void Delete(TEntity entity);

    /// <summary>
    /// Deletes the specified entities.
    /// </summary>
    /// <param name="entities">The entities.</param>
    void Delete(params TEntity[] entities);

    /// <summary>
    /// Deletes the specified entities.
    /// </summary>
    /// <param name="entities">The entities.</param>
    void Delete(IEnumerable<TEntity> entities);

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
    int ExecuteDelete();

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
    Task<int> ExecuteDeleteAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Count

    /// <summary>
    /// Gets the count based on a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int Count(Expression<Func<TEntity, bool>>? predicate = null);

    /// <summary>
    /// Gets async the count based on a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the long count based on a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    long LongCount(Expression<Func<TEntity, bool>>? predicate = null);

    /// <summary>
    /// Gets async the long count based on a predicate.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    Task<long> LongCountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

    #endregion

    #region Exists

    /// <summary>
    /// Gets the Exists record based on a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Exists(Expression<Func<TEntity, bool>>? predicate = null);

    /// <summary>
    /// Gets the Async Exists record based on a predicate.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? selector = null, CancellationToken cancellationToken = default);

    #endregion

    #region Aggregations

    /// <summary>
    /// Gets the max based on a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    ///  /// <param name="selector"></param>
    /// <returns>decimal</returns>
    T? Max<T>(Expression<Func<TEntity, T>> selector, Expression<Func<TEntity, bool>>? predicate = null);

    /// <summary>
    /// Gets the async max based on a predicate.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="predicate"></param>
    /// <returns>decimal</returns>
    Task<T> MaxAsync<T>(
        Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the min based on a predicate.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="predicate"></param>
    /// <returns>decimal</returns>
    T? Min<T>(Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null);

    /// <summary>
    /// Gets the async min based on a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="selector"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>decimal</returns>
    Task<T> MinAsync<T>(
        Expression<Func<TEntity, T>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the average based on a predicate.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="predicate"></param>
    /// <returns>decimal</returns>
    decimal Average(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null);

    /// <summary>
    /// Gets the async average based on a predicate.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>decimal</returns>
    Task<decimal> AverageAsync(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the sum based on a predicate.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="predicate"></param>
    /// <returns>decimal</returns>
    decimal Sum(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>>? predicate = null);

    /// <summary>
    /// Gets the async sum based on a predicate.
    /// </summary>
    /// <param name="selector"></param>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    /// ///
    /// <returns>decimal</returns>
    Task<decimal> SumAsync(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Other

    /// <summary>
    /// Change entity state for patch method on web api.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// /// <param name="state">The entity state.</param>
    void ChangeEntityState(TEntity entity, EntityState state);

    /// <summary>
    /// Changes the table name. This require the tables in the same database.
    /// </summary>
    /// <param name="table"></param>
    /// <remarks>
    /// This only been used for supporting multiple tables in the same model. This require the tables in the same database.
    /// </remarks>
    void ChangeTable(string table);


    /// <summary>
    /// Uses raw SQL queries to fetch the specified <typeparamref name="TEntity" /> data.
    /// </summary>
    /// <param name="sql">The raw SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>An <see cref="IQueryable{TEntity}" /> that contains elements that satisfy the condition specified by raw SQL.</returns>
    IQueryable<TEntity> FromSql(string sql, params object[] parameters);

    #endregion
}