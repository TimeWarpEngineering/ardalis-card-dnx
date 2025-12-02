# Create DotNetConf Score Handler

## Summary

Create DotNetConfScoreHandler to display top videos from .NET Conf playlists.

## Todo List

- [ ] Create `Handlers/DotNetConfScoreHandler.cs`
- [ ] Use `NuruTerminal.Default` for output
- [ ] Port logic from existing DotNetConfScoreCommand
- [ ] Use existing `ArdalisApiClient` for API calls
- [ ] Accept `int year` parameter (defaults to current year)
- [ ] Display video rankings in table
- [ ] Verify `dotnet build` succeeds

## Notes

This is Phase 4j of the TimeWarp.Nuru migration.

DotNetConfScoreHandler uses ArdalisApiClient which needs proper configuration. Consider this a candidate for conversion to Mediator command post-migration (see Future Considerations).

File to create:
- `Handlers/DotNetConfScoreHandler.cs`

Reference: `.agent/workspace/2025-12-02T16-30-00_timewarp-nuru-migration-plan.md` - Phase 4

## Results

