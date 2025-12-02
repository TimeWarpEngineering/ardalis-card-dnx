# Create URL Constants

## Summary

Create `Urls.cs` with URL constants for all Ardalis links, replacing magic strings throughout the codebase.

## Todo List

- [ ] Create `Urls.cs` in project root
- [ ] Add constant for Blog URL
- [ ] Add constant for BlueSky URL
- [ ] Add constant for Contact URL
- [ ] Add constant for Dometrain URL
- [ ] Add constant for GitHub URL
- [ ] Add constant for LinkedIn URL
- [ ] Add constant for NimblePros URL
- [ ] Add constant for NuGet URL
- [ ] Add constant for Pluralsight URL
- [ ] Add constant for Speaker/Sessionize URL
- [ ] Add constant for Subscribe URL
- [ ] Add constant for YouTube URL
- [ ] Verify `dotnet build` succeeds

## Notes

This is Phase 4a of the TimeWarp.Nuru migration.

Constants are preferable to wrapper methods like `UrlHandlers.OpenBlog()` because:
- URLs are reusable across handlers (CardHandler uses same URLs for panel links)
- Self-documenting (`Blog` vs magic string)
- Refactorable
- No pointless indirection

File to create:
- `Urls.cs`

Reference: `.agent/workspace/2025-12-02T16-30-00_timewarp-nuru-migration-plan.md` - Phase 4

## Results

