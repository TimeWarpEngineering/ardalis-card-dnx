# Copilot Instructions for ardalis-card-dnx

## Project Overview

A .NET global tool CLI for accessing Ardalis resources built with Spectre.Console.Cli. Runs via `dnx ardalis` (no install) or `ardalis` (after `dotnet tool install -g ardalis`).

## Invariants

- Do NOT change the FrameworkVersion.

## Architecture Patterns

### Command Structure
- **Simple Commands** (URLs/actions): Inherit from `Command`, execute synchronously
  - Example: `BlogCommand` - Opens URLs using `UrlHelper.Open()`
  - Pattern: `Commands/BlogCommand.cs`, `Commands/YouTubeCommand.cs`
  
- **Data-Fetching Commands**: Inherit from `AsyncCommand<T>` or `AsyncCommand<Settings>`
  - Example: `PackagesCommand`, `ReposCommand`, `RecentCommand`
  - Pattern: Fetch from external APIs, fallback to hardcoded data on failure
  - Always include `CancellationToken cancellationToken = default` parameter

### Command Options Pattern
Commands with options use a nested `Settings` class:
```csharp
public class MyCommand : AsyncCommand<MyCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("--flag")]
        [Description("Description for help")]
        public bool Flag { get; set; }
    }
    
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken = default)
    {
        // Use settings.Flag
    }
}
```

### API Fetching with Fallback
**Critical Pattern** - All data-fetching commands must have fallbacks:
```csharp
try {
    var data = await FetchFromApi();
} catch {
    data = FallbackData; // Hardcoded static readonly array/list
}
```
Examples: `PackagesCommand.FallbackPackages`, `CoursesCommand.GetFallbackCourses()`

### Helper Organization
- `Helpers/UrlHelper.cs` - Opens URLs cross-platform
- `Helpers/QuoteHelper.cs`, `TipHelper.cs` - Fetch random items from JSON endpoints
- `Helpers/RecentHelper.cs` - Aggregates data from multiple sources in parallel

### Shared Source Configuration
Avoid duplication when sources are used in multiple places - extract to static field:
```csharp
private static readonly List<(string Name, string Icon, Func<Task<List<T>>> FetchFunc)> Sources = new() { ... };
```
Example: `RecentHelper.Sources` used by both verbose and normal modes

## Registration Requirements

### Adding a New Command
1. Create command in `Commands/` folder
2. Register in `Program.cs` **TWICE** (help intercept + main app)
3. Add to `InteractiveMode.cs` switch statement
4. Update README.md examples

### Version Updates
Update **both** in `ardalis.csproj`:
- `<Version>` - Increment appropriately (major.minor.patch)
- `<ReleaseNotes>` - Describe changes concisely

## Development Workflows

### Testing Locally
```bash
dotnet run -- <command>           # Test specific command
dotnet run -- --help              # Test help with installation instructions
dotnet run -- -i                  # Test interactive mode
dotnet run -- <command> --verbose # Test verbose output (if applicable)
```

### Publishing
1. Update `<Version>` and `<ReleaseNotes>` in `ardalis.csproj`
2. Commit and push to main
3. Create GitHub Release with tag (e.g., `v1.10.0`) - triggers auto-publish to NuGet
4. See `CONTRIBUTING.md` for details

## Spectre.Console Conventions

### Icons
Use emojis consistently:
- 📝 Blog
- 🎥 YouTube  
- ⚡ GitHub
- 🦋 Bluesky
- 💼 LinkedIn
- 📦 NuGet packages

### Table Formatting
```csharp
var table = new Table();
table.Border = TableBorder.Rounded;
table.AddColumn(new TableColumn("[bold]Header[/]").Centered());
table.AddRow($"[link={url}]Text[/]", $"[yellow]⭐ {count}[/]");
AnsiConsole.Write(table);
```

### Markup Patterns
- `[bold green]Success[/]` - Success messages
- `[yellow]Warning[/]` - Warnings
- `[dim]Subtle info[/]` - Less important text
- `[link=url]Click here[/]` - Clickable links

## Target Framework
Currently targeting **net10.0** (.NET 10 preview). Adjust when stable releases come out.

## Code Style
- Use file-scoped namespaces: `namespace Ardalis.Commands;`
- Prefer `var` for local variables
- Use collection expressions: `new() { item1, item2 }`
- Use pattern matching and switch expressions where appropriate
