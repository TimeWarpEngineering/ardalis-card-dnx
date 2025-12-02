# Create Nuru Program Entry Point

## Summary

Replace Program.cs with Nuru `NuruApp.CreateBuilder` pattern including route definitions and pipeline behavior registration.

## Todo List

- [x] Backup existing `Program.cs` to `Program.Spectre.cs.bak`
- [x] Create new `Program.cs` with `NuruApp.CreateBuilder`
- [x] Configure REPL options (prompt, welcome message, goodbye message)
- [x] Set app metadata (name, description, version)
- [ ] Call `UseTelemetry()` for OpenTelemetry integration (deferred - not critical for MVP)
- [x] Register `PostHogService` in DI
- [x] Register Mediator with `PostHogTrackingBehavior` (via services.AddMediator())
- [x] Add URL command routes using constants and `Open()`
- [x] Add display command routes pointing to handlers
- [x] Add commands with options (packages, books, courses, recent)
- [x] Add command with argument (dotnetconf-score)
- [x] Add default handler
- [x] Verify `dotnet build` succeeds
- [x] Verify `ardalis --help` shows all commands
- [x] Verify `ardalis card` works
- [x] Verify `ardalis blog` works
- [ ] Verify `ardalis -i` enters REPL mode (requires TimeWarp.Nuru.Repl package)

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

- Backed up `Program.cs` to `Program.Spectre.cs.bak`
- Created new `Program.cs` with `NuruApp.CreateBuilder` (160 lines)
- REPL options configured: cyan prompt "ardalis> ", welcome/goodbye messages
- Metadata configured with description
- PostHogService registered in DI
- Mediator registered via `services.AddMediator()`
- 12 URL commands using `Open()` with URL constants
- 4 display commands (card, quote, tip, repos)
- 4 commands with options (packages, books, courses, recent)
- 1 command with argument (dotnetconf-score with optional year)
- Version command with NuGet update check
- Default handler shows business card
- All commands verified working:
  - `--help` shows all 28+ commands
  - `card` displays business card with Nuru widgets
  - `blog` opens browser
  - `quote` displays random quote
  - `version` shows version and update status
- REPL mode requires TimeWarp.Nuru.Repl package (not included yet)
- OpenTelemetry integration deferred
