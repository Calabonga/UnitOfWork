using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calabonga.UnitOfWork;

/// <summary>
/// CALABONGA Warning: do not remove sealed
/// Represents the default implementation of the <see cref="T:IUnitOfWork"/> and <see cref="T:IUnitOfWork{TContext}"/> interface.
/// </summary>
/// <typeparam name="TContext">The type of the db context.</typeparam>
public sealed class UnitOfWork<TContext> : IRepositoryFactory, IUnitOfWork<TContext>
    where TContext : DbContext
{
    #region fields

    private bool _disposed;
    private Dictionary<Type, object>? _repositories;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork{TContext}"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public UnitOfWork(TContext context)
    {
        DbContext = context ?? throw new ArgumentNullException(nameof(context));
        LastSaveChangesResult = new SaveChangesResult();
    }

    #region properties

    /// <summary>
    /// Gets the db context.
    /// </summary>
    /// <returns>The instance of type <typeparamref name="TContext"/>.</returns>
    public TContext DbContext { get; }

    #endregion

    #region Methods

    /// <summary>
    /// Returns Transaction 
    /// </summary>
    /// <returns></returns>
    public Task<IDbContextTransaction> BeginTransactionAsync(bool useIfExists = false)
    {
        var transaction = DbContext.Database.CurrentTransaction;
        if (transaction == null)
        {
            return DbContext.Database.BeginTransactionAsync();
        }

        return useIfExists ? Task.FromResult(transaction) : DbContext.Database.BeginTransactionAsync();
    }

    /// <summary>
    /// Returns Transaction 
    /// </summary>
    /// <returns></returns>
    public IDbContextTransaction BeginTransaction(bool useIfExists = false)
    {
        var transaction = DbContext.Database.CurrentTransaction;
        if (transaction == null)
        {
            return DbContext.Database.BeginTransaction();
        }

        return useIfExists ? transaction : DbContext.Database.BeginTransaction();
    }

    /// <summary>
    /// DbContext disable/enable auto detect changes
    /// </summary>
    /// <param name="value"></param>
    public void SetAutoDetectChanges(bool value) => DbContext.ChangeTracker.AutoDetectChangesEnabled = value;

    public SaveChangesResult LastSaveChangesResult { get; }


    #endregion

    /// <summary>
    /// Gets the specified repository for the <typeparamref name="TEntity"/>.
    /// </summary>
    /// <param name="hasCustomRepository"><c>True</c> if providing custom repository</param>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <returns>An instance of type inherited from <see cref="IRepository{TEntity}"/> interface.</returns>
    public IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = false) where TEntity : class
    {
        _repositories ??= new Dictionary<Type, object>();

        // what's the best way to support custom repository?
        if (hasCustomRepository)
        {
            var customRepo = DbContext.GetService<IRepository<TEntity>>();
            if (customRepo != null)
            {
                return customRepo;
            }
        }

        var type = typeof(TEntity);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new Repository<TEntity>(DbContext);
        }

        return (IRepository<TEntity>)_repositories[type];
    }

    /// <summary>
    /// Executes the specified raw SQL command.
    /// </summary>
    /// <param name="sql">The raw SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The number of state entities written to database.</returns>
    public int ExecuteSqlCommand(string sql, params object[] parameters) => DbContext.Database.ExecuteSqlRaw(sql, parameters);

    /// <summary>
    /// Executes the specified raw SQL command.
    /// </summary>
    /// <param name="sql">The raw SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The number of state entities written to database.</returns>
    public Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters) => DbContext.Database.ExecuteSqlRawAsync(sql, parameters);

    /// <summary>
    /// Uses raw SQL queries to fetch the specified <typeparamref name="TEntity" /> data.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="sql">The raw SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>An <see cref="IQueryable{T}" /> that contains elements that satisfy the condition specified by raw SQL.</returns>
    public IQueryable<TEntity> FromSqlRaw<TEntity>(string sql, params object[] parameters) where TEntity : class => DbContext.Set<TEntity>().FromSqlRaw(sql, parameters);

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    public int SaveChanges()
    {
        try
        {
            return DbContext.SaveChanges();
        }
        catch (Exception exception)
        {
            LastSaveChangesResult.Exception = exception;
            return 0;
        }
    }

    /// <summary>
    /// Asynchronously saves all changes made in this unit of work to the database.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous save operation. The task result contains the number of state entities written to database.</returns>
    public async Task<int> SaveChangesAsync()
    {
        try
        {
            return await DbContext.SaveChangesAsync();
        }
        catch (Exception exception)
        {
            LastSaveChangesResult.Exception = exception;
            return 0;
        }
    }

    /// <summary>
    /// Saves all changes made in this context to the database with distributed transaction.
    /// </summary>
    /// <param name="unitOfWorks">An optional <see cref="T:IUnitOfWork"/> array.</param>
    /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous save operation. The task result contains the number of state entities written to database.</returns>
    public async Task<int> SaveChangesAsync(params IUnitOfWork[] unitOfWorks)
    {
        var count = 0;
        foreach (var unitOfWork in unitOfWorks)
        {
            count += await unitOfWork.SaveChangesAsync();
        }

        count += await SaveChangesAsync();
        return count;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        // ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name="disposing">The disposing.</param>
    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _repositories?.Clear();
                DbContext.Dispose();
            }
        }
        _disposed = true;
    }

    /// <summary>
    /// Uses Track Graph Api to attach disconnected entities
    /// </summary>
    /// <param name="rootEntity"> Root entity</param>
    /// <param name="callback">Delegate to convert Object's State properties to Entities entry state.</param>
    public void TrackGraph(object rootEntity, Action<EntityEntryGraphNode> callback) => DbContext.ChangeTracker.TrackGraph(rootEntity, callback);
}