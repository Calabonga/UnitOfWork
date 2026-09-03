using Calabonga.PagedListCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calabonga.UnitOfWork;

/// <summary>
/// Provides some extension methods for <see cref="IEnumerable{T}"/> to provide paging capability.
/// </summary>
public static class EnumerablePagedListExtensions
{
    /// <summary>
    /// Converts the specified source to <see cref="IPagedList{T}"/> by the specified <paramref name="pageIndex"/> and <paramref name="pageSize"/>.
    /// </summary>
    /// <typeparam name="T">The type of the source.</typeparam>
    /// <param name="source">The source to paging.</param>
    /// <param name="pageIndex">The zero-based index of the page.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="indexFrom">The start index value.</param>
    /// <returns>An instance of the inherited from <see cref="IPagedList{T}"/> interface.</returns>
    /// <remarks>Paging is zero-based: <paramref name="pageIndex"/> <c>0</c> returns the first page.</remarks>
    public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize, int indexFrom = 0)
    {
        if (indexFrom > pageIndex)
        {
            throw new ArgumentException(
                $"indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex");
        }

        var materialized = source as IReadOnlyCollection<T> ?? source.ToList();
        var items = materialized.Skip((pageIndex - indexFrom) * pageSize).Take(pageSize).ToList();

        return new PagedListResult<T>(items, pageIndex, pageSize, materialized.Count, indexFrom);
    }

    /// <summary>
    /// Converts the specified source to <see cref="IPagedList{T}"/> by the specified <paramref name="converter"/>, <paramref name="pageIndex"/> and <paramref name="pageSize"/>
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="source">The source to convert.</param>
    /// <param name="converter">The converter to change the <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
    /// <returns>An instance of the inherited from <see cref="IPagedList{T}"/> interface.</returns>
    public static IPagedList<TResult> ToPagedList<TSource, TResult>(
        this IPagedList<TSource> source,
        Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
        => new PagedListResult<TResult>(
            converter(source.Items).ToList(),
            source.PageIndex,
            source.PageSize,
            source.TotalCount,
            (source as PagedListResult<TSource>)?.IndexFrom ?? 0);
}
