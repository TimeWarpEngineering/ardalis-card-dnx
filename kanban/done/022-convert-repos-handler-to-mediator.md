# Convert ReposHandler to Mediator Command

## Summary

Convert `ReposHandler.cs` to `ReposCommand.cs` using Mediator pattern with `IHttpClientFactory` injection.

## Todo List

- [x] Create `Handlers/ReposCommand.cs` implementing `IRequest`
- [x] Add nested `Handler` class implementing `IRequestHandler<ReposCommand>`
- [x] Inject `IHttpClientFactory` and `ILogger<Handler>`
- [x] Use `_httpClientFactory.CreateClient("GitHub")` instead of static HttpClient
- [x] Update `Program.cs` route: `.Map<ReposCommand>("repos", ...)`
- [x] Delete `Handlers/ReposHandler.cs`
- [x] Verify `ardalis repos` works

## Notes

Reference: `.agent/workspace/2025-12-03T18-00-00_di-migration-analysis.md`

Named client: `GitHub`
Parameters: None

## Results

- Created `Handlers/ReposCommand.cs` with nested `Handler` class using MediatR pattern
- Handler injects `IHttpClientFactory` and `ILogger<Handler>` via constructor
- Uses `_httpClientFactory.CreateClient("GitHub")` which was pre-configured in Program.cs with BaseAddress and User-Agent
- Updated Program.cs to use `.Map<ReposCommand>("repos", ...)` generic mediator syntax
- Deleted static `ReposHandler.cs` - no longer needed
- Build succeeds with 0 warnings, 0 errors
- `ardalis repos` command tested successfully - displays table with repository stats from GitHub API
