# Convert BooksHandler to Mediator Command

## Summary

Convert `BooksHandler.cs` to `BooksCommand.cs` using Mediator pattern with `IHttpClientFactory` injection.

## Todo List

- [x] Create `Handlers/BooksCommand.cs` implementing `IRequest`
- [x] Add properties: `bool NoPaging`, `int? Size`
- [x] Add nested `Handler` class implementing `IRequestHandler<BooksCommand>`
- [x] Inject `IHttpClientFactory` and `ILogger<Handler>`
- [x] Use `_httpClientFactory.CreateClient("ArdalisWeb")` instead of static HttpClient
- [x] Update `Program.cs` route: `.Map<BooksCommand>("books --no-paging? --page-size? {size:int?}", ...)`
- [x] Delete `Handlers/BooksHandler.cs`
- [x] Verify `ardalis books` works

## Notes

Reference: `.agent/workspace/2025-12-03T18-00-00_di-migration-analysis.md`

Named client: `ArdalisWeb`
Parameters: `bool noPaging`, `int? size`

## Results

- Created `Handlers/BooksCommand.cs` with Mediator pattern following ReposCommand/PackagesCommand patterns
- Added `Books` constant to `Urls.cs` for consistency
- Updated `Program.cs` to use `.Map<BooksCommand>()` instead of delegate pattern
- Deleted old `BooksHandler.cs`
- Build succeeds with 0 warnings, 0 errors
- Command `ardalis books --no-paging` works correctly, displaying books with proper formatting
