using System;
using System.Linq;
using System.Threading.Tasks;
using Calabonga.UnitOfWork.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Calabonga.UnitOfWork.Tests;

public sealed class UnitOfWorkTests : DatabaseTestBase
{
    private UnitOfWork<TestDbContext> CreateSut() => new(Context);

    [Fact]
    public void GetRepository_SameEntityType_ReturnsCachedInstance()
    {
        var sut = CreateSut();

        var first = sut.GetRepository<Product>();
        var second = sut.GetRepository<Product>();

        Assert.Same(first, second);
    }

    [Fact]
    public void GetRepository_DifferentEntityTypes_ReturnDifferentInstances()
    {
        var sut = CreateSut();

        object products = sut.GetRepository<Product>();
        object categories = sut.GetRepository<Category>();

        Assert.NotSame(products, categories);
    }

    [Fact]
    public void Result_BeforeAnySave_IsOk()
    {
        var sut = CreateSut();

        Assert.True(sut.Result.Ok);
        Assert.Null(sut.Result.Exception);
    }

    [Fact]
    public void SaveChanges_OnSuccess_ReturnsAffectedCount()
    {
        var sut = CreateSut();
        sut.GetRepository<Product>().Insert(new Product { Name = "New", Price = 1m, CategoryId = 1 });

        var affected = sut.SaveChanges();

        Assert.Equal(1, affected);
        Assert.True(sut.Result.Ok);
    }

    [Fact]
    public void SaveChanges_OnException_SwallowsAndRecordsError()
    {
        var sut = CreateSut();
        sut.GetRepository<Product>().Insert(new Product { Name = null!, Price = 1m, CategoryId = 1 });

        var affected = sut.SaveChanges();

        Assert.Equal(0, affected);
        Assert.False(sut.Result.Ok);
        Assert.NotNull(sut.Result.Exception);
    }

    [Fact]
    public async Task SaveChangesAsync_OnException_SwallowsAndRecordsError()
    {
        var sut = CreateSut();
        sut.GetRepository<Product>().Insert(new Product { Name = null!, Price = 1m, CategoryId = 1 });

        var affected = await sut.SaveChangesAsync();

        Assert.Equal(0, affected);
        Assert.NotNull(sut.Result.Exception);
    }

    [Fact]
    public async Task SaveChangesAsync_WithAdditionalUnitsOfWork_AggregatesCounts()
    {
        var sut = CreateSut();
        var secondaryContext = CreateSecondaryContext();
        var secondary = new UnitOfWork<TestDbContext>(secondaryContext);

        sut.GetRepository<Product>().Insert(new Product { Name = "one", Price = 1m, CategoryId = 1 });
        secondary.GetRepository<Product>().Insert(new Product { Name = "two", Price = 2m, CategoryId = 1 });

        var total = await sut.SaveChangesAsync(secondary);

        Assert.Equal(2, total);
    }

    [Fact]
    public void BeginTransaction_WithUseIfExists_ReturnsCurrentTransaction()
    {
        var sut = CreateSut();

        using var first = sut.BeginTransaction();
        var second = sut.BeginTransaction(useIfExists: true);

        Assert.Same(first, second);
    }

    [Fact]
    public void SetAutoDetectChanges_TogglesChangeTrackerFlag()
    {
        var sut = CreateSut();

        sut.SetAutoDetectChanges(false);

        Assert.False(Context.ChangeTracker.AutoDetectChangesEnabled);
    }

    [Fact]
    public void TrackGraph_AppliesCallbackStateToRoot()
    {
        var sut = CreateSut();
        var graph = new Product { Id = 999, Name = "Detached", CategoryId = 1 };

        sut.TrackGraph(graph, node => node.Entry.State = EntityState.Added);

        Assert.Equal(EntityState.Added, Context.Entry(graph).State);
    }

    [Fact]
    public void ExecuteSqlCommand_ReturnsAffectedRowCount()
    {
        var sut = CreateSut();

        var affected = sut.ExecuteSqlCommand("UPDATE Products SET Stock = 0 WHERE CategoryId = {0}", 1);

        Assert.Equal(5, affected);
    }

    [Fact]
    public async Task ExecuteSqlCommandAsync_ReturnsAffectedRowCount()
    {
        var sut = CreateSut();

        var affected = await sut.ExecuteSqlCommandAsync("DELETE FROM Products WHERE Stock = {0}", 0);

        Assert.Equal(1, affected);
    }

    [Fact]
    public void FromSqlRaw_ReturnsMappedEntities()
    {
        var sut = CreateSut();

        var rows = sut.FromSqlRaw<Product>("SELECT * FROM Products WHERE Stock = {0}", 0).ToList();

        Assert.Single(rows);
        Assert.Equal("Cable", rows[0].Name);
    }

    [Fact]
    public void FromSql_Interpolated_ReturnsMappedEntities()
    {
        var sut = CreateSut();
        var name = "Mouse";

        var rows = sut.FromSql<Product>($"SELECT * FROM Products WHERE Name = {name}").ToList();

        Assert.Single(rows);
    }

    [Fact]
    public void FromSqlRawInterpolated_ReturnsMappedEntities()
    {
        var sut = CreateSut();
        var name = "Monitor";

        var rows = sut.FromSqlRawInterpolated<Product>($"SELECT * FROM Products WHERE Name = {name}").ToList();

        Assert.Single(rows);
    }

    [Fact]
    public void Dispose_DisposesUnderlyingContext()
    {
        var secondaryContext = CreateSecondaryContext();
        var sut = new UnitOfWork<TestDbContext>(secondaryContext);

        sut.Dispose();

        Assert.Throws<ObjectDisposedException>(() => secondaryContext.Products.Any());
    }
}
