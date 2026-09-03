## Рекомендации по языку C# и стилю (проект Calabonga.UnitOfWork)

Документ адаптирован под этот репозиторий: это **библиотека-NuGet** (`net10.0`,
EF Core 10), без Blazor, без ASP.NET, без собственного `DbContext`. Разделы,
неприменимые к библиотеке, помечены отдельно.

### Современный синтаксис
- `file-scoped namespaces` (уже используется во всех файлах).
- Глобальные / неявные `using` — **не включены** в `.csproj`; `using`-директивы
  указываются явно в каждом файле. Не добавляй `<ImplicitUsings>` без запроса.
- Primary constructors допустимы для новых внутренних типов; публичные классы
  библиотеки (`UnitOfWork<TContext>`, `Repository<TEntity>`) менять не нужно.

### Безопасность нулевых значений
- `<Nullable>enable</Nullable>` включён во всех проектах решения.
- Не используй `!` (null-forgiving) без комментария с обоснованием.
- Необязательные параметры фильтров/селекторов объявляются как
  `Expression<Func<TEntity, bool>>? predicate = null` — сохраняй этот стиль.

### Неизменяемость
- `record` / `readonly record struct` — для DTO и результатов в тестах и в новых
  вспомогательных типах.
- Существующий `SaveChangesResult` — обычный `sealed class` по историческим
  причинам, не переписывай в `record` без отдельной задачи.

### Стиль написания кода
- `var` для локальных переменных, когда тип очевиден справа.
- `nameof()` вместо строковых литералов.
- `is` / `as` вместо приведения типов; паттерн-матчинг там, где читаемо.
- `switch`-выражения вместо `switch`-операторов и цепочек `if-else`
  (см. `trackingType switch { ... }` в `Repository<TEntity>` — эталон стиля).
- `try-catch` только для ожидаемых ошибок, не для управления потоком.
  Единственное намеренное исключение — `UnitOfWork.SaveChanges*`, где исключение
  ловится и кладётся в `Result.Exception` (исторический контракт, не трогать).
- Явные модификаторы доступа у всех членов; `private` по умолчанию.
- `sealed` по умолчанию для новых классов, не предназначенных к наследованию.
  `UnitOfWork<TContext>` и `Repository<TEntity>` обязаны оставаться `sealed`.
- Порядок членов класса:
  1. `private readonly` поля — в начале;
  2. конструкторы;
  3. публичные свойства и методы;
  4. приватные (вспомогательные) методы;
  5. `override`-методы;
  6. `Dispose()` / `DisposeAsync()` — в самом конце.
- Публичный API библиотеки документируется XML-комментариями (`<summary>`,
  `<param>`, `<returns>`). Проект пакует doc-файл — при касании метода без
  документации добавь её.

> Blazor-специфичные правила (`[Inject]`/`[Parameter]`, `protected`-методы
> компонентов и т.п.) в этом репозитории **неприменимы**.

### Асинхронный поток
- Суффикс `Async` у всех асинхронных методов.
- `ConfigureAwait(false)` во всей библиотечной логике (см.
  `QueryablePageListExtensions.ToPagedListAsync` — эталон). В тестах
  `ConfigureAwait` не обязателен.
- `CancellationToken` передаётся во все асинхронные методы. Новые async-перегрузки
  добавляй с `CancellationToken cancellationToken = default` и пробрасывай его в
  вызовы EF Core.

### Обработка ошибок
- Fail Fast: проверка аргументов через `ArgumentNullException.ThrowIfNull` /
  `throw new ArgumentOutOfRangeException(...)` в начале метода (см. конструкторы
  `Repository` / `UnitOfWork`).
- Пакет `Calabonga.Results` **не является зависимостью** этого проекта — не
  добавляй его. Для результата операции сохранения используется существующий
  `SaveChangesResult`.

### Правила работы с EF Core
- Чтение по умолчанию — без трекинга. В библиотеке это выражается параметром
  `TrackingType` (`NoTracking` по умолчанию), а не прямым `AsNoTracking()`
  в каждом методе. Новые read-методы обязаны принимать
  `TrackingType trackingType = TrackingType.NoTracking` и применять его через
  тот же `switch`, что и остальные методы `Repository`.
- Навигационные свойства подключаются делегатом
  `Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include`.
- `IDbContextFactory<TContext>` используется только в `UnitOfWorkFactory<TContext>`
  для сценариев вне scoped-DI; основной путь — `DbContext` через конструктор.
- Порядок построения запроса в `Repository`: tracking → include → where →
  ignoreQueryFilters → ignoreAutoIncludes → orderBy → (select) → материализация.
  Соблюдай его в новых методах.

### Время / часы
- Для нового кода, которому нужно текущее время, внедряй `TimeProvider`, а не
  `DateTime.UtcNow`. В текущем коде библиотеки работы со временем нет.

### Тесты
- Каждый публичный метод `IRepository<TEntity>` и `IUnitOfWork` покрывается
  unit-тестами (успешный путь + граничные случаи: `null` predicate, пустой набор,
  пагинация за пределами данных).
- Проект тестов: `src/Calabonga.UnitOfWork.Tests` (xUnit v3).
- Провайдер БД для тестов — **SQLite in-memory** (`Microsoft.EntityFrameworkCore.Sqlite`
  с соединением `DataSource=:memory:`), а не InMemory-провайдер: нужны реальная
  трансляция LINQ→SQL, `ExecuteUpdate/ExecuteDelete`, `FromSqlRaw`, query filters.
- Именование тестов: `Method_Scenario_ExpectedResult`.
- Схема AAA (Arrange / Act / Assert), одна проверяемая идея на тест.
- Общая инфраструктура (создание `DbContext`, сидинг) — через `IClassFixture<>`
  или базовый класс с `IDisposable`/`IAsyncLifetime`.
