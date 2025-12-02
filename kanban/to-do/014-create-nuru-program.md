# Create Nuru Program Entry Point

## Summary

Replace Program.cs with Nuru `NuruApp.CreateBuilder` pattern including route definitions and pipeline behavior registration.

## Todo List

- [ ] Backup existing `Program.cs` to `Program.Spectre.cs.bak`
- [ ] Create new `Program.cs` with `NuruApp.CreateBuilder`
- [ ] Configure REPL options (prompt, welcome message, goodbye message)
- [ ] Set app metadata (name, description, version)
- [ ] Call `UseTelemetry()` for OpenTelemetry integration
- [ ] Register `PostHogService` in DI
- [ ] Register Mediator with `PostHogTrackingBehavior`
- [ ] Add URL command routes using constants and `Open()`
- [ ] Add display command routes pointing to handlers
- [ ] Add commands with options (packages, books, courses, recent)
- [ ] Add command with argument (dotnetconf-score)
- [ ] Add default handler
- [ ] Verify `dotnet build` succeeds
- [ ] Verify `ardalis --help` shows all commands
- [ ] Verify `ardalis card` works
- [ ] Verify `ardalis blog` works
- [ ] Verify `ardalis -i` enters REPL mode

## Notes

This is Phase 5 of the TimeWarp.Nuru migration.

Use `using static Ardalis.Urls;` and `using static Ardalis.Helpers.UrlHelper;` for clean route definitions:
```csharp
.Map("blog", () => Open(Blog), "Open Ardalis's blog")
```

File to create:
- `Program.cs` (new)

File to backup:
- `Program.cs` -> `Program.Spectre.cs.bak`

Reference: `.agent/workspace/2025-12-02T16-30-00_timewarp-nuru-migration-plan.md` - Phase 5

## Results

