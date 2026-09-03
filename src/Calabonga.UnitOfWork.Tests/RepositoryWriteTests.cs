using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Calabonga.UnitOfWork.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Calabonga.UnitOfWork.Tests;

public sealed class RepositoryWriteTests : DatabaseTestBase
{
    private Repository<Product> ProductRepository => new(Context);

    [Fact]
    public void Insert_ThenSaveChanges_PersistsEntity()
    {
        var entity = ProductRepository.Insert(new Product { Name = "Headset", Price = 60m, Stock = 3, CategoryId = 1 });
        Context.SaveChanges();

        Assert.True(entity.Id > 0);
        Assert.Equal(7, ProductRepository.Count());
    }

    [Fact]
    public async Task InsertAsync_Range_PersistsAllEntities()
    {
        await ProductRepository.InsertAsync(
            [
                new Product { Name = "A", Price = 1m, CategoryId = 1 },
                new Product { Name = "B", Price = 2m, CategoryId = 1 }
            ],
            CancellationToken.None);
        await Context.SaveChangesAsync();

        Assert.Equal(8, await ProductRepository.CountAsync());
    }

    [Fact]
    public void Update_ChangesPersistedValues()
    {
        var product = ProductRepository.GetFirstOrDefault(predicate: x => x.Id == 1, trackingType: TrackingType.Tracking)!;
        product.Price = 999m;

        ProductRepository.Update(product);
        Context.SaveChanges();
        Context.ChangeTracker.Clear();

        Assert.Equal(999m, ProductRepository.GetFirstOrDefault(predicate: x => x.Id == 1)!.Price);
    }

    [Fact]
    public void Delete_ByEntity_RemovesRow()
    {
        var product = ProductRepository.Find(5)!;

        ProductRepository.Delete(product);
        Context.SaveChanges();

        Assert.Null(ProductRepository.Find(5));
    }

    [Fact]
    public void Delete_ById_RemovesRow()
    {
        ProductRepository.Delete(4);
        Context.SaveChanges();
        Context.ChangeTracker.Clear();

        Assert.Null(ProductRepository.Find(4));
    }

    [Fact]
    public void Delete_Range_RemovesAllGivenRows()
    {
        var products = ProductRepository.GetAll(predicate: x => x.CategoryId == 1, trackingType: TrackingType.Tracking).ToArray();

        ProductRepository.Delete(products);
        Context.SaveChanges();

        Assert.Equal(1, ProductRepository.Count());
    }

    [Fact]
    public void ChangeEntityState_SetsTrackedState()
    {
        var product = ProductRepository.Find(1)!;

        ProductRepository.ChangeEntityState(product, EntityState.Modified);

        Assert.Equal(EntityState.Modified, Context.Entry(product).State);
    }

    [Fact]
    public void ExecuteUpdate_AffectsWholeSet_AndReturnsRowCount()
    {
        var affected = ProductRepository.ExecuteUpdate(s => s.SetProperty(x => x.Stock, 0));

        Assert.Equal(6, affected);
        Context.ChangeTracker.Clear();
        Assert.All(ProductRepository.GetAll(TrackingType.NoTracking), x => Assert.Equal(0, x.Stock));
    }

    [Fact]
    public async Task ExecuteUpdateAsync_AffectsWholeSet_AndReturnsRowCount()
    {
        var affected = await ProductRepository.ExecuteUpdateAsync(
            s => s.SetProperty(x => x.Price, x => x.Price * 2),
            CancellationToken.None);

        Assert.Equal(6, affected);
    }

    [Fact]
    public void ExecuteDelete_AffectsWholeSet_AndReturnsRowCount()
    {
        var affected = ProductRepository.ExecuteDelete();

        Assert.Equal(6, affected);
        Context.ChangeTracker.Clear();
        Assert.Equal(0, ProductRepository.Count());
    }

    [Fact]
    public async Task ExecuteDeleteAsync_AffectsWholeSet_AndReturnsRowCount()
    {
        var affected = await ProductRepository.ExecuteDeleteAsync(CancellationToken.None);

        Assert.Equal(6, affected);
    }

    [Fact]
    public void FromSql_WithParameter_ReturnsMatchingRows()
    {
        var rows = ProductRepository.FromSql("SELECT * FROM Products WHERE CategoryId = {0}", 1).ToList();

        Assert.Equal(5, rows.Count);
    }
}
