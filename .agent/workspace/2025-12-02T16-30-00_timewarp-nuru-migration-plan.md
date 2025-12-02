# TimeWarp.Nuru Migration Plan for Ardalis CLI

## Executive Summary

This document provides a comprehensive migration plan to convert the Ardalis CLI from Spectre.Console.Cli to TimeWarp.Nuru. The migration removes the `--with-covers` option and ImageSharp dependency, replaces Spectre.Console UI widgets with Nuru terminal widgets, and modernizes the CLI architecture using route-based patterns with pipeline behaviors for cross-cutting concerns. The plan is structured in 7 incremental phases to maintain a compilable codebase throughout.

## Scope

**Current State:**
- CLI framework: Spectre.Console.Cli (v0.53.0)
- UI rendering: Spectre.Console + Spectre.Console.ImageSharp
- 19 commands across two categories (display + open URLs)
- Interactive mode via custom `InteractiveMode.cs`
- PostHog telemetry injected into every command
- TypeRegistrar/TypeResolver bridge for DI

**Target State:**
- CLI framework: TimeWarp.Nuru (v3.0.0-beta.11+)
- UI rendering: Nuru Terminal widgets (Panel, Table, Rule, Hyperlink)
- Route-based command definitions with `builder.Map()`
- Built-in REPL mode replacing custom InteractiveMode
- **PostHog tracking via single pipeline behavior (cross-cutting concern)**
- Integrated telemetry via `UseTelemetry()`
- No ImageSharp dependency

## Methodology

- Analyzed all 19 command files in `/Commands/`
- Reviewed Program.cs, InteractiveMode.cs, and helper classes
- Examined TimeWarp.Nuru examples: delegate, createbuilder, repl-interactive, panel-widget, table-widget, hyperlink-widget, rule-widget, aspire-telemetry, **pipeline-middleware, unified-middleware**
- Mapped Spectre.Console patterns to Nuru equivalents

---

## Phase 1: Foundation Setup

**Goal:** Add TimeWarp.Nuru without removing Spectre.Console yet - establish hybrid environment.

### Changes Required:

1. **Update ardalis.csproj:**
```xml
<ItemGroup>
  <!-- Add Nuru + Mediator for pipeline behaviors -->
  <PackageReference Include="TimeWarp.Nuru" Version="3.0.0-beta.11" />
  <PackageReference Include="Mediator.Abstractions" Version="2.1.7" />
  <PackageReference Include="Mediator.SourceGenerator" Version="2.1.7" />
  
  <!-- Keep existing for now -->
  <PackageReference Include="Microsoft.Extensions.Hosting" Version="10.0.0" />
  <PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.14.0" />
  <PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.14.0" />
  <PackageReference Include="PostHog.AspNetCore" Version="2.2.1" />
  <PackageReference Include="Spectre.Console.Cli" Version="0.53.0" />
  <PackageReference Include="Spectre.Console" Version="0.53.0" />
  <PackageReference Include="Spectre.Console.Extensions.Logging" Version="0.2.1" />
  <!-- REMOVE: Spectre.Console.ImageSharp - causes --with-covers dependency -->
</ItemGroup>
```

### Files Modified:
- `ardalis.csproj`

### Validation:
- `dotnet build` succeeds
- Existing CLI functionality unchanged

---

## Phase 2: Remove ImageSharp Dependency

**Goal:** Remove `--with-covers` option and ImageSharp package.

### Changes Required:

1. **Update BooksCommand.cs:**
   - Remove `WithCovers` property from Settings
   - Remove `DisplayBook` image rendering code
   - Remove ImageSharp using statements

**Before (lines 27-31):**
```csharp
public class Settings : CommandSettings
{
    [CommandOption("--with-covers")]
    [Description("Display book cover images")]
    public bool WithCovers { get; set; }
    // ...
}
```

**After:**
```csharp
public class Settings : CommandSettings
{
    [CommandOption("--no-paging")]
    [Description("Disable paging")]
    public bool NoPaging { get; set; }

    [CommandOption("--page-size")]
    [Description("Sets page size (default: 10)")]
    public int PageSize { get; set; } = 10;
}
```

**Remove from DisplayBook method (lines 124-143):**
```csharp
// DELETE this entire block:
if (withCovers && !string.IsNullOrEmpty(book.CoverImage))
{
    try
    {
        var imageBytes = await _httpClient.GetByteArrayAsync(book.CoverImage);
        using var imageStream = new MemoryStream(imageBytes);
        var image = new CanvasImage(imageStream);
        // ...
    }
}
```

**Update DisplayBook signature:**
```csharp
// Before:
private static async Task DisplayBook(Book book, bool withCovers)

// After:
private static Task DisplayBook(Book book)
```

**Update caller in ExecuteAsync:**
```csharp
// Before:
async book => await DisplayBook(book, settings.WithCovers),

// After:
book => DisplayBook(book),
```

2. **Update ardalis.csproj:**
   - Remove: `<PackageReference Include="Spectre.Console.ImageSharp" Version="0.53.0" />`

### Files Modified:
- `ardalis.csproj`
- `Commands/BooksCommand.cs`

### Validation:
- `dotnet build` succeeds
- `ardalis books` works (without --with-covers)
- `ardalis books --help` no longer shows --with-covers option

---

## Phase 3: Create PostHog Pipeline Behavior

**Goal:** Implement PostHog tracking as a cross-cutting concern via a single pipeline behavior.

### Why Pipeline Behavior?

**Current approach (bad):** PostHogService injected into every command
```csharp
// Every handler has this boilerplate:
public static void Execute(PostHogService postHog)
{
    postHog.TrackCommand("card");  // Manual tracking in every handler
    // ... actual logic
}
```

**Target approach (good):** Pipeline behavior handles tracking automatically
```csharp
// Handler is clean - just business logic:
public static void Execute()
{
    // ... actual logic only
}

// Single pipeline behavior tracks ALL commands automatically
```

### How It Works

In Nuru, delegate routes are wrapped in `DelegateRequest` and flow through the same Mediator pipeline as command classes. One behavior handles both:

```
┌─────────────────────────────────────────────────────────────┐
│                    Single Pipeline                          │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Delegate route: ardalis blog                               │
│         │                                                   │
│         ▼                                                   │
│  Wrapped as DelegateRequest ──┐                             │
│                               │                             │
│  Mediator route: ardalis card │                             │
│         │                     │                             │
│         ▼                     ▼                             │
│  ┌─────────────────────────────────────┐                   │
│  │ PostHogTrackingBehavior             │                   │
│  │   - Extracts command name           │                   │
│  │   - Calls _postHog.TrackCommand()   │                   │
│  │   - Calls next()                    │                   │
│  └─────────────────────────────────────┘                   │
│                    │                                        │
│                    ▼                                        │
│              Handler executes                               │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Changes Required:

1. **Create `Behaviors/PostHogTrackingBehavior.cs`:**
```csharp
using Ardalis.Cli.Telemetry;
using Mediator;
using Microsoft.Extensions.Logging;
using TimeWarp.Nuru;

namespace Ardalis.Cli.Behaviors;

/// <summary>
/// Pipeline behavior that automatically tracks ALL command execution with PostHog.
/// Works for both Mediator commands and delegate routes (via DelegateRequest).
/// </summary>
public sealed class PostHogTrackingBehavior<TMessage, TResponse> 
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    private readonly PostHogService _postHog;
    private readonly RouteExecutionContext _executionContext;
    private readonly ILogger<PostHogTrackingBehavior<TMessage, TResponse>> _logger;

    public PostHogTrackingBehavior(
        PostHogService postHog,
        RouteExecutionContext executionContext,
        ILogger<PostHogTrackingBehavior<TMessage, TResponse>> logger)
    {
        _postHog = postHog;
        _executionContext = executionContext;
        _logger = logger;
    }

    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        // Extract command name from route pattern (works for delegates and commands)
        // RoutePattern examples: "blog", "card", "packages --all?", "dotnetconf-score {year:int?}"
        string commandName = _executionContext.RoutePattern
            .Split(' ')[0]  // Get first segment before any parameters/options
            .ToLowerInvariant();

        _logger.LogDebug("Tracking command: {CommandName}", commandName);
        _postHog.TrackCommand(commandName);

        return await next(message, cancellationToken);
    }
}
```

### Files Created:
- `Behaviors/PostHogTrackingBehavior.cs`

### Validation:
- `dotnet build` succeeds

---

## Phase 4: Create URL Constants and Display Handlers

**Goal:** Create URL constants (not wrapper methods) and display handlers with pure business logic.

### Why Constants Instead of Wrapper Methods?

**Bad - pointless indirection:**
```csharp
public static class UrlHandlers
{
    public static void OpenBlog() => UrlHelper.Open("https://ardalis.com");
    public static void OpenSubscribe() => UrlHelper.Open("https://ardalis.com/tips");
    // 10 one-liner methods, each used exactly once
}

// In Program.cs:
.Map("blog", UrlHandlers.OpenBlog, "Open Ardalis's blog")
```

**Good - constants with inline usage:**
```csharp
public static class Urls
{
    public const string Blog = "https://ardalis.com";
    public const string Subscribe = "https://ardalis.com/tips";
    // Reusable, self-documenting, refactorable
}

// In Program.cs:
using static Ardalis.Helpers.UrlHelper;
using static Ardalis.Urls;

.Map("blog", () => Open(Blog), "Open Ardalis's blog")
```

### Changes Required:

1. **Create `Urls.cs`:**
```csharp
namespace Ardalis;

/// <summary>
/// Ardalis URL constants - reusable across handlers and route definitions.
/// </summary>
public static class Urls
{
    public const string Blog = "https://ardalis.com";
    public const string BlueSky = "https://bsky.app/profile/ardalis.com";
    public const string Contact = "https://ardalis.com/contact";
    public const string Dometrain = "https://dometrain.com/author/steve-smith/";
    public const string GitHub = "https://github.com/ardalis";
    public const string LinkedIn = "https://www.linkedin.com/in/stevenandrewsmith/";
    public const string NimblePros = "https://nimblepros.com";
    public const string NuGet = "https://www.nuget.org/profiles/ardalis";
    public const string Pluralsight = "https://www.pluralsight.com/authors/steve-smith";
    public const string Speaker = "https://sessionize.com/ardalis";
    public const string Subscribe = "https://ardalis.com/tips";
    public const string YouTube = "https://www.youtube.com/ardaboris";
}
```

2. **Create `Handlers/CardHandler.cs`:**
```csharp
using TimeWarp.Nuru;
using static Ardalis.Urls;
using static Ardalis.Helpers.UrlHelper;

namespace Ardalis.Handlers;

public static class CardHandler
{
    public static void Execute()
    {
        var terminal = NuruTerminal.Default;
        
        // URLs with tracking - reusing constants
        var blogUrl = AddUtmSource(Blog);
        var nimbleprosUrl = AddUtmSource(NimblePros);
        var blueskyUrl = AddUtmSource(BlueSky);
        var linkedinUrl = AddUtmSource(LinkedIn);
        var speakerUrl = AddUtmSource(Speaker);

        terminal.WritePanel(panel => panel
            .Header("💠 Ardalis".Cyan().Bold())
            .Content(
                "Steve 'Ardalis' Smith".Magenta().Bold() + "\n" +
                "Software Architect, Speaker, and Trainer".Gray() + "\n\n" +
                Blog.Link(blogUrl).Cyan() + "\n" +
                NimblePros.Link(nimbleprosUrl).Magenta() + "\n\n" +
                "BlueSky".Link(blueskyUrl).Cyan() + " • " +
                "LinkedIn".Link(linkedinUrl).Cyan() + " • " +
                "Sessionize".Link(speakerUrl).Cyan() + "\n\n" +
                "Clean Architecture • DDD • .NET".Gray().Italic())
            .Border(BorderStyle.Rounded)
            .BorderColor(AnsiColors.Magenta)
            .Padding(2, 1));
        
        terminal.WriteLine();
        terminal.WriteLine($"Try '{"ardalis blog".Cyan()}' or '{"ardalis youtube".Magenta()}' for more".Gray());
    }
}
```

3. **Create other display handlers:**

**`Handlers/ReposHandler.cs`:**
```csharp
using TimeWarp.Nuru;
using static Ardalis.Urls;

namespace Ardalis.Handlers;

public static class ReposHandler
{
    public static async Task ExecuteAsync()
    {
        var terminal = NuruTerminal.Default;
        terminal.WriteLine("Ardalis's Popular GitHub Repositories".Green().Bold());
        terminal.WriteLine();

        var repos = await FetchReposAsync();

        terminal.WriteTable(t => t
            .AddColumns("Repository", "Stars", "Description")
            .AddRows(repos.Select(r => new[] 
            {
                r.Name.Link(r.Url),
                $"⭐ {r.Stars:N0}".Yellow(),
                r.Description.Gray()
            }))
            .Border(BorderStyle.Rounded));

        terminal.WriteLine();
        terminal.WriteLine($"Visit: {GitHub.Link(GitHub).Cyan()}".Gray());
    }
    
    // ... FetchReposAsync implementation ...
}
```

### Files Created:
- `Urls.cs`
- `Handlers/CardHandler.cs`
- `Handlers/QuoteHandler.cs`
- `Handlers/TipHandler.cs`
- `Handlers/ReposHandler.cs`
- `Handlers/PackagesHandler.cs`
- `Handlers/BooksHandler.cs`
- `Handlers/CoursesHandler.cs`
- `Handlers/RecentHandler.cs`
- `Handlers/DotNetConfScoreHandler.cs`

### Validation:
- `dotnet build` succeeds

---

## Phase 5: Create Nuru Program with Pipeline Behavior

**Goal:** Replace Program.cs with Nuru `NuruApp.CreateBuilder` pattern.

### Changes Required:

1. **Create new `Program.cs`:**
```csharp
using TimeWarp.Nuru;
using Ardalis.Cli.Behaviors;
using Ardalis.Cli.Telemetry;
using Ardalis.Handlers;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using static Ardalis.Urls;
using static Ardalis.Helpers.UrlHelper;

namespace Ardalis.Cli;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        NuruAppOptions options = new()
        {
            ConfigureRepl = replOptions =>
            {
                replOptions.Prompt = "ardalis> ";
                replOptions.WelcomeMessage = 
                    "Interactive Mode - Ardalis CLI\n" +
                    "Type 'help' for available commands, 'exit' to quit.";
                replOptions.GoodbyeMessage = "Goodbye!";
            }
        };

        var app = NuruApp.CreateBuilder(args, options)
            .WithMetadata(
                name: "ardalis",
                description: "CLI for Ardalis resources and information",
                version: typeof(Program).Assembly.GetName().Version?.ToString() ?? "1.0.0")
            .UseTelemetry()
            .ConfigureServices(services =>
            {
                services.AddSingleton<PostHogService>();
                
                services.AddMediator(opts =>
                {
                    opts.PipelineBehaviors =
                    [
                        typeof(PostHogTrackingBehavior<,>)
                    ];
                });
            })
            // URL commands - inline with constants, no wrapper class needed
            .Map("blog", () => Open(Blog), "Open Ardalis's blog")
            .Map("bluesky", () => Open(BlueSky), "Open Ardalis's Bluesky profile")
            .Map("contact", () => Open(Contact), "Open Ardalis's contact page")
            .Map("dometrain", () => Open(Dometrain), "Open Ardalis's Dometrain Author profile")
            .Map("linkedin", () => Open(LinkedIn), "Open Ardalis's LinkedIn profile")
            .Map("nimblepros", () => Open(NimblePros), "Open NimblePros website")
            .Map("pluralsight", () => Open(Pluralsight), "Open Ardalis's Pluralsight profile")
            .Map("speaker", () => Open(Speaker), "Open Ardalis's Sessionize speaker profile")
            .Map("subscribe", () => Open(Subscribe), "Open Ardalis's newsletter subscription page")
            .Map("youtube", () => Open(YouTube), "Open Ardalis's YouTube channel")
            // Display commands
            .Map("card", CardHandler.Execute, "Display Ardalis's business card")
            .Map("quote", QuoteHandler.ExecuteAsync, "Display a random Ardalis quote")
            .Map("tip", TipHelper.ExecuteAsync, "Display a random coding tip")
            .Map("repos", ReposHandler.ExecuteAsync, "Display popular Ardalis GitHub repositories")
            // Commands with options
            .Map("packages --all? --page-size {size:int?}",
                (bool all, int? size) => PackagesHandler.ExecuteAsync(all, size ?? 10),
                "Display popular Ardalis NuGet packages")
            .Map("books --no-paging? --page-size {size:int?}",
                (bool noPaging, int? size) => BooksHandler.ExecuteAsync(noPaging, size ?? 10),
                "Display published books by Ardalis")
            .Map("courses --all? --page-size {size:int?}",
                (bool all, int? size) => CoursesHandler.ExecuteAsync(all, size ?? 10),
                "Display available courses")
            .Map("recent --verbose?",
                (bool verbose) => RecentHandler.ExecuteAsync(verbose),
                "Display recent activity from Ardalis")
            .Map("dotnetconf-score {year:int?}",
                (int? year) => DotNetConfScoreHandler.ExecuteAsync(year ?? DateTime.Now.Year),
                "Display top videos from .NET Conf playlists")
            // Default handler
            .MapDefault(() =>
            {
                var terminal = NuruTerminal.Default;
                terminal.WriteLine("ardalis CLI - Run 'ardalis --help' for commands".Cyan());
                terminal.WriteLine("Use 'ardalis -i' or 'ardalis --interactive' for REPL mode".Gray());
                return 0;
            })
            .Build();

        return await app.RunAsync(args);
    }
}
```

### Files Modified:
- Backup `Program.cs` → `Program.Spectre.cs.bak`
- Create new `Program.cs` with Nuru implementation

### Validation:
- `dotnet build` succeeds
- `ardalis --help` shows all commands
- `ardalis card` displays business card AND tracks in PostHog
- `ardalis blog` opens blog AND tracks in PostHog
- `ardalis -i` enters REPL mode

---

## Phase 6: Migrate UrlHelper to Nuru

**Goal:** Update UrlHelper to use `NuruTerminal.Default` instead of Spectre.Console.

### Changes Required:

**Before (UrlHelper.cs with Spectre):**
```csharp
using Spectre.Console;

public static void Open(string url)
{
    AnsiConsole.MarkupLine($"Opening [link={urlWithTracking}]{displayUrl}[/]");
    // ...
    AnsiConsole.MarkupLine($"[yellow]Could not open browser automatically.[/]");
}
```

**After (UrlHelper.cs with Nuru):**
```csharp
using TimeWarp.Nuru;

public static void Open(string url)
{
    var terminal = NuruTerminal.Default;
    var urlWithTracking = AddUtmSource(url);
    var displayUrl = StripQueryString(url);
    
    terminal.Write("Opening ");
    terminal.WriteLine(displayUrl.Link(urlWithTracking).Cyan());
    
    var opened = TryOpenUrl(urlWithTracking);
    if (!opened)
    {
        terminal.WriteLine("Could not open browser automatically.".Yellow());
        terminal.Write("Please visit: ");
        terminal.WriteLine(displayUrl.Link(urlWithTracking).Cyan());
    }
}
```

### Files Modified:
- `Helpers/UrlHelper.cs`

---

## Phase 7: Final Cleanup

**Goal:** Remove all Spectre.Console dependencies and legacy code.

### Changes Required:

1. **Update ardalis.csproj - Final packages:**
```xml
<ItemGroup>
  <PackageReference Include="TimeWarp.Nuru" Version="3.0.0-beta.11" />
  <PackageReference Include="Mediator.Abstractions" Version="2.1.7" />
  <PackageReference Include="Mediator.SourceGenerator" Version="2.1.7" />
  <PackageReference Include="Microsoft.Extensions.Hosting" Version="10.0.0" />
  <PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.14.0" />
  <PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.14.0" />
  <PackageReference Include="PostHog.AspNetCore" Version="2.2.1" />
  <!-- REMOVED: Spectre.Console, Spectre.Console.Cli, Spectre.Console.Extensions.Logging, Spectre.Console.ImageSharp -->
</ItemGroup>
```

2. **Delete legacy files:**
   - `Commands/` directory (all 19 command files)
   - `Infrastructure/TypeRegistrar.cs`
   - `InteractiveMode.cs`
   - `Program.Spectre.cs.bak`
   - `Helpers/PagingHelper.cs` (if no longer needed)

### Files Deleted:
- `Commands/*.cs` (19 files)
- `Infrastructure/TypeRegistrar.cs`
- `InteractiveMode.cs`
- `Helpers/PagingHelper.cs`

### Files Modified:
- `ardalis.csproj`

---

## Final Project Structure

```
ardalis/
├── Behaviors/
│   └── PostHogTrackingBehavior.cs    # Cross-cutting analytics
├── Handlers/
│   ├── CardHandler.cs                 # Business card display
│   ├── QuoteHandler.cs                # Random quote
│   ├── TipHandler.cs                  # Random tip
│   ├── ReposHandler.cs                # GitHub repos table
│   ├── PackagesHandler.cs             # NuGet packages table
│   ├── BooksHandler.cs                # Books list
│   ├── CoursesHandler.cs              # Courses list
│   ├── RecentHandler.cs               # Recent activity
│   └── DotNetConfScoreHandler.cs      # .NET Conf rankings
├── Helpers/
│   ├── UrlHelper.cs                   # URL opening + UTM tracking
│   ├── QuoteHelper.cs                 # Quote fetching
│   ├── TipHelper.cs                   # Tip fetching
│   └── RecentHelper.cs                # Recent activity fetching
├── Telemetry/
│   ├── PostHogService.cs              # PostHog client
│   ├── LoggingHttpClientFactory.cs    # HTTP logging
│   └── ArdalisCliTelemetry.cs         # OTEL setup
├── Program.cs                         # Route definitions + DI
├── Urls.cs                            # URL constants
├── ArdalisApiClient.cs                # API client
└── ardalis.csproj
```

---

## Route Pattern Reference

### Final Route Definitions

| Route Pattern | Description |
|---------------|-------------|
| `blog` | Open Ardalis's blog |
| `bluesky` | Open Bluesky profile |
| `books --no-paging? --page-size {size:int?}` | Display published books |
| `card` | Display business card |
| `contact` | Open contact page |
| `courses --all? --page-size {size:int?}` | Display available courses |
| `dometrain` | Open Dometrain profile |
| `dotnetconf-score {year:int?}` | Display .NET Conf video rankings |
| `linkedin` | Open LinkedIn profile |
| `nimblepros` | Open NimblePros website |
| `packages --all? --page-size {size:int?}` | Display NuGet packages |
| `pluralsight` | Open Pluralsight profile |
| `quote` | Display random quote |
| `recent --verbose?` | Display recent activity |
| `repos` | Display GitHub repositories |
| `speaker` | Open Sessionize profile |
| `subscribe` | Open newsletter subscription |
| `tip` | Display random coding tip |
| `youtube` | Open YouTube channel |

---

## Benefits of Migration

1. **Simplified Architecture**
   - Route-based patterns replace command class hierarchy
   - No TypeRegistrar/TypeResolver bridge needed
   - Built-in REPL replaces custom InteractiveMode.cs

2. **Clean Separation of Concerns**
   - **Handlers contain only business logic**
   - Telemetry/analytics via single pipeline behavior
   - URL constants reusable across handlers

3. **Reduced Dependencies**
   - Remove: Spectre.Console, Spectre.Console.Cli, Spectre.Console.ImageSharp, Spectre.Console.Extensions.Logging
   - Add: TimeWarp.Nuru, Mediator.Abstractions, Mediator.SourceGenerator

4. **Code Reduction**
   - ~19 command class files → single Program.cs with route definitions
   - ~160 line InteractiveMode.cs → built-in REPL
   - ~78 line TypeRegistrar.cs → deleted
   - **19 `postHog.TrackCommand()` calls → 1 pipeline behavior**
   - **No UrlHandlers wrapper class - just constants + inline lambdas**

5. **Maintainability**
   - URLs defined once as constants, reusable everywhere
   - Add analytics to ALL commands by adding one behavior
   - Change tracking logic in one place
   - Test handlers without mocking PostHogService

---

## Validation Checklist

After each phase, verify:

- [ ] `dotnet build` succeeds with no errors
- [ ] `dotnet test` passes (if tests exist)
- [ ] Core commands work: `ardalis card`, `ardalis blog`, `ardalis quote`
- [ ] Options work: `ardalis packages --all`, `ardalis books --page-size 5`
- [ ] Arguments work: `ardalis dotnetconf-score 2024`
- [ ] Help works: `ardalis --help`, `ardalis books --help`
- [ ] Interactive mode works: `ardalis -i`
- [ ] Version check works: `ardalis --version`
- [ ] **PostHog tracking works for all commands (check PostHog dashboard)**

---

## References

- [TimeWarp.Nuru Documentation](https://github.com/TimeWarpEngineering/timewarp-nuru)
- [TimeWarp.Nuru v3.0.0-beta.11](https://www.nuget.org/packages/TimeWarp.Nuru)
- [Nuru Route Pattern Syntax](TimeWarp.Nuru MCP `get_syntax`)
- [Nuru Pipeline Middleware Example](samples/pipeline-middleware/pipeline-middleware.cs)
- [Nuru Unified Middleware Example](samples/unified-middleware/unified-middleware.cs)
- [martinothamar/Mediator](https://github.com/martinothamar/Mediator)

---

## Future Considerations

After completing the migration, evaluate these potential improvements:

### 1. Delegate Routes vs Mediator Commands

The migration plan uses delegate routes (`Map()`) for all handlers to keep the initial migration simple. Post-migration, consider converting handlers with dependencies to Mediator commands (`Map<TCommand>()`):

**Current (delegate with static HttpClient):**
```csharp
.Map("repos", ReposHandler.ExecuteAsync, "Display GitHub repositories")

// Handler creates its own HttpClient
public static class ReposHandler
{
    private static readonly HttpClient _httpClient = new();
    
    public static async Task ExecuteAsync()
    {
        var response = await _httpClient.GetAsync(...);
    }
}
```

**Improved (Mediator command with DI):**
```csharp
.Map<ReposCommand>("repos", "Display GitHub repositories")

public sealed class ReposCommand : IRequest
{
    public sealed class Handler : IRequestHandler<ReposCommand>
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<Handler> _logger;
        
        public Handler(HttpClient httpClient, ILogger<Handler> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        
        public async ValueTask<Unit> Handle(ReposCommand request, CancellationToken ct)
        {
            // Uses injected, properly configured HttpClient
        }
    }
}
```

**Benefits of Mediator commands:**
- Proper dependency injection (HttpClientFactory, ILogger, etc.)
- Easier unit testing with mocked dependencies
- Consistent with complex commands that have parameters
- Better lifecycle management for dependencies

**Candidates for conversion:**
| Handler | Dependencies | Priority |
|---------|-------------|----------|
| ReposHandler | HttpClient | Medium |
| PackagesHandler | HttpClient | Medium |
| BooksHandler | HttpClient | Medium |
| CoursesHandler | HttpClient | Medium |
| RecentHandler | HttpClient | Medium |
| DotNetConfScoreHandler | HttpClient, ArdalisApiClient | High |
| QuoteHandler | HttpClient | Low |
| TipHandler | HttpClient | Low |

**Keep as delegates:**
- URL commands (`blog`, `youtube`, etc.) - No dependencies
- `CardHandler` - Only uses static constants and terminal

### 2. IHttpClientFactory Integration

Replace static `HttpClient` instances with `IHttpClientFactory` for:
- Proper DNS refresh handling
- Connection pooling
- Named/typed clients with pre-configured base URLs
- Resilience policies (Polly)

```csharp
services.AddHttpClient("GitHub", client =>
{
    client.BaseAddress = new Uri("https://api.github.com/");
    client.DefaultRequestHeaders.Add("User-Agent", "ardalis-cli");
});

services.AddHttpClient("NuGet", client =>
{
    client.BaseAddress = new Uri("https://api-v2v3search-0.nuget.org/");
});
```

### 3. Additional Pipeline Behaviors

Consider adding more cross-cutting behaviors:

```csharp
services.AddMediator(opts =>
{
    opts.PipelineBehaviors =
    [
        typeof(PostHogTrackingBehavior<,>),    // Analytics (current)
        typeof(LoggingBehavior<,>),            // Structured logging
        typeof(PerformanceBehavior<,>),        // Slow command warnings
        typeof(RetryBehavior<,>),              // Retry transient HTTP failures
        typeof(CachingBehavior<,>),            // Cache API responses
    ];
});
```
