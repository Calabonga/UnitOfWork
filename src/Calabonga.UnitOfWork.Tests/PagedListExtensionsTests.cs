using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Calabonga.PagedListCore;
using Calabonga.UnitOfWork.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Calabonga.UnitOfWork.Tests;

public sealed class PagedListExtensionsTests : DatabaseTestBase
{
    [Fact]
    public void ToPagedList_OnEnumerable_ProducesRequestedPage()
    {
        // Calabonga.PagedListCore 2.0.0: the pageIndex argument is 1-based for selecting
        // items (page 2 -> items 11..20), but the resulting PageIndex property is stored
        // 0-based (argument - 1).
        var page = Enumerable.Range(1, 25).ToPagedList(pageIndex: 2, pageSize: 10);

        Assert.Equal(25, page.TotalCount);
        Assert.Equal(1, page.PageIndex);
        Assert.Equal(10, page.PageSize);
        Assert.Equal(3, page.TotalPages);
        Assert.Equal(10, page.Items.Count);
        Assert.Equal(11, page.Items[0]);
    }

    [Fact]
    public void ToPagedList_WithConverter_MapsEachItem()
    {
        IPagedList<int> source = Enumerable.Range(1, 5).ToPagedList(0, 10);

        var converted = source.ToPagedList(items => items.Select(x => $"#{x}"));

        Assert.Equal(5, converted.TotalCount);
        Assert.Equal("#1", converted.Items[0]);
    }

    [Fact]
    public async Task ToPagedListAsync_OnQueryable_ProducesRequestedPage()
    {
        var page = await Context.Products
            .OrderBy(x => x.Id)
            .ToPagedListAsync(pageIndex: 0, pageSize: 4, cancellationToken: CancellationToken.None);

        Assert.Equal(6, page.TotalCount);
        Assert.Equal(4, page.Items.Count);
        Assert.True(page.HasNextPage);
        Assert.False(page.HasPreviousPage);
    }

    [Fact]
    public async Task ToPagedListAsync_WhenIndexFromGreaterThanPageIndex_Throws()
        => await Assert.ThrowsAsync<ArgumentException>(
            () => Context.Products.ToPagedListAsync(pageIndex: 0, pageSize: 5, indexFrom: 2));
}
