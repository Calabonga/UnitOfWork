using Calabonga.UnitOfWork.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Calabonga.UnitOfWork.Tests;

public sealed class ServiceCollectionExtensionsTests
{
    private static ServiceCollection CreateServicesWithDbContext()
    {
        var services = new ServiceCollection();
        services.AddDbContext<TestDbContext>(options => options.UseSqlite("DataSource=:memory:"));
        return services;
    }

    [Fact]
    public void AddUnitOfWork_RegistersCoreAbstractions()
    {
        var services = CreateServicesWithDbContext();
        services.AddUnitOfWork<TestDbContext>();

        using var provider = services.BuildServiceProvider(validateScopes: true);
        using var scope = provider.CreateScope();

        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IUnitOfWork>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IUnitOfWork<TestDbContext>>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IRepositoryFactory>());
    }

    [Fact]
    public void AddUnitOfWork_WithSingletonLifetime_RegistersSingletonDescriptor()
    {
        var services = CreateServicesWithDbContext();
        services.AddUnitOfWork<TestDbContext>(ServiceLifetime.Singleton);

        Assert.Contains(services, descriptor =>
            descriptor.ServiceType == typeof(IUnitOfWork<TestDbContext>)
            && descriptor.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddUnitOfWorkFactory_ResolvesFactoryThatCreatesUsableUnitOfWork()
    {
        var services = new ServiceCollection();
        services.AddDbContextFactory<TestDbContext>(options => options.UseSqlite("DataSource=:memory:"));
        services.AddUnitOfWorkFactory<TestDbContext>();

        using var provider = services.BuildServiceProvider(validateScopes: true);
        using var scope = provider.CreateScope();

        var factory = scope.ServiceProvider.GetRequiredService<IUnitOfWorkFactory>();
        using var unitOfWork = factory.CreateUnitOfWork();

        Assert.NotNull(unitOfWork.GetRepository<Product>());
    }

    [Fact]
    public void AddCustomRepository_ResolvesProvidedImplementation()
    {
        var services = CreateServicesWithDbContext();
        services.AddCustomRepository<Product, StubProductRepository>();

        using var provider = services.BuildServiceProvider(validateScopes: true);
        using var scope = provider.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IRepository<Product>>();

        Assert.IsType<StubProductRepository>(repository);
    }
}
