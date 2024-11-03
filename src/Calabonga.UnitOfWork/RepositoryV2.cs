using Calabonga.PagedListCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Calabonga.UnitOfWork;

/// <summary>
/// Represents a default generic repository implements the <see cref="IRepository{TEntity}"/> interface.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public sealed partial class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{

    /// <summary>
    /// Gets all entities. This method is not recommended
    /// </summary>
    /// <returns>The <see cref="IQueryable{TEntity}"/>.</returns>
    public IQueryable<TEntity> GetAll(TrackingType trackingType = TrackingType.NoTracking) =>
        trackingType switch
        {
            TrackingType.NoTracking => _dbSet.AsNoTracking(),
            TrackingType.NoTrackingWithIdentityResolution => _dbSet.AsNoTrackingWithIdentityResolution(),
            TrackingType.Tracking => _dbSet,
            _ => throw new ArgumentOutOfRangeException(nameof(trackingType), trackingType, null)
        };

    /// <summary>
    /// Gets all entities. This method is not recommended
    /// </summary>
    /// <param name="selector">The selector for projection.</param>
    /// <param name="trackingType"><c>NoTracking</c> to disable changing tracking; <c>Tracking</c> to enable changing tracking; <c>NoTrackingWithIdentityResolution</c> to disable changing tracking but identity resolving. Default to <c>NoTracking</c>.</param>
    /// <returns>The <see cref="IQueryable{TEntity}"/>.</returns>
    public IQueryable<TResult> GetAll<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        TrackingType trackingType = TrackingType.NoTracking) =>
        trackingType switch
        {
            TrackingType.NoTracking => _dbSet.AsNoTracking().Select(selector),
            TrackingType.NoTrackingWithIdentityResolution => _dbSet.AsNoTrackingWithIdentityResolution().Select(selector),
            TrackingType.Tracking => _dbSet.Select(selector),
            _ => throw new ArgumentOutOfRangeException(nameof(trackingType), trackingType, null)
        };

    /// <summary>
    /// Gets all entities. This method is not recommended
    /// </summary>
    /// <param name="selector">The selector for projection.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="trackingType"><c>NoTracking</c> to disable changing tracking; <c>Tracking</c> to enable changing tracking; <c>NoTrackingWithIdentityResolution</c> to disable changing tracking but identity resolving. Default to <c>NoTracking</c>.</param>
    /// <returns>The <see cref="IQueryable{TEntity}"/>.</returns>
    public IQueryable<TResult> GetAll<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        TrackingType trackingType = TrackingType.NoTracking)
    {
        var query = trackingType switch
        {
            TrackingType.NoTracking => _dbSet.AsNoTracking(),
            TrackingType.NoTrackingWithIdentityResolution => _dbSet.AsNoTrackingWithIdentityResolution(),
            TrackingType.Tracking => _dbSet,
            _ => throw new ArgumentOutOfRangeException(nameof(trackingType), trackingType, null)
        };

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return query.Select(selector);
    }

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
    public IQueryable<TEntity> GetAll(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false)
    {
        var query = trackingType switch
        {
            TrackingType.NoTracking => _dbSet.AsNoTracking(),
            TrackingType.NoTrackingWithIdentityResolution => _dbSet.AsNoTrackingWithIdentityResolution(),
            TrackingType.Tracking => _dbSet,
            _ => throw new ArgumentOutOfRangeException(nameof(trackingType), trackingType, null)
        };

        if (include is not null)
        {
            query = include(query);
        }

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        if (ignoreAutoIncludes)
        {
            query = query.IgnoreAutoIncludes();
        }

        return orderBy is not null
            ? orderBy(query)
            : query;
    }

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
    public IQueryable<TResult> GetAll<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false)
    {

        var query = trackingType switch
        {
            TrackingType.NoTracking => _dbSet.AsNoTracking(),
            TrackingType.NoTrackingWithIdentityResolution => _dbSet.AsNoTrackingWithIdentityResolution(),
            TrackingType.Tracking => _dbSet,
            _ => throw new ArgumentOutOfRangeException(nameof(trackingType), trackingType, null)
        };

        if (include is not null)
        {
            query = include(query);
        }

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        if (ignoreAutoIncludes)
        {
            query = query.IgnoreAutoIncludes();
        }

        return orderBy != null
            ? orderBy(query).Select(selector)
            : query.Select(selector);
    }

    /// <summary>
    /// Gets all entities. This method is not recommended
    /// </summary>
    /// <param name="trackingType"><c>NoTracking</c> to disable changing tracking; <c>Tracking</c> to enable changing tracking; <c>NoTrackingWithIdentityResolution</c> to disable changing tracking but identity resolving. Default to <c>NoTracking</c>.</param>
    /// <returns>The <see cref="IQueryable{TEntity}"/>.</returns>
    public async Task<IList<TEntity>> GetAllAsync(TrackingType trackingType = TrackingType.NoTracking) =>
        trackingType switch
        {
            TrackingType.NoTracking => await _dbSet.AsNoTracking().ToListAsync(),
            TrackingType.NoTrackingWithIdentityResolution => await _dbSet.AsNoTrackingWithIdentityResolution().ToListAsync(),
            TrackingType.Tracking => await _dbSet.ToListAsync(),
            _ => throw new ArgumentOutOfRangeException(nameof(trackingType), trackingType, null)
        };

    /// <summary>
    /// Gets all entities. This method is not recommended
    /// </summary>
    /// <param name="selector">The selector for projection.</param>
    /// <param name="trackingType"><c>NoTracking</c> to disable changing tracking; <c>Tracking</c> to enable changing tracking; <c>NoTrackingWithIdentityResolution</c> to disable changing tracking but identity resolving. Default to <c>NoTracking</c>.</param>
    /// <returns>The <see cref="IQueryable{TEntity}"/>.</returns>
    public async Task<IList<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, TrackingType trackingType = TrackingType.NoTracking) =>
        trackingType switch
        {
            TrackingType.NoTracking => await _dbSet.AsNoTracking().Select(selector).ToListAsync(),
            TrackingType.NoTrackingWithIdentityResolution => await _dbSet.AsNoTrackingWithIdentityResolution().Select(selector).ToListAsync(),
            TrackingType.Tracking => await _dbSet.Select(selector).ToListAsync(),
            _ => throw new ArgumentOutOfRangeException(nameof(trackingType), trackingType, null)
        };

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
    public async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false)
    {
        var query = trackingType switch
        {
            TrackingType.NoTracking => _dbSet.AsNoTracking(),
            TrackingType.NoTrackingWithIdentityResolution => _dbSet.AsNoTrackingWithIdentityResolution(),
            TrackingType.Tracking => _dbSet,
            _ => throw new ArgumentOutOfRangeException(nameof(trackingType), trackingType, null)
        };

        if (include is not null)
        {
            query = include(query);
        }

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        if (ignoreAutoIncludes)
        {
            query = query.IgnoreAutoIncludes();
        }

        if (orderBy is not null)
        {
            return await orderBy(query).ToListAsync();
        }

        return await query.ToListAsync();
    }

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
    public async Task<IList<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false)
    {
        var query = trackingType switch
        {
            TrackingType.NoTracking => _dbSet.AsNoTracking(),
            TrackingType.NoTrackingWithIdentityResolution => _dbSet.AsNoTrackingWithIdentityResolution(),
            TrackingType.Tracking => _dbSet,
            _ => throw new ArgumentOutOfRangeException(nameof(trackingType), trackingType, null)
        };

        if (include is not null)
        {
            query = include(query);
        }

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        if (ignoreAutoIncludes)
        {
            query = query.IgnoreAutoIncludes();
        }

        return orderBy is not null
            ? await orderBy(query).Select(selector).ToListAsync()
            : await query.Select(selector).ToListAsync();
    }

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
    public IPagedList<TEntity> GetPagedList(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false)
    {
        var query = trackingType switch
        {
            TrackingType.NoTracking => _dbSet.AsNoTracking(),
            TrackingType.NoTrackingWithIdentityResolution => _dbSet.AsNoTrackingWithIdentityResolution(),
            TrackingType.Tracking => _dbSet,
            _ => throw new ArgumentOutOfRangeException(nameof(trackingType), trackingType, null)
        };


        if (include is not null)
        {
            query = include(query);
        }

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        if (ignoreAutoIncludes)
        {
            query = query.IgnoreAutoIncludes();
        }

        return orderBy is not null
            ? orderBy(query).ToPagedList(pageIndex, pageSize)
            : query.ToPagedList(pageIndex, pageSize);
    }

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
    public Task<IPagedList<TEntity>> GetPagedListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        TrackingType trackingType = TrackingType.NoTracking,
        CancellationToken cancellationToken = default,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false)
    {
        var query = trackingType switch
        {
            TrackingType.NoTracking => _dbSet.AsNoTracking(),
            TrackingType.NoTrackingWithIdentityResolution => _dbSet.AsNoTrackingWithIdentityResolution(),
            TrackingType.Tracking => _dbSet,
            _ => throw new ArgumentOutOfRangeException(nameof(trackingType), trackingType, null)
        };


        if (include is not null)
        {
            query = include(query);
        }

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        if (ignoreAutoIncludes)
        {
            query = query.IgnoreAutoIncludes();
        }

        return orderBy is not null
            ? orderBy(query).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken)
            : query.ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
    }

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
    public IPagedList<TResult> GetPagedList<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false)
        where TResult : class
    {
        var query = trackingType switch
        {
            TrackingType.NoTracking => _dbSet.AsNoTracking(),
            TrackingType.NoTrackingWithIdentityResolution => _dbSet.AsNoTrackingWithIdentityResolution(),
            TrackingType.Tracking => _dbSet,
            _ => throw new ArgumentOutOfRangeException(nameof(trackingType), trackingType, null)
        };

        if (include is not null)
        {
            query = include(query);
        }

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        if (ignoreAutoIncludes)
        {
            query = query.IgnoreAutoIncludes();
        }

        return orderBy is not null
            ? orderBy(query).Select(selector).ToPagedList(pageIndex, pageSize)
            : query.Select(selector).ToPagedList(pageIndex, pageSize);
    }

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
    public Task<IPagedList<TResult>> GetPagedListAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        TrackingType trackingType = TrackingType.NoTracking,
        CancellationToken cancellationToken = default,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false)
        where TResult : class
    {
        var query = trackingType switch
        {
            TrackingType.NoTracking => _dbSet.AsNoTracking(),
            TrackingType.NoTrackingWithIdentityResolution => _dbSet.AsNoTrackingWithIdentityResolution(),
            TrackingType.Tracking => _dbSet,
            _ => throw new ArgumentOutOfRangeException(nameof(trackingType), trackingType, null)
        };

        if (include != null)
        {
            query = include(query);
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        if (ignoreAutoIncludes)
        {
            query = query.IgnoreAutoIncludes();
        }

        return orderBy != null
            ? orderBy(query).Select(selector).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken)
            : query.Select(selector).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
    }

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
    public TEntity? GetFirstOrDefault(Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false)
    {
        var query = trackingType switch
        {
            TrackingType.NoTracking => _dbSet.AsNoTracking(),
            TrackingType.NoTrackingWithIdentityResolution => _dbSet.AsNoTrackingWithIdentityResolution(),
            TrackingType.Tracking => _dbSet,
            _ => throw new ArgumentOutOfRangeException(nameof(trackingType), trackingType, null)
        };

        if (include is not null)
        {
            query = include(query);
        }

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        if (ignoreAutoIncludes)
        {
            query = query.IgnoreAutoIncludes();
        }

        return orderBy is not null
            ? orderBy(query).FirstOrDefault()
            : query.FirstOrDefault();
    }

    public async Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false)
    {
        var query = trackingType switch
        {
            TrackingType.NoTracking => _dbSet.AsNoTracking(),
            TrackingType.NoTrackingWithIdentityResolution => _dbSet.AsNoTrackingWithIdentityResolution(),
            TrackingType.Tracking => _dbSet,
            _ => throw new ArgumentOutOfRangeException(nameof(trackingType), trackingType, null)
        };

        if (include is not null)
        {
            query = include(query);
        }

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        if (ignoreAutoIncludes)
        {
            query = query.IgnoreAutoIncludes();
        }

        return orderBy is not null
            ? await orderBy(query).FirstOrDefaultAsync()
            : await query.FirstOrDefaultAsync();
    }

    public TResult? GetFirstOrDefault<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false)
    {
        var query = trackingType switch
        {
            TrackingType.NoTracking => _dbSet.AsNoTracking(),
            TrackingType.NoTrackingWithIdentityResolution => _dbSet.AsNoTrackingWithIdentityResolution(),
            TrackingType.Tracking => _dbSet,
            _ => throw new ArgumentOutOfRangeException(nameof(trackingType), trackingType, null)
        };

        if (include is not null)
        {
            query = include(query);
        }

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        if (ignoreAutoIncludes)
        {
            query = query.IgnoreAutoIncludes();
        }

        return orderBy is not null
            ? orderBy(query).Select(selector).FirstOrDefault()
            : query.Select(selector).FirstOrDefault();
    }

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
    public async Task<TResult?> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        TrackingType trackingType = TrackingType.NoTracking,
        bool ignoreQueryFilters = false,
        bool ignoreAutoIncludes = false)
    {
        var query = trackingType switch
        {
            TrackingType.NoTracking => _dbSet.AsNoTracking(),
            TrackingType.NoTrackingWithIdentityResolution => _dbSet.AsNoTrackingWithIdentityResolution(),
            TrackingType.Tracking => _dbSet,
            _ => throw new ArgumentOutOfRangeException(nameof(trackingType), trackingType, null)
        };

        if (include is not null)
        {
            query = include(query);
        }

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        if (ignoreAutoIncludes)
        {
            query = query.IgnoreAutoIncludes();
        }

        return orderBy is not null
            ? await orderBy(query).Select(selector).FirstOrDefaultAsync()
            : await query.Select(selector).FirstOrDefaultAsync();
    }
}