using Calabonga.PagedListCore;
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
    #region GetPagedList

    /// <summary>
    /// Gets the <see cref="IPagedList{T}"/> based on a predicate, orderBy delegate and page information. This method default no-tracking query.. This method default no-tracking query.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="orderBy">A function to order elements.</param>
    /// <param name="include">A function to include navigation properties</param>
    /// <param name="pageIndex">The index of page.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="trackingType"><c>NoTracking</c> to disable changing tracking; <c>Tracking</c> to enable changing tracking; <c>NoTrackingWithIdentityResolution</c> to disable changing tracking but identity resolving. Default to <c>NoTracking</c>.</param>
    /// <param name="ignoreQueryFilters">Ignore query filters</param>
    /// <param name="ignoreAutoIncludes">Ignore automatic includes</param>
    /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
    /// <remarks>This method default no-tracking query.</remarks>
    IPagedList<TEntity> GetPagedList(
    Expression<Func<TEntity, bool>>? predicate = null,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
    int pageIndex = 0,
    int pageSize = 20,
    TrackingType trackingType = TrackingType.NoTracking,
    bool ignoreQueryFilters = false,
    bool ignoreAutoIncludes = false);

    /// <summary>
    /// Gets the <see cref="IPagedList{TEntity}"/> based on a predicate, orderBy delegate and page information. This method default no-tracking query.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="orderBy">A function to order elements.</param>
    /// <param name="include">A function to include navigation properties</param>
    /// <param name="pageIndex">The index of page.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="trackingType"><c>NoTracking</c> to disable changing tracking; <c>Tracking</c> to enable changing tracking; <c>NoTrackingWithIdentityResolution</c> to disable changing tracking but identity resolving. Default to <c>NoTracking</c>.</param>
    /// <param name="cancellationToken">
    ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <param name="ignoreQueryFilters">Ignore query filters</param>
    /// <param name="ignoreAutoIncludes">Ignore automatic includes</param>
    /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
    /// <remarks>This method default no-tracking query.</remarks>
    Task<IPagedList<TEntity>> GetPagedListAsync(
    Expression<Func<TEntity, bool>>? predicate = null,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
    int pageIndex = 0,
    int pageSize = 20,
    TrackingType trackingType = TrackingType.NoTracking,
    CancellationToken cancellationToken = default,
    bool ignoreQueryFilters = false,
    bool ignoreAutoIncludes = false);

    /// <summary>
    /// Gets the <see cref="IPagedList{TResult}"/> based on a predicate, orderBy delegate and page information. This method default no-tracking query.
    /// </summary>
    /// <param name="selector">The selector for projection.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="orderBy">A function to order elements.</param>
    /// <param name="include">A function to include navigation properties</param>
    /// <param name="pageIndex">The index of page.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="trackingType"><c>NoTracking</c> to disable changing tracking; <c>Tracking</c> to enable changing tracking; <c>NoTrackingWithIdentityResolution</c> to disable changing tracking but identity resolving. Default to <c>NoTracking</c>.</param>
    /// <param name="ignoreQueryFilters">Ignore query filters</param>
    /// <param name="ignoreAutoIncludes">Ignore automatic includes</param>
    /// <returns>An <see cref="IPagedList{TResult}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
    /// <remarks>This method default no-tracking query.</remarks>
    IPagedList<TResult> GetPagedList<TResult>(
    Expression<Func<TEntity, TResult>> selector,
    Expression<Func<TEntity, bool>>? predicate = null,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
    int pageIndex = 0,
    int pageSize = 20,
    TrackingType trackingType = TrackingType.NoTracking,
    bool ignoreQueryFilters = false,
    bool ignoreAutoIncludes = false) where TResult : class;

    /// <summary>
    /// Gets the <see cref="IPagedList{TEntity}"/> based on a predicate, orderBy delegate and page information. This method default no-tracking query.
    /// </summary>
    /// <param name="selector">The selector for projection.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="orderBy">A function to order elements.</param>
    /// <param name="include">A function to include navigation properties</param>
    /// <param name="pageIndex">The index of page.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="trackingType"><c>NoTracking</c> to disable changing tracking; <c>Tracking</c> to enable changing tracking; <c>NoTrackingWithIdentityResolution</c> to disable changing tracking but identity resolving. Default to <c>NoTracking</c>.</param>
    /// <param name="cancellationToken">
    ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <param name="ignoreQueryFilters">Ignore query filters</param>
    /// <param name="ignoreAutoIncludes">Ignore automatic includes</param>
    /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
    /// <remarks>This method default no-tracking query.</remarks>
    Task<IPagedList<TResult>> GetPagedListAsync<TResult>(
    Expression<Func<TEntity, TResult>> selector,
    Expression<Func<TEntity, bool>>? predicate = null,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
    int pageIndex = 0,
    int pageSize = 20,
    TrackingType trackingType = TrackingType.NoTracking,
    CancellationToken cancellationToken = default,
    bool ignoreQueryFilters = false,
    bool ignoreAutoIncludes = false) where TResult : class;

    #endregion

    #region GetFirstOrDefault

    /// <summary>
    /// Gets the first or default entity based on a predicate, orderBy delegate and include delegate. This method defaults to a read-only, no-tracking query.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="orderBy">A function to order elements.</param>
    /// <param name="include">A function to include navigation properties</param>
    /// <param name="trackingType"><c>NoTracking</c> to disable changing tracking; <c>Tracking</c> to enable changing tracking; <c>NoTrackingWithIdentityResolution</c> to disable changing tracking but identity resolving. Default to <c>NoTracking</c>.</param>
    /// <param name="ignoreQueryFilters">Ignore query filters</param>
    /// <param name="ignoreAutoIncludes">Ignore automatic includes</param>
    /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
    /// <remarks>This method defaults to a read-only, no-tracking query.</remarks>
    TEntity? GetFirstOrDefault(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false);

    /// <summary>
    /// Gets the first or default entity based on a predicate, orderBy delegate and include delegate. This method defaults to a read-only, no-tracking query.
    /// </summary>
    /// <param name="selector">The selector for projection.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="orderBy">A function to order elements.</param>
    /// <param name="include">A function to include navigation properties</param>
    /// <param name="trackingType"><c>NoTracking</c> to disable changing tracking; <c>Tracking</c> to enable changing tracking; <c>NoTrackingWithIdentityResolution</c> to disable changing tracking but identity resolving. Default to <c>NoTracking</c>.</param>
    /// <param name="ignoreQueryFilters">Ignore query filters</param>
    /// <param name="ignoreAutoIncludes">Ignore automatic includes</param>
    /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
    /// <remarks>This method defaults to a read-only, no-tracking query.</remarks>
    TResult? GetFirstOrDefault<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false);

    /// <summary>
    /// Gets the first or default entity based on a predicate, orderBy delegate and include delegate. This method defaults to a read-only, no-tracking query.
    /// </summary>
    /// <param name="selector">The selector for projection.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="orderBy">A function to order elements.</param>
    /// <param name="include">A function to include navigation properties</param>
    /// <param name="trackingType"><c>NoTracking</c> to disable changing tracking; <c>Tracking</c> to enable changing tracking; <c>NoTrackingWithIdentityResolution</c> to disable changing tracking but identity resolving. Default to <c>NoTracking</c>.</param>
    /// <param name="ignoreQueryFilters">Ignore query filters</param>
    /// <param name="ignoreAutoIncludes">Ignore automatic includes</param>
    /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
    /// <remarks>Ex: This method defaults to a read-only, no-tracking query.</remarks>
    Task<TResult?> GetFirstOrDefaultAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false);

    /// <summary>
    /// Gets the first or default entity based on a predicate, orderBy delegate and include delegate. This method defaults to a read-only, no-tracking query.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="orderBy">A function to order elements.</param>
    /// <param name="include">A function to include navigation properties</param>
    /// <param name="trackingType"><c>NoTracking</c> to disable changing tracking; <c>Tracking</c> to enable changing tracking; <c>NoTrackingWithIdentityResolution</c> to disable changing tracking but identity resolving. Default to <c>NoTracking</c>.</param>
    /// <param name="ignoreQueryFilters">Ignore query filters</param>
    /// <param name="ignoreAutoIncludes">Ignore automatic includes</param>
    /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
    /// <remarks>Ex: This method defaults to a read-only, no-tracking query. </remarks>
    Task<TEntity?> GetFirstOrDefaultAsync
    (
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false);

    #endregion

    #region GetAll

    /// <summary>
    /// Gets all entities. This method is not recommended
    /// </summary>
    /// <returns>The <see cref="IQueryable{TEntity}"/>.</returns>
    /// <param name="trackingType"><c>NoTracking</c> to disable changing tracking; <c>Tracking</c> to enable changing tracking; <c>NoTrackingWithIdentityResolution</c> to disable changing tracking but identity resolving. Default to <c>NoTracking</c>.</param>
    IQueryable<TEntity> GetAll(TrackingType trackingType = TrackingType.NoTracking);

    /// <summary>
    /// Gets all entities. This method is not recommended
    /// </summary>
    /// <param name="selector">The selector for projection.</param>
    /// <param name="trackingType"><c>NoTracking</c> to disable changing tracking; <c>Tracking</c> to enable changing tracking; <c>NoTrackingWithIdentityResolution</c> to disable changing tracking but identity resolving. Default to <c>NoTracking</c>.</param>
    /// <returns>The <see cref="IQueryable{TEntity}"/>.</returns>
    IQueryable<TResult> GetAll<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        TrackingType trackingType = TrackingType.NoTracking);

    /// <summary>
    /// Gets all entities. This method is not recommended
    /// </summary>
    /// <param name="selector">The selector for projection.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="trackingType"><c>NoTracking</c> to disable changing tracking; <c>Tracking</c> to enable changing tracking; <c>NoTrackingWithIdentityResolution</c> to disable changing tracking but identity resolving. Default to <c>NoTracking</c>.</param>
    /// <returns>The <see cref="IQueryable{TEntity}"/>.</returns>
    IQueryable<TResult> GetAll<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        TrackingType trackingType = TrackingType.NoTracking);

    /// <summary>
    /// Gets all entities. This method is not recommended
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="orderBy">A function to order elements.</param>
    /// <param name="include">A function to include navigation properties</param>
    /// <param name="trackingType"><c>NoTracking</c> to disable changing tracking; <c>Tracking</c> to enable changing tracking; <c>NoTrackingWithIdentityResolution</c> to disable changing tracking but identity resolving. Default to <c>NoTracking</c>.</param>
    /// <param name="ignoreQueryFilters">Ignore query filters</param>
    /// <param name="ignoreAutoIncludes">Ignore automatic includes</param>
    /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
    /// <remarks>Ex: This method defaults to a read-only, no-tracking query.</remarks>
    IQueryable<TEntity> GetAll(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false);

    /// <summary>
    /// Gets all entities. This method is not recommended
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="selector">The selector for projection.</param>
    /// <param name="orderBy">A function to order elements.</param>
    /// <param name="include">A function to include navigation properties</param>
    /// <param name="trackingType"><c>NoTracking</c> to disable changing tracking; <c>Tracking</c> to enable changing tracking; <c>NoTrackingWithIdentityResolution</c> to disable changing tracking but identity resolving. Default to <c>NoTracking</c>.</param>
    /// <param name="ignoreQueryFilters">Ignore query filters</param>
    /// <param name="ignoreAutoIncludes">Ignore automatic includes</param>
    /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
    /// <remarks>Ex: This method defaults to a read-only, no-tracking query.</remarks>
    IQueryable<TResult> GetAll<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false);

    /// <summary>
    /// Gets all entities. This method is not recommended
    /// </summary>
    /// <param name="trackingType"><c>NoTracking</c> to disable changing tracking; <c>Tracking</c> to enable changing tracking; <c>NoTrackingWithIdentityResolution</c> to disable changing tracking but identity resolving. Default to <c>NoTracking</c>.</param>
    /// <returns>The <see cref="IQueryable{TEntity}"/>.</returns>
    Task<IList<TEntity>> GetAllAsync(TrackingType trackingType = TrackingType.NoTracking);

    /// <summary>
    /// Gets all entities. This method is not recommended
    /// </summary>
    /// <param name="selector">The selector for projection.</param>
    /// <param name="trackingType"><c>NoTracking</c> to disable changing tracking; <c>Tracking</c> to enable changing tracking; <c>NoTrackingWithIdentityResolution</c> to disable changing tracking but identity resolving. Default to <c>NoTracking</c>.</param>
    /// <returns>The <see cref="IQueryable{TEntity}"/>.</returns>
    Task<IList<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        TrackingType trackingType = TrackingType.NoTracking);

    /// <summary>
    /// Gets all entities. This method is not recommended
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="orderBy">A function to order elements.</param>
    /// <param name="include">A function to include navigation properties</param>
    /// <param name="trackingType"><c>NoTracking</c> to disable changing tracking; <c>Tracking</c> to enable changing tracking; <c>NoTrackingWithIdentityResolution</c> to disable changing tracking but identity resolving. Default to <c>NoTracking</c>.</param>
    /// <param name="ignoreQueryFilters">Ignore query filters</param>
    /// <param name="ignoreAutoIncludes">Ignore automatic includes</param>
    /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
    /// <remarks>Ex: This method defaults to a read-only, no-tracking query.</remarks>
    Task<IList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false);

    /// <summary>
    /// Gets all entities. This method is not recommended
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="selector">The selector for projection.</param>
    /// <param name="orderBy">A function to order elements.</param>
    /// <param name="include">A function to include navigation properties</param>
    /// <param name="trackingType"><c>NoTracking</c> to disable changing tracking; <c>Tracking</c> to enable changing tracking; <c>NoTrackingWithIdentityResolution</c> to disable changing tracking but identity resolving. Default to <c>NoTracking</c>.</param>
    /// <param name="ignoreQueryFilters">Ignore query filters</param>
    /// <param name="ignoreAutoIncludes">Ignore automatic includes</param>
    /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
    /// <remarks>Ex: This method defaults to a read-only, no-tracking query.</remarks>
    Task<IList<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false);

    #endregion
}