using System;
using System.Collections.Generic;
using Calabonga.PagedListCore;

namespace Calabonga.UnitOfWork;

/// <summary>
/// Internal <see cref="IPagedList{T}"/> implementation with consistent, zero-based paging
/// semantics.
/// </summary>
/// <remarks>
/// <para>
/// It exists because <c>Calabonga.PagedListCore</c> only ships the <see cref="IPagedList{T}"/>
/// contract that is convenient to reuse, while its own <c>PagedList&lt;T&gt;</c> is strictly
/// one-based and throws for <c>pageIndex</c> below <c>1</c>. This library keeps the historical
/// zero-based contract (default <c>pageIndex</c> is <c>0</c>), so both the synchronous and the
/// asynchronous <c>GetPagedList</c> paths build this type instead to guarantee identical,
/// zero-based behaviour.
/// </para>
/// <para>
/// <see cref="PageIndex"/> is zero-based: the first page is <c>0</c>, which is also the default
/// value of the <c>pageIndex</c> parameters on <see cref="IRepository{TEntity}"/>.
/// </para>
/// </remarks>
/// <typeparam name="T">The type of the page items.</typeparam>
internal sealed class PagedListResult<T> : IPagedList<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PagedListResult{T}"/> class.
    /// </summary>
    /// <param name="items">The items of the current page (already sliced).</param>
    /// <param name="pageIndex">The zero-based index of the current page.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="totalCount">The total number of items across all pages.</param>
    /// <param name="indexFrom">The value the first page index starts from.</param>
    public PagedListResult(IList<T> items, int pageIndex, int pageSize, int totalCount, int indexFrom = 0)
    {
        Items = items;
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
        IndexFrom = indexFrom;
        TotalPages = pageSize > 0
            ? (int)Math.Ceiling(totalCount / (double)pageSize)
            : 0;
    }

    /// <summary>The value the first page index starts from (usually <c>0</c>).</summary>
    public int IndexFrom { get; }

    /// <inheritdoc />
    public int PageIndex { get; }

    /// <inheritdoc />
    public int PageSize { get; }

    /// <inheritdoc />
    public int TotalCount { get; }

    /// <inheritdoc />
    public int TotalPages { get; }

    /// <inheritdoc />
    public IList<T> Items { get; }

    /// <inheritdoc />
    public bool HasPreviousPage => PageIndex - IndexFrom > 0;

    /// <inheritdoc />
    public bool HasNextPage => PageIndex - IndexFrom + 1 < TotalPages;
}
