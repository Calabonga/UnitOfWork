using System;

namespace Calabonga.UnitOfWork;

public interface IUnitOfWorkFactory<out TContext> : IUnitOfWorkFactory
{
    TContext DbContext { get; }
}

public interface IUnitOfWorkFactory : IDisposable
{
    IUnitOfWork CreateUnitOfWork();
}