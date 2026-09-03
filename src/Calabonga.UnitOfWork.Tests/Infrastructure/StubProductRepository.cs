using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Calabonga.PagedListCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;

namespace Calabonga.UnitOfWork.Tests.Infrastructure;

/// <summary>
/// Custom repository used only to verify <c>AddCustomRepository</c> DI registration.
/// Delegates everything to a real <see cref="Repository{TEntity}"/> (which is sealed,
/// hence composition rather than inheritance).
/// </summary>
public sealed class StubProductRepository(TestDbContext context) : IRepository<Product>
{
    private readonly Repository<Product> _inner = new(context);

    public Product? Find(params object[] keyValues) => _inner.Find(keyValues);

    public ValueTask<Product?> FindAsync(params object[] keyValues) => _inner.FindAsync(keyValues);

    public ValueTask<Product?> FindAsync(object[] keyValues, CancellationToken cancellationToken)
        => _inner.FindAsync(keyValues, cancellationToken);

    public Product Insert(Product entity) => _inner.Insert(entity);

    public void Insert(params Product[] entities) => _inner.Insert(entities);

    public void Insert(IEnumerable<Product> entities) => _inner.Insert(entities);

    public ValueTask<EntityEntry<Product>> InsertAsync(Product entity, CancellationToken cancellationToken = default)
        => _inner.InsertAsync(entity, cancellationToken);

    public Task InsertAsync(params Product[] entities) => _inner.InsertAsync(entities);

    public Task InsertAsync(IEnumerable<Product> entities, CancellationToken cancellationToken = default)
        => _inner.InsertAsync(entities, cancellationToken);

    public void Update(Product entity) => _inner.Update(entity);

    public void Update(params Product[] entities) => _inner.Update(entities);

    public void Update(IEnumerable<Product> entities) => _inner.Update(entities);

    public int ExecuteUpdate(Action<UpdateSettersBuilder<Product>> predicate) => _inner.ExecuteUpdate(predicate);

    public Task<int> ExecuteUpdateAsync(Action<UpdateSettersBuilder<Product>> predicate, CancellationToken cancellationToken)
        => _inner.ExecuteUpdateAsync(predicate, cancellationToken);

    public void Delete(object id) => _inner.Delete(id);

    public void Delete(Product entity) => _inner.Delete(entity);

    public void Delete(params Product[] entities) => _inner.Delete(entities);

    public void Delete(IEnumerable<Product> entities) => _inner.Delete(entities);

    public int ExecuteDelete() => _inner.ExecuteDelete();

    public Task<int> ExecuteDeleteAsync(CancellationToken cancellationToken = default) => _inner.ExecuteDeleteAsync(cancellationToken);

    public int Count(Expression<Func<Product, bool>>? predicate = null) => _inner.Count(predicate);

    public Task<int> CountAsync(Expression<Func<Product, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => _inner.CountAsync(predicate, cancellationToken);

    public long LongCount(Expression<Func<Product, bool>>? predicate = null) => _inner.LongCount(predicate);

    public Task<long> LongCountAsync(Expression<Func<Product, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => _inner.LongCountAsync(predicate, cancellationToken);

    public bool Exists(Expression<Func<Product, bool>>? predicate = null) => _inner.Exists(predicate);

    public Task<bool> ExistsAsync(Expression<Func<Product, bool>>? selector = null, CancellationToken cancellationToken = default)
        => _inner.ExistsAsync(selector, cancellationToken);

    public T? Max<T>(Expression<Func<Product, T>> selector, Expression<Func<Product, bool>>? predicate = null)
        => _inner.Max(selector, predicate);

    public Task<T> MaxAsync<T>(Expression<Func<Product, T>> selector, Expression<Func<Product, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => _inner.MaxAsync(selector, predicate, cancellationToken);

    public T? Min<T>(Expression<Func<Product, T>> selector, Expression<Func<Product, bool>>? predicate = null)
        => _inner.Min(selector, predicate);

    public Task<T> MinAsync<T>(Expression<Func<Product, T>> selector, Expression<Func<Product, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => _inner.MinAsync(selector, predicate, cancellationToken);

    public decimal Average(Expression<Func<Product, decimal>> selector, Expression<Func<Product, bool>>? predicate = null)
        => _inner.Average(selector, predicate);

    public Task<decimal> AverageAsync(Expression<Func<Product, decimal>> selector, Expression<Func<Product, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => _inner.AverageAsync(selector, predicate, cancellationToken);

    public decimal Sum(Expression<Func<Product, decimal>> selector, Expression<Func<Product, bool>>? predicate = null)
        => _inner.Sum(selector, predicate);

    public Task<decimal> SumAsync(Expression<Func<Product, decimal>> selector, Expression<Func<Product, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => _inner.SumAsync(selector, predicate, cancellationToken);

    public void ChangeEntityState(Product entity, EntityState state) => _inner.ChangeEntityState(entity, state);

    public void ChangeTable(string table) => _inner.ChangeTable(table);

    public IQueryable<Product> FromSql(string sql, params object[] parameters) => _inner.FromSql(sql, parameters);

    public IPagedList<Product> GetPagedList(
        Expression<Func<Product, bool>>? predicate = null,
        Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
        Func<IQueryable<Product>, IIncludableQueryable<Product, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false)
        => _inner.GetPagedList(predicate, orderBy, include, pageIndex, pageSize, trackingType, ignoreQueryFilters, ignoreAutoIncludes);

    public Task<IPagedList<Product>> GetPagedListAsync(
        Expression<Func<Product, bool>>? predicate = null,
        Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
        Func<IQueryable<Product>, IIncludableQueryable<Product, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        TrackingType trackingType = TrackingType.NoTracking,
        CancellationToken cancellationToken = default,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false)
        => _inner.GetPagedListAsync(predicate, orderBy, include, pageIndex, pageSize, trackingType, cancellationToken, ignoreQueryFilters, ignoreAutoIncludes);

    public IPagedList<TResult> GetPagedList<TResult>(
        Expression<Func<Product, TResult>> selector,
        Expression<Func<Product, bool>>? predicate = null,
        Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
        Func<IQueryable<Product>, IIncludableQueryable<Product, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false) where TResult : class
        => _inner.GetPagedList(selector, predicate, orderBy, include, pageIndex, pageSize, trackingType, ignoreQueryFilters, ignoreAutoIncludes);

    public Task<IPagedList<TResult>> GetPagedListAsync<TResult>(
        Expression<Func<Product, TResult>> selector,
        Expression<Func<Product, bool>>? predicate = null,
        Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
        Func<IQueryable<Product>, IIncludableQueryable<Product, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        TrackingType trackingType = TrackingType.NoTracking,
        CancellationToken cancellationToken = default,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false) where TResult : class
        => _inner.GetPagedListAsync(selector, predicate, orderBy, include, pageIndex, pageSize, trackingType, cancellationToken, ignoreQueryFilters, ignoreAutoIncludes);

    public Product? GetFirstOrDefault(
        Expression<Func<Product, bool>>? predicate = null,
        Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
        Func<IQueryable<Product>, IIncludableQueryable<Product, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false)
        => _inner.GetFirstOrDefault(predicate, orderBy, include, trackingType, ignoreQueryFilters, ignoreAutoIncludes);

    public TResult? GetFirstOrDefault<TResult>(
        Expression<Func<Product, TResult>> selector,
        Expression<Func<Product, bool>>? predicate = null,
        Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
        Func<IQueryable<Product>, IIncludableQueryable<Product, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false)
        => _inner.GetFirstOrDefault(selector, predicate, orderBy, include, trackingType, ignoreQueryFilters, ignoreAutoIncludes);

    public Task<TResult?> GetFirstOrDefaultAsync<TResult>(
        Expression<Func<Product, TResult>> selector,
        Expression<Func<Product, bool>>? predicate = null,
        Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
        Func<IQueryable<Product>, IIncludableQueryable<Product, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false)
        => _inner.GetFirstOrDefaultAsync(selector, predicate, orderBy, include, trackingType, ignoreQueryFilters, ignoreAutoIncludes);

    public Task<Product?> GetFirstOrDefaultAsync(
        Expression<Func<Product, bool>>? predicate = null,
        Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
        Func<IQueryable<Product>, IIncludableQueryable<Product, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false)
        => _inner.GetFirstOrDefaultAsync(predicate, orderBy, include, trackingType, ignoreQueryFilters, ignoreAutoIncludes);

    public IQueryable<Product> GetAll(TrackingType trackingType = TrackingType.NoTracking) => _inner.GetAll(trackingType);

    public IQueryable<TResult> GetAll<TResult>(Expression<Func<Product, TResult>> selector, TrackingType trackingType = TrackingType.NoTracking)
        => _inner.GetAll(selector, trackingType);

    public IQueryable<TResult> GetAll<TResult>(
        Expression<Func<Product, TResult>> selector,
        Expression<Func<Product, bool>>? predicate = null,
        TrackingType trackingType = TrackingType.NoTracking)
        => _inner.GetAll(selector, predicate, trackingType);

    public IQueryable<Product> GetAll(
        Expression<Func<Product, bool>>? predicate = null,
        Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
        Func<IQueryable<Product>, IIncludableQueryable<Product, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false)
        => _inner.GetAll(predicate, orderBy, include, trackingType, ignoreQueryFilters, ignoreAutoIncludes);

    public IQueryable<TResult> GetAll<TResult>(
        Expression<Func<Product, TResult>> selector,
        Expression<Func<Product, bool>>? predicate = null,
        Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
        Func<IQueryable<Product>, IIncludableQueryable<Product, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false)
        => _inner.GetAll(selector, predicate, orderBy, include, trackingType, ignoreQueryFilters, ignoreAutoIncludes);

    public Task<IList<Product>> GetAllAsync(TrackingType trackingType = TrackingType.NoTracking) => _inner.GetAllAsync(trackingType);

    public Task<IList<TResult>> GetAllAsync<TResult>(Expression<Func<Product, TResult>> selector, TrackingType trackingType = TrackingType.NoTracking)
        => _inner.GetAllAsync(selector, trackingType);

    public Task<IList<Product>> GetAllAsync(
        Expression<Func<Product, bool>>? predicate = null,
        Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
        Func<IQueryable<Product>, IIncludableQueryable<Product, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false)
        => _inner.GetAllAsync(predicate, orderBy, include, trackingType, ignoreQueryFilters, ignoreAutoIncludes);

    public Task<IList<TResult>> GetAllAsync<TResult>(
        Expression<Func<Product, TResult>> selector,
        Expression<Func<Product, bool>>? predicate = null,
        Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
        Func<IQueryable<Product>, IIncludableQueryable<Product, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false)
        => _inner.GetAllAsync(selector, predicate, orderBy, include, trackingType, ignoreQueryFilters, ignoreAutoIncludes);
}
