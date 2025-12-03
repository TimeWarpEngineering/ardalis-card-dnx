# Convert CoursesHandler to Mediator Command

## Summary

Convert `CoursesHandler.cs` to `CoursesCommand.cs` using Mediator pattern with `IHttpClientFactory` injection.

## Todo List

- [x] Create `Handlers/CoursesCommand.cs` implementing `IRequest`
- [x] Add properties: `bool All`, `int? Size`
- [x] Add nested `Handler` class implementing `IRequestHandler<CoursesCommand>`
- [x] Inject `IHttpClientFactory` and `ILogger<Handler>`
- [x] Use `_httpClientFactory.CreateClient("ArdalisWeb")` instead of static HttpClient
- [x] Update `Program.cs` route: `.Map<CoursesCommand>("courses --all? --page-size? {size:int?}", ...)`
- [x] Delete `Handlers/CoursesHandler.cs`
- [x] Verify `ardalis courses` works

## Notes

Reference: `.agent/workspace/2025-12-03T18-00-00_di-migration-analysis.md`

Named client: `ArdalisWeb`
Parameters: `bool all`, `int? size`

## Results

- Created `CoursesCommand.cs` with nested `Handler` class following established patterns from `BooksCommand.cs`
- Uses `IHttpClientFactory` with "ArdalisWeb" named client for proper DI
- Uses `ILogger<Handler>` for logging failures
- Added `Courses` URL constant to `Urls.cs`
- Updated `Program.cs` to use `.Map<CoursesCommand>()` instead of delegate with `CoursesHandler`
- Deleted legacy `CoursesHandler.cs`
- Build succeeds, command tested with `--all` and `--page-size` options
