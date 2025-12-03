# Convert DotNetConfScoreHandler to Mediator Command

## Summary

Convert `DotNetConfScoreHandler.cs` to `DotNetConfScoreCommand.cs` using Mediator pattern with `IHttpClientFactory` injection.

## Todo List

- [x] Create `Handlers/DotNetConfScoreCommand.cs` implementing `IRequest`
- [x] Add property: `int? Year`
- [x] Add nested `Handler` class implementing `IRequestHandler<DotNetConfScoreCommand>`
- [x] Inject `IHttpClientFactory` and `ILogger<Handler>`
- [x] Use `_httpClientFactory.CreateClient("ArdalisApi")` for API calls
- [x] Use `_httpClientFactory.CreateClient("ArdalisWeb")` for playlists.json
- [x] Update `Program.cs` route: `.Map<DotNetConfScoreCommand>("dotnetconf-score {year:int?}", ...)`
- [x] Delete `Handlers/DotNetConfScoreHandler.cs`
- [x] Verify `ardalis dotnetconf-score` and `ardalis dotnetconf-score 2024` work

## Notes

Reference: `.agent/workspace/2025-12-03T18-00-00_di-migration-analysis.md`

Named clients: `ArdalisApi`, `ArdalisWeb`
Parameters: `int? year`

This handler uses both the Ardalis API (for video stats) and ardalis.com (for playlists.json).

## Results

- Created `DotNetConfScoreCommand.cs` with nested `Handler` class implementing `IRequestHandler<DotNetConfScoreCommand>`
- Handler injects `IHttpClientFactory` and `ILogger<Handler>`
- Uses "ArdalisApi" named client for video stats (via `ArdalisApiClient`)
- Uses "ArdalisWeb" named client for fetching playlists.json
- Updated `Program.cs` to use `.Map<DotNetConfScoreCommand>` with the same route pattern
- Deleted legacy `DotNetConfScoreHandler.cs`
- Build succeeds with 0 warnings, 0 errors
- Tested: `ardalis dotnetconf-score` (defaults to 2025) - works correctly
- Tested: `ardalis dotnetconf-score 2024` - works correctly with different year
