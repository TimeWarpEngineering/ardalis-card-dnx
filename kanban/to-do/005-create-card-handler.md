# Create Card Handler

## Summary

Create CardHandler using Nuru terminal widgets to display Ardalis business card.

## Todo List

- [ ] Create `Handlers/` directory
- [ ] Create `Handlers/CardHandler.cs`
- [ ] Use `NuruTerminal.Default` for output
- [ ] Use URL constants from `Urls.cs` with `using static`
- [ ] Use `UrlHelper.AddUtmSource()` for tracking URLs
- [ ] Implement panel with header, content, border styling
- [ ] Use Nuru string extensions (`.Cyan()`, `.Bold()`, `.Link()`, etc.)
- [ ] Verify `dotnet build` succeeds

## Notes

This is Phase 4b of the TimeWarp.Nuru migration.

CardHandler is a good first display handler because:
- No external dependencies (no HttpClient)
- No async work
- Tests panel widget, colors, links

No PostHogService injection needed - tracking handled by pipeline behavior.

File to create:
- `Handlers/CardHandler.cs`

Reference: `.agent/workspace/2025-12-02T16-30-00_timewarp-nuru-migration-plan.md` - Phase 4

## Results

