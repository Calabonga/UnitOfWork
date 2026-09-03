using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Calabonga.UnitOfWork.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Calabonga.UnitOfWork.Tests;

public sealed class RepositoryReadTests : DatabaseTestBase
{
    private Repository<Product> ProductRepository => new(Context);

    private Repository<Category> CategoryRepository => new(Context);

    [Fact]
    public void GetAll_NoArguments_ReturnsEveryRow()
    {
        var all = ProductRepository.GetAll(TrackingType.NoTracking).ToList();

        Assert.Equal(6, all.Count);
    }

    [Fact]
    public void GetAll_WithPredicate_FiltersRows()
    {
        var result = ProductRepository.GetAll(predicate: x => x.CategoryId == 1).ToList();

        Assert.Equal(5, result.Count);
        Assert.All(result, x => Assert.Equal(1, x.CategoryId));
    }

    [Fact]
    public void GetAll_WithOrderByAndSelector_ProjectsAndOrders()
    {
        var names = ProductRepository
            .GetAll(
                selector: x => x.Name,
                predicate: x => x.CategoryId == 1,
                orderBy: q => q.OrderByDescending(x => x.Price))
            .ToList();

        Assert.Equal("Monitor", names[0]);
        Assert.Equal("Cable", names[^1]);
    }

    [Fact]
    public void GetAll_NoTracking_DoesNotTrackEntities()
    {
        _ = ProductRepository.GetAll(predicate: x => x.Id == 1).ToList();

        Assert.Empty(Context.ChangeTracker.Entries<Product>());
    }

    [Fact]
    public void GetAll_Tracking_TracksEntities()
    {
        _ = ProductRepository.GetAll(predicate: x => x.Id == 1, trackingType: TrackingType.Tracking).ToList();

        Assert.Single(Context.ChangeTracker.Entries<Product>());
    }

    [Fact]
    public async Task GetAllAsync_WithSelector_ReturnsProjectedList()
    {
        var list = await ProductRepository.GetAllAsync(
            selector: x => new ProductDto(x.Id, x.Name, x.Price),
            predicate: x => x.CategoryId == 1,
            orderBy: q => q.OrderBy(x => x.Id));

        Assert.Equal(5, list.Count);
        Assert.Equal("Mouse", list[0].Name);
    }

    [Fact]
    public void GetFirstOrDefault_WithInclude_LoadsNavigation()
    {
        var product = ProductRepository.GetFirstOrDefault(
            predicate: x => x.Id == 1,
            include: q => q.Include(x => x.Category));

        Assert.NotNull(product);
        Assert.NotNull(product!.Category);
        Assert.Equal("Electronics", product.Category.Name);
    }

    [Fact]
    public void GetFirstOrDefault_NoMatch_ReturnsNull()
        => Assert.Null(ProductRepository.GetFirstOrDefault(predicate: x => x.Name == "does-not-exist"));

    [Fact]
    public void GetFirstOrDefault_WithSelector_ReturnsProjection()
    {
        var dto = ProductRepository.GetFirstOrDefault(
            selector: x => new ProductDto(x.Id, x.Name, x.Price),
            predicate: x => x.Name == "Keyboard");

        Assert.NotNull(dto);
        Assert.Equal(45.50m, dto!.Price);
    }

    [Fact]
    public async Task GetFirstOrDefaultAsync_WithOrderBy_ReturnsExpectedRow()
    {
        var cheapest = await ProductRepository.GetFirstOrDefaultAsync(
            predicate: x => x.CategoryId == 1,
            orderBy: q => q.OrderBy(x => x.Price));

        Assert.NotNull(cheapest);
        Assert.Equal("Cable", cheapest!.Name);
    }

    [Fact]
    public void GetPagedList_FirstPage_HasExpectedMetadata()
    {
        // Paging is zero-based and consistent between the sync and async paths:
        // pageIndex 0 == first page.
        var page = ProductRepository.GetPagedList(
            predicate: x => x.CategoryId == 1,
            orderBy: q => q.OrderBy(x => x.Id),
            pageIndex: 0,
            pageSize: 2);

        Assert.Equal(5, page.TotalCount);
        Assert.Equal(2, page.PageSize);
        Assert.Equal(0, page.PageIndex);
        Assert.Equal(3, page.TotalPages);
        Assert.Equal(2, page.Items.Count);
        Assert.Equal("Mouse", page.Items[0].Name);
        Assert.False(page.HasPreviousPage);
        Assert.True(page.HasNextPage);
    }

    [Fact]
    public void GetPagedList_LastPage_HasNoNextPage()
    {
        // 5 rows, page size 2 -> zero-based pages 0, 1, 2; page 2 is the last.
        var page = ProductRepository.GetPagedList(
            predicate: x => x.CategoryId == 1,
            orderBy: q => q.OrderBy(x => x.Id),
            pageIndex: 2,
            pageSize: 2);

        Assert.Single(page.Items);
        Assert.Equal("Webcam", page.Items[0].Name);
        Assert.True(page.HasPreviousPage);
        Assert.False(page.HasNextPage);
    }

    [Fact]
    public void GetPagedList_MiddlePage_HasBothPreviousAndNextPage()
    {
        var page = ProductRepository.GetPagedList(
            predicate: x => x.CategoryId == 1,
            orderBy: q => q.OrderBy(x => x.Id),
            pageIndex: 1,
            pageSize: 2);

        Assert.Equal(1, page.PageIndex);
        Assert.Equal("Monitor", page.Items[0].Name);
        Assert.True(page.HasPreviousPage);
        Assert.True(page.HasNextPage);
    }

    [Fact]
    public async Task GetPagedList_SyncAndAsync_ProduceSameMetadataForSamePage()
    {
        var sync = ProductRepository.GetPagedList(
            predicate: x => x.CategoryId == 1,
            orderBy: q => q.OrderBy(x => x.Id),
            pageIndex: 1,
            pageSize: 2);

        var async = await ProductRepository.GetPagedListAsync(
            predicate: x => x.CategoryId == 1,
            orderBy: q => q.OrderBy(x => x.Id),
            pageIndex: 1,
            pageSize: 2);

        Assert.Equal(sync.PageIndex, async.PageIndex);
        Assert.Equal(sync.TotalPages, async.TotalPages);
        Assert.Equal(sync.HasPreviousPage, async.HasPreviousPage);
        Assert.Equal(sync.HasNextPage, async.HasNextPage);
        Assert.Equal(
            sync.Items.Select(x => x.Id),
            async.Items.Select(x => x.Id));
    }

    [Fact]
    public async Task GetPagedListAsync_WithProjection_ReturnsProjectedPage()
    {
        var page = await ProductRepository.GetPagedListAsync(
            selector: x => new ProductDto(x.Id, x.Name, x.Price),
            predicate: x => x.CategoryId == 1,
            orderBy: q => q.OrderBy(x => x.Id),
            pageIndex: 0,
            pageSize: 3);

        Assert.Equal(5, page.TotalCount);
        Assert.Equal(3, page.Items.Count);
        Assert.All(page.Items, x => Assert.IsType<ProductDto>(x));
    }

    [Fact]
    public void Count_AppliesGlobalQueryFilter()
        => Assert.Equal(1, CategoryRepository.Count());

    [Fact]
    public void GetAll_IgnoreQueryFilters_ReturnsSoftDeletedRows()
        => Assert.Equal(2, CategoryRepository.GetAll(ignoreQueryFilters: true).Count());

    [Fact]
    public void Count_WithPredicate_CountsMatches()
        => Assert.Equal(5, ProductRepository.Count(x => x.CategoryId == 1));

    [Fact]
    public async Task CountAsync_NoPredicate_CountsAll()
        => Assert.Equal(6, await ProductRepository.CountAsync());

    [Fact]
    public void LongCount_NoPredicate_CountsAll()
        => Assert.Equal(6L, ProductRepository.LongCount());

    [Fact]
    public async Task LongCountAsync_WithPredicate_CountsMatches()
        => Assert.Equal(1L, await ProductRepository.LongCountAsync(x => x.CategoryId == 2));

    [Theory]
    [InlineData("Mouse", true)]
    [InlineData("Nonexistent", false)]
    public void Exists_ReflectsPresence(string name, bool expected)
        => Assert.Equal(expected, ProductRepository.Exists(x => x.Name == name));

    [Fact]
    public async Task ExistsAsync_NoPredicate_TrueWhenTableNotEmpty()
        => Assert.True(await ProductRepository.ExistsAsync());

    [Fact]
    public void Max_WithPredicate_ReturnsLargestValue()
        => Assert.Equal(199.99m, ProductRepository.Max(x => x.Price, x => x.CategoryId == 1));

    [Fact]
    public async Task MinAsync_WithPredicate_ReturnsSmallestValue()
        => Assert.Equal(5.00m, await ProductRepository.MinAsync(x => x.Price, x => x.CategoryId == 1));

    [Fact]
    public void Sum_WithPredicate_ReturnsTotal()
        => Assert.Equal(350.49m, ProductRepository.Sum(x => x.Price, x => x.CategoryId == 1), 2);

    [Fact]
    public async Task AverageAsync_WithPredicate_ReturnsMean()
        => Assert.Equal(70.098m, await ProductRepository.AverageAsync(x => x.Price, x => x.CategoryId == 1), 3);

    [Fact]
    public void Find_ByPrimaryKey_ReturnsEntity()
    {
        var product = ProductRepository.Find(3);

        Assert.NotNull(product);
        Assert.Equal("Monitor", product!.Name);
    }

    [Fact]
    public async Task FindAsync_WithCancellationToken_ReturnsEntity()
    {
        var product = await ProductRepository.FindAsync([2], CancellationToken.None);

        Assert.NotNull(product);
        Assert.Equal("Keyboard", product!.Name);
    }
}
