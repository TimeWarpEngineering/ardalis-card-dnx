# Create DotNetConf Score Handler

## Summary

Create DotNetConfScoreHandler to display top videos from .NET Conf playlists.

## Todo List

- [x] Create `Handlers/DotNetConfScoreHandler.cs`
- [x] Use `NuruTerminal.Default` for output
- [x] Port logic from existing DotNetConfScoreCommand
- [x] Use existing `ArdalisApiClient` for API calls
- [x] Accept `int year` parameter (defaults to current year)
- [x] Display video rankings in table
- [x] Verify `dotnet build` succeeds

## Notes

This is Phase 4j of the TimeWarp.Nuru migration.

DotNetConfScoreHandler uses ArdalisApiClient which needs proper configuration. Consider this a candidate for conversion to Mediator command post-migration (see Future Considerations).

File to create:
- `Handlers/DotNetConfScoreHandler.cs`

Reference: `.agent/workspace/2025-12-02T16-30-00_timewarp-nuru-migration-plan.md` - Phase 4

## Results

- Created `Handlers/DotNetConfScoreHandler.cs` (179 lines)
- Uses `NuruTerminal.Default` for all output
- Async `ExecuteAsync(int year)` method
- Uses `ArdalisApiClient` to fetch playlist statistics
- Fetches playlists from ardalis.com/playlists.json
- Table widget with Rank, Title (clickable link), Views columns
- Ardalis's videos highlighted with ⭐ in yellow
- Proper error handling for API failures (403, network errors, etc.)
- Build succeeds with 0 warnings, 0 errors
