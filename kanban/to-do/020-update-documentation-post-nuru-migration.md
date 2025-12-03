# Update Documentation Post-Nuru Migration

## Summary

Update all documentation to reflect the Spectre.Console to TimeWarp.Nuru migration completed in PR #53. Documentation still references obsolete patterns, deleted files, and removed features.

## Todo List

### Critical (README.md)

- [ ] Remove all `--with-covers` references (lines 102-103, 259) - feature was removed
- [ ] Update framework attribution from Spectre.Console to TimeWarp.Nuru (line 288-289)
- [ ] Add missing `github` command to command list
- [ ] Add missing `nuget` command to command list
- [ ] Document `--interactive` flag alongside `-i` for REPL mode

### Critical (copilot-instructions.md) - Complete Rewrite

- [ ] Update project overview to reference TimeWarp.Nuru
- [ ] Replace Command class structure with route-based pattern documentation
- [ ] Remove obsolete `AsyncCommand<T>` and `CommandSettings` patterns
- [ ] Document Nuru route pattern syntax (`{param}`, `{param?}`, `--flag?`, etc.)
- [ ] Document Handler pattern (static classes in `Handlers/`)
- [ ] Document URL constants pattern (`Urls.cs` + inline lambdas)
- [ ] Remove obsolete registration instructions (no Commands folder, no InteractiveMode.cs)
- [ ] Replace Spectre.Console markup conventions with Nuru fluent extensions
- [ ] Document pipeline behaviors for cross-cutting concerns
- [ ] Add current project structure

### Medium (CONTRIBUTING.md)

- [ ] Update `QuoteCommand` reference to `QuoteHandler`
- [ ] Update `TipsCommand` reference to `TipHandler`
- [ ] Update `CoursesCommand` reference to `CoursesHandler`
- [ ] Update `BooksCommand` reference to `BooksHandler`

## Notes

### Files to Update

1. **README.md** - User-facing documentation
2. **CONTRIBUTING.md** - Contributor guidelines
3. **.github/copilot-instructions.md** - AI assistant instructions (needs complete rewrite)

### Key Architecture Changes to Document

**Old (Spectre.Console.Cli):**
- Command classes inheriting from `Command` or `AsyncCommand<T>`
- `CommandSettings` classes with `[CommandOption]` attributes
- TypeRegistrar/TypeResolver DI bridge
- Custom `InteractiveMode.cs` for REPL
- Spectre markup: `[bold green]Text[/]`, `[link=url]Click[/]`

**New (TimeWarp.Nuru):**
- Route-based patterns: `.Map("command --flag? {param:int?}", handler, description)`
- Static handler classes in `Handlers/` directory
- URL constants in `Urls.cs`
- Built-in REPL via `app.RunReplAsync()`
- Nuru fluent extensions: `"Text".Green().Bold()`, `"Click".Link(url)`
- Pipeline behaviors in `Behaviors/` for cross-cutting concerns

### Current Project Structure

```
ardalis/
├── Behaviors/
│   └── PostHogTrackingBehavior.cs
├── Handlers/
│   ├── BooksHandler.cs
│   ├── CardHandler.cs
│   ├── CoursesHandler.cs
│   ├── DotNetConfScoreHandler.cs
│   ├── PackagesHandler.cs
│   ├── QuoteHandler.cs
│   ├── RecentHandler.cs
│   ├── ReposHandler.cs
│   └── TipHandler.cs
├── Helpers/
│   ├── QuoteHelper.cs
│   ├── RecentHelper.cs
│   ├── TipHelper.cs
│   └── UrlHelper.cs
├── Telemetry/
│   ├── ArdalisCliTelemetry.cs
│   ├── LoggingHttpClientFactory.cs
│   └── PostHogService.cs
├── Program.cs
├── Urls.cs
├── ArdalisApiClient.cs
└── ardalis.csproj
```

### Reference Documents

- Full analysis: `.agent/workspace/2025-12-03T12-00-00_documentation-review-nuru-migration.md`
- Migration plan: `.agent/workspace/2025-12-02T16-30-00_timewarp-nuru-migration-plan.md`
- Merge PR: https://github.com/ardalis/ardalis-card-dnx/pull/53
