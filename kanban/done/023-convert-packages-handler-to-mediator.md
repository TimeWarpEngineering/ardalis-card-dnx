# Convert PackagesHandler to Mediator Command

## Summary

Convert `PackagesHandler.cs` to `PackagesCommand.cs` using Mediator pattern with `IHttpClientFactory` injection.

## Todo List

- [x] Create `Handlers/PackagesCommand.cs` implementing `IRequest`
- [x] Add properties: `bool All`, `int? Size`
- [x] Add nested `Handler` class implementing `IRequestHandler<PackagesCommand>`
- [x] Inject `IHttpClientFactory` and `ILogger<Handler>`
- [x] Use `_httpClientFactory.CreateClient("NuGet")` instead of static HttpClient
- [x] Update `Program.cs` route: `.Map<PackagesCommand>("packages --all? --page-size? {size:int?}", ...)`
- [x] Delete `Handlers/PackagesHandler.cs`
- [x] Verify `ardalis packages` and `ardalis packages --all` work

## Notes

Reference: `.agent/workspace/2025-12-03T18-00-00_di-migration-analysis.md`

Named client: `NuGet`
Parameters: `bool all`, `int? size`

## Results

- Converted PackagesHandler.cs to PackagesCommand.cs following ReposCommand.cs pattern
- Used nested Handler class with IHttpClientFactory and ILogger<Handler> injection
- Uses `_httpClientFactory.CreateClient("NuGet")` with base address from Program.cs
- Updated Program.cs to use `.Map<PackagesCommand>(...)` generic overload
- Build succeeds with zero warnings/errors
- Both `packages` and `packages --all` commands work correctly
