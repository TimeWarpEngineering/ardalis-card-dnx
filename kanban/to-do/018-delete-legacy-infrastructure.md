# Delete Legacy Infrastructure

## Summary

Delete TypeRegistrar, InteractiveMode, and other legacy files no longer needed with Nuru.

## Todo List

- [ ] Delete `Infrastructure/TypeRegistrar.cs`
- [ ] Delete `Infrastructure/` directory (if empty)
- [ ] Delete `InteractiveMode.cs`
- [ ] Delete `Helpers/PagingHelper.cs` (if no longer needed)
- [ ] Delete `Program.Spectre.cs.bak`
- [ ] Verify `dotnet build` succeeds

## Notes

This is Phase 7c of the TimeWarp.Nuru migration.

These files are replaced by:
- `TypeRegistrar.cs` -> Nuru's built-in DI integration
- `InteractiveMode.cs` -> Nuru's built-in REPL mode
- `PagingHelper.cs` -> May be reimplemented in handlers or use Nuru equivalent

Files to delete:
- `Infrastructure/TypeRegistrar.cs`
- `InteractiveMode.cs`
- `Helpers/PagingHelper.cs`
- `Program.Spectre.cs.bak`

Reference: `.agent/workspace/2025-12-02T16-30-00_timewarp-nuru-migration-plan.md` - Phase 7

## Results

