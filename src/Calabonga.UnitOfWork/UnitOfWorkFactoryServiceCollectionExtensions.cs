using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Calabonga.UnitOfWork;

/// <summary>
/// Extension methods for setting up unit of work related services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class UnitOfWorkFactoryServiceCollectionExtensions
{
    /// <summary>
    /// Registers the unit of work given context as a service in the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TContext">The type of the db context.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="lifetime"></param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    /// <remarks>
    /// This method only support one db context, if been called more than once, will throw exception.
    /// </remarks>
    public static IServiceCollection AddUnitOfWorkFactory<TContext>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TContext : DbContext
    {
        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                services.TryAddSingleton<IRepositoryFactory, UnitOfWork<TContext>>();
                // Following has an issue: IUnitOfWork cannot support multiple dbContext/database, 
                // that means cannot call AddUnitOfWork<TContext> multiple times.
                // Solution: check IUnitOfWork whether or null
                services.TryAddSingleton<IUnitOfWork, UnitOfWork<TContext>>();
                services.TryAddSingleton<IUnitOfWork<TContext>, UnitOfWork<TContext>>();
                services.TryAddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory<TContext>>();
                services.TryAddSingleton<IUnitOfWorkFactory<TContext>, UnitOfWorkFactory<TContext>>();
                break;
            case ServiceLifetime.Scoped:
                services.TryAddScoped<IRepositoryFactory, UnitOfWork<TContext>>();
                // Following has an issue: IUnitOfWork cannot support multiple dbContext/database, 
                // that means cannot call AddUnitOfWork<TContext> multiple times.
                // Solution: check IUnitOfWork whether or null
                services.TryAddScoped<IUnitOfWork, UnitOfWork<TContext>>();
                services.TryAddScoped<IUnitOfWork<TContext>, UnitOfWork<TContext>>();
                services.TryAddScoped<IUnitOfWorkFactory, UnitOfWorkFactory<TContext>>();
                services.TryAddScoped<IUnitOfWorkFactory<TContext>, UnitOfWorkFactory<TContext>>();
                break;
            case ServiceLifetime.Transient:
                services.TryAddTransient<IRepositoryFactory, UnitOfWork<TContext>>();
                // Following has an issue: IUnitOfWork cannot support multiple dbContext/database, 
                // that means cannot call AddUnitOfWork<TContext> multiple times.
                // Solution: check IUnitOfWork whether or null
                services.TryAddTransient<IUnitOfWork, UnitOfWork<TContext>>();
                services.TryAddTransient<IUnitOfWork<TContext>, UnitOfWork<TContext>>();
                services.TryAddTransient<IUnitOfWorkFactory, UnitOfWorkFactory<TContext>>();
                services.TryAddTransient<IUnitOfWorkFactory<TContext>, UnitOfWorkFactory<TContext>>();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
        }

        services.AddScoped<IRepositoryFactory, UnitOfWork<TContext>>();
        // Following has an issue: IUnitOfWork cannot support multiple dbContext/database, 
        // that means cannot call AddUnitOfWork<TContext> multiple times.
        // Solution: check IUnitOfWork whether or null
        services.AddScoped<IUnitOfWork, UnitOfWork<TContext>>();
        services.AddScoped<IUnitOfWork<TContext>, UnitOfWork<TContext>>();

        return services;
    }

}