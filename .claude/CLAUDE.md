# CLAUDE.md

Guidance for Claude Code when working in this repository.

## Что это

`Calabonga.UnitOfWork` — NuGet-пакет с реализацией паттернов **Unit of Work** и
**Repository** поверх Entity Framework Core. Один проект-библиотека, без тестов и
примеров в решении.

- Solution: `src/Calabonga.UnitOfWork.sln`
- Проект пакета: `src/Calabonga.UnitOfWork/Calabonga.UnitOfWork.csproj`
- Проект тестов: `src/Calabonga.UnitOfWork.Tests/Calabonga.UnitOfWork.Tests.csproj`
  (xUnit v3, SQLite in-memory; `IsPackable=false`, в NuGet-пакет и в workflow
  публикации не входит).
- TFM: `net10.0`
- Зависимости: `Microsoft.EntityFrameworkCore` 10.0.1,
  `Microsoft.EntityFrameworkCore.Relational` 10.0.1, `Calabonga.PagedListCore` 2.0.0
- `Nullable` включён; `GeneratePackageOnBuild=true` (при каждой сборке Release
  создаётся `.nupkg` + `.snupkg`).
- Публикация в NuGet — GitHub Actions `.github/workflows/main.yml` при push в `master`.

## Команды

```bash
dotnet restore src/Calabonga.UnitOfWork.sln
dotnet build   src/Calabonga.UnitOfWork.sln -c Release
dotnet test    src/Calabonga.UnitOfWork.sln
dotnet pack    src/Calabonga.UnitOfWork.sln -c Release -o ./artifacts
```

`dotnet test` работает через Microsoft.Testing.Platform (xUnit v3 + .NET 10 SDK).
Opt-in задан в `global.json` (секция `test.runner`) — не удалять, иначе
`dotnet test` падает с ошибкой про VSTest.

## Правила

Помимо этого файла соблюдай `.claude/rules/`:
- `rules/code-styles.md` — стиль C#, соглашения `Repository`/`UnitOfWork`,
  требования к тестам.
- `rules/workflow.md` — ветки, коммиты, порядок «реализация → `dotnet test` →
  коммит».

## Архитектура

| Файл | Роль |
|------|------|
| `IUnitOfWork.cs` / `UnitOfWork.cs` | `IUnitOfWork`, `IUnitOfWork<TContext>`. Держит `DbContext`, кэш репозиториев (`Dictionary<Type, object>`), транзакции, raw SQL, `TrackGraph`, `SaveChanges*`. `sealed` — не снимать (см. комментарий в коде). |
| `IRepository.cs` / `Repository.cs` | Дженерик-репозиторий: `GetPagedList(Async)`, `GetFirstOrDefault(Async)`, `GetAll(Async)`, `Find(Async)`, `Insert/Update/Delete` (+ `ExecuteUpdate/ExecuteDelete`), агрегаты (`Count/LongCount/Exists/Min/Max/Average/Sum`), `FromSql`, `ChangeTable`, `ChangeEntityState`. `sealed`. |
| `IRepositoryFactory.cs` | `GetRepository<TEntity>(bool hasCustomRepository = false)`. Реализуется `UnitOfWork<TContext>`. |
| `IUnitOfWorkFactory.cs` / `UnitOfWorkFactory.cs` | Создание `IUnitOfWork` из `IDbContextFactory<TContext>` (для сценариев без scoped-DI). |
| `UnitOfWorkServiceCollectionExtensions.cs` | `AddUnitOfWork<TContext>(lifetime)`, перегрузки на 2/3/4 контекста, `AddCustomRepository<TEntity, TRepository>`. |
| `UnitOfWorkFactoryServiceCollectionExtensions.cs` | `AddUnitOfWorkFactory<TContext>(lifetime)`. |
| `TrackingType.cs` | enum `NoTracking` / `NoTrackingWithIdentityResolution` / `Tracking`. Заменил устаревший `bool disableTracking`. |
| `IQueryablePageListExtensions.cs` | `ToPagedListAsync` для `IQueryable<T>`. |
| `EnumerablePagedListExtensions.cs` | `ToPagedList` для `IEnumerable<T>` и конвертация `IPagedList`. |
| `SaveChangesResult.cs` | Результат `SaveChanges`: `Exception`, `Ok`. `SaveChanges*` в `UnitOfWork` **гасят** исключение и пишут его в `Result.Exception`, возвращая `0`. |
| `ExceptionHelper.cs` | Рекурсивный сбор `InnerException.Message`. |

### Соглашения

- Каждый read-метод репозитория принимает `predicate`, `orderBy`,
  `include` (`Func<IQueryable<T>, IIncludableQueryable<T, object>>`), `trackingType`,
  `ignoreQueryFilters`, `ignoreAutoIncludes`. Порядок применения в реализации:
  tracking → include → where → ignoreQueryFilters → ignoreAutoIncludes → orderBy.
- Проекционные перегрузки `<TResult>` требуют `where TResult : class` в
  `GetPagedList`, но не в `GetAll` — при правках соблюдать существующую сигнатуру.
- XML-документация обязательна для публичного API (пакет включает `IncludeSource`,
  генерирует doc-файл). Многие методы сейчас без `<summary>` — при касании
  добавлять.
- Пагинация: `pageIndex` начинается с 0, `pageSize` по умолчанию 20.
- Версия пакета правится вручную в `.csproj` (`<Version>`), changelog — в
  `README.md` (двуязычный, RU + EN).

## Что НЕ трогать без явной просьбы

- `sealed` на `UnitOfWork<TContext>` и `Repository<TEntity>`.
- Публичные сигнатуры методов `IRepository` / `IUnitOfWork` — это ломающие
  изменения для потребителей пакета. Только аддитивные перегрузки.
- Поведение `SaveChanges*` с проглатыванием исключений — исторический контракт.

## Известные слабые места (для контекста, не чинить молча)

- `Repository.ExecuteDelete()` / `ExecuteUpdate(...)` работают по **всему** `DbSet`
  без `predicate` — вызов удаляет/меняет все строки таблицы.
- `ExecuteUpdateAsync` не имеет значения по умолчанию для `CancellationToken`.
- `AddUnitOfWork<TContext>` после `switch (lifetime)` безусловно до-регистрирует
  сервисы как `Scoped` (дублирующий блок кода).
- `AddUnitOfWork<TContext1, TContext2>` регистрирует только `IUnitOfWork<T>`,
  не `IUnitOfWork` / `IRepositoryFactory`.
- `SaveChangesResult.Messages` — приватное, наружу не отдаётся.
- `SaveChangesAsync(params IUnitOfWork[])` заявлен как «distributed transaction»,
  но транзакции/`IExecutionStrategy` внутри нет — просто последовательные вызовы.
- `UnitOfWork` реализует только `IDisposable`, не `IAsyncDisposable`.
- Пагинация несогласована после апгрейда на `Calabonga.PagedListCore` 2.0.0:
  синхронный `Repository.GetPagedList` идёт через PagedListCore и трактует
  `pageIndex` как **1-based** (первая страница — `pageIndex: 1`), а в результате
  кладёт `PageIndex = pageIndex - 1`; асинхронный `GetPagedListAsync` использует
  локальный `QueryablePageListExtensions.ToPagedListAsync`, который **0-based**.
  Значения по умолчанию (`pageIndex = 0`) для синхронного пути дают
  `PageIndex = -1`. Зафиксировано тестами в `RepositoryReadTests` /
  `PagedListExtensionsTests`.
