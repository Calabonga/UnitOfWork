using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Calabonga.UnitOfWork.Tests.Infrastructure;

/// <summary>
/// Base class that spins up an isolated SQLite in-memory database per test method
/// (xUnit creates a new instance of the test class for every test), applies the
/// schema and seeds a deterministic data set.
/// </summary>
public abstract class DatabaseTestBase : IDisposable
{
    private readonly SqliteConnection _connection;

    protected DatabaseTestBase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new TestDbContext(options);
        Context.Database.EnsureCreated();
        Seed(Context);
        Context.ChangeTracker.Clear();
    }

    /// <summary>Shared context for the current test.</summary>
    protected TestDbContext Context { get; }

    /// <summary>
    /// Creates an additional context over the SAME in-memory database. Useful for
    /// assertions that must read state through a fresh change-tracker.
    /// </summary>
    protected TestDbContext CreateSecondaryContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(_connection)
            .Options;

        return new TestDbContext(options);
    }

    protected static IReadOnlyList<Product> SeededActiveProducts { get; private set; } = [];

    private static void Seed(TestDbContext context)
    {
        var electronics = new Category { Id = 1, Name = "Electronics", IsDeleted = false };
        var archive = new Category { Id = 2, Name = "Archive", IsDeleted = true };

        var products = new List<Product>
        {
            new() { Id = 1, Name = "Mouse", Price = 25.00m, Stock = 100, CategoryId = 1 },
            new() { Id = 2, Name = "Keyboard", Price = 45.50m, Stock = 50, CategoryId = 1 },
            new() { Id = 3, Name = "Monitor", Price = 199.99m, Stock = 10, CategoryId = 1 },
            new() { Id = 4, Name = "Cable", Price = 5.00m, Stock = 0, CategoryId = 1 },
            new() { Id = 5, Name = "Webcam", Price = 75.00m, Stock = 5, CategoryId = 1 },
            new() { Id = 6, Name = "Old Phone", Price = 10.00m, Stock = 1, CategoryId = 2 }
        };

        context.Categories.AddRange(electronics, archive);
        context.Products.AddRange(products);
        context.SaveChanges();

        SeededActiveProducts = products.FindAll(x => x.CategoryId == 1);
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}
