## Правила рабочего процесса (проект Calabonga.UnitOfWork)

- Всегда создавай отдельную ветку Git перед внесением изменений.
  `master` — основная ветка, публикуется в NuGet через GitHub Actions при push.
- Допустимые префиксы веток: `feature/`, `bugfix/`, `hotfix/`.
- Формат коммитов: `type: description`
  (`feat`, `fix`, `refactor`, `test`, `docs`, `style`, `perf`, `build`, `chore`, `revert`).
- Атомарные коммиты — одно логическое изменение на коммит.
- Перед созданием нового класса проверь, нет ли файла с таким же именем в решении
  (`src/Calabonga.UnitOfWork` и `src/Calabonga.UnitOfWork.Tests`).
- Сборка и тесты:
  - `dotnet build src/Calabonga.UnitOfWork.sln -c Release`
  - `dotnet test src/Calabonga.UnitOfWork.sln` — запускай после каждой реализации
    и обязательно перед фиксацией изменений.
- Изменения публичного API `IRepository<TEntity>` / `IUnitOfWork` — только
  аддитивные (новые перегрузки). Ломающие сигнатуры — отдельная major-версия и
  запись в changelog.
- Версию пакета (`<Version>` в `Calabonga.UnitOfWork.csproj`) и changelog в
  `README.md` (двуязычный, RU + EN) обновляй в том же PR, что и функциональные
  изменения.
- Тестовый проект в NuGet-пакет не входит и в CI-workflow публикации не участвует.
