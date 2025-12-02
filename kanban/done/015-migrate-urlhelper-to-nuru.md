# Migrate UrlHelper to Nuru

## Summary

Update UrlHelper to use `NuruTerminal.Default` instead of Spectre.Console for terminal output.

## Todo List

- [x] Replace `using Spectre.Console;` with `using TimeWarp.Nuru;`
- [x] Replace `AnsiConsole.MarkupLine()` calls with `terminal.WriteLine()`
- [x] Use Nuru string extensions (`.Cyan()`, `.Yellow()`, `.Link()`)
- [x] Update `Open()` method to use `NuruTerminal.Default`
- [x] Verify `dotnet build` succeeds
- [x] Verify URL opening still works correctly

## Notes

This is Phase 6 of the TimeWarp.Nuru migration.

UrlHelper methods to update:
- `Open()` - Main method that outputs to terminal

Keep unchanged:
- `StripQueryString()` - Pure string manipulation
- `AddUtmSource()` - Pure string manipulation
- `TryOpenUrl()` - Process launching, no terminal output
- `TryStartProcess()` - Process launching
- `IsWsl()` - Environment detection

File to modify:
- `Helpers/UrlHelper.cs`

Reference: `.agent/workspace/2025-12-02T16-30-00_timewarp-nuru-migration-plan.md` - Phase 6

## Results

- Replaced `using Spectre.Console;` with `using TimeWarp.Nuru;`
- Updated `Open()` method to use `NuruTerminal.Default`
- Uses Nuru string extensions: `.Link()`, `.Gray()`, `.Cyan()`, `.Yellow()`
- Build succeeds with 0 warnings, 0 errors
- URL opening verified working with `ardalis blog` command
