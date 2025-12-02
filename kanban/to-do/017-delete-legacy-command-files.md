# Delete Legacy Command Files

## Summary

Delete all Spectre.Console command files from the Commands/ directory.

## Todo List

- [ ] Delete `Commands/BlogCommand.cs`
- [ ] Delete `Commands/BlueSkyCommand.cs`
- [ ] Delete `Commands/BooksCommand.cs`
- [ ] Delete `Commands/CardCommand.cs`
- [ ] Delete `Commands/ContactCommand.cs`
- [ ] Delete `Commands/CoursesCommand.cs`
- [ ] Delete `Commands/DometrainCommand.cs`
- [ ] Delete `Commands/DotNetConfScoreCommand.cs`
- [ ] Delete `Commands/LinkedInCommand.cs`
- [ ] Delete `Commands/NimbleProCommand.cs`
- [ ] Delete `Commands/PackagesCommand.cs`
- [ ] Delete `Commands/PluralsightCommand.cs`
- [ ] Delete `Commands/QuoteCommand.cs`
- [ ] Delete `Commands/RecentCommand.cs`
- [ ] Delete `Commands/ReposCommand.cs`
- [ ] Delete `Commands/SpeakerCommand.cs`
- [ ] Delete `Commands/SubscribeCommand.cs`
- [ ] Delete `Commands/TipCommand.cs`
- [ ] Delete `Commands/YouTubeCommand.cs`
- [ ] Delete `Commands/` directory
- [ ] Verify `dotnet build` succeeds

## Notes

This is Phase 7b of the TimeWarp.Nuru migration.

All 19 command files are replaced by:
- URL constants in `Urls.cs`
- Handler files in `Handlers/`
- Route definitions in `Program.cs`

Files to delete:
- `Commands/*.cs` (19 files)
- `Commands/` directory

Reference: `.agent/workspace/2025-12-02T16-30-00_timewarp-nuru-migration-plan.md` - Phase 7

## Results

