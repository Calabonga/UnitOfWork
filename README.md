# UnitOfWork

Реализация паттерна "Unit Of Work" для EntityFrameworkCore на платформе .NET. This is a Unit ofWork pattern implementation on .NET.

## Версии

### 6.1.0 2025-10-13

* FromSqlRawInterpolated implemented in IUnitOfWork from DbContext.

### 6.0.0 2025-03-05

* Deprecated methods were removed. Please use `TrackingType` parameter instead of `disableTracking`.
* New release `6.0.0` published.

### 5.0.0 2024-11-25

* Released new `5.0.0` version
* Deprecated methods will remove soon. Please use new overrides with `TrackingType` parameter.

### 5.0.0-beta.2 2024-11-23

* Добавлена поддержка фреймворка NET9.0 наряду с фреймворком NET8.0 (Added support for NET9.0 framework along with NET8.0 framework). Теперь в одном пакете поддержка двух версий.


### 5.0.0-beta.1 2024-11-03

* Созданые перегрузки для всех методов, где используется параметр `disableTracking` с целью дополнить новым способом управления слежением за изменениями. Теперь доступно выбрать один из вариантов:
  ``` csharp
  /// <summary>
  /// Changes Tracking Type for DbSet operations
  /// </summary>
  public enum TrackingType
  {
      NoTracking,
      NoTrackingWithIdentityResolution,
      Tracking
  }
  ```
  Если вы используте сборку `Calabonga.UnitOfWork` впервые, то никаких действий дополнительных не потребуется. А если вы обновляете сборку на проекте, где уже был использовано `Calabonga.UnitOfWork`, то для использования новых методов достаточно указать явно тип слежения за изменениями. Пример кода без использования явного типа слежения:
    ``` csharp
    public async Task<IEnumerable<PictureFile>> GetFilesForPostAsync(Guid postId, CancellationToken cancellationToken)
    {
        var maps = await _unitOfWork
            .GetRepository<ImageMapLink>()
            .GetAllAsync(predicate: x => x.PostId == postId); // <= Error after update 5.0.x

        if (maps.Any())
        {
            return maps.ToList().Select(x => new PictureFile(x.ImageSrc, x.MappedSrc!));
        }

        return [];
    }
   ```
  Достаточно явно добавить тип  ``
    ``` csharp
    public async Task<IEnumerable<PictureFile>> GetFilesForPostAsync(Guid postId, CancellationToken cancellationToken)
    {
        var maps = await _unitOfWork
            .GetRepository<ImageMapLink>()
            .GetAllAsync(predicate: x => x.PostId == postId, 
                trackingType: TrackingType.NoTracking); // <= Add explicit value

        if (maps.Any())
        {
            return maps.ToList().Select(x => new PictureFile(x.ImageSrc, x.MappedSrc!));
        }

        return [];
    }
    ```

* Методы, где используется параметр `disableTracking`, помечены атрибутом `obsolete`, что означает, что в последующих версиях данные методы будут удалены.
* Исправлены дубликаты кода, возникшие при склеивании веток (спасибо Andrey).


### 4.0.0 от 2024-02-25

* Обновлена версия сборки до .NET8.
* `IPagedList<T>` вынесен в свой собственный nuget-пакет `Calabogna.PagedListCore`.
* Обновлена документация https://calabonga.github.io/UnitOfWork/

### 3.1.0 от 2023-03-25
* Обновлены nuget-пакеты EntityFramework
* Добавлены дополнительные параметры в методы `IRepository<T>` для управления `AutoIncludes`.
* Обновлена документация https://calabonga.github.io/UnitOfWork/

### 2022-02-08
* Сборка переведена на NET6.0. В проект был обновлен (Nullable = true)
* Обновлены методы на предмет обработки Nullable типов
* Удалена сборка AutoHistory по причине отсутствия поддержки ее NET6.0.

## Ссылки 

* [EntityFramework Core и паттерны "Unit of Work" и "Repository" (ru)](https://www.calabonga.net/blog/post/entityframework-unitofwork-and-repository) 
* [Документация API](https://calabonga.github.io/UnitOfWork/api/index.html)
* [Блог по программированию](https://www.calabonga.net)
* [Boosty.To](https://boosty.to/calabonga)
* Пишите комментарии к видео на сайте [www.calabonga.net](https://www.calabonga.net)

## Автор

![Author](https://www.calabonga.net/images/Calabonga.gif)