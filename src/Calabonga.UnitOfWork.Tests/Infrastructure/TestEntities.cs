using System.Collections.Generic;

namespace Calabonga.UnitOfWork.Tests.Infrastructure;

/// <summary>
/// Root entity with a navigation collection and a global query filter (soft-delete).
/// </summary>
public sealed class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }

    public List<Product> Products { get; set; } = [];
}

/// <summary>
/// Child entity used for aggregate, projection and paging assertions.
/// </summary>
public sealed class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;
}

/// <summary>
/// Lightweight projection target (must be a reference type for <c>GetPagedList&lt;TResult&gt;</c>).
/// </summary>
public sealed record ProductDto(int Id, string Name, decimal Price);
