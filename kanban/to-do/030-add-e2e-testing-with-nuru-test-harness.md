# Add E2E Testing with Nuru Test Harness

## Summary

Implement end-to-end testing for the ardalis CLI using Nuru's runfile test harness pattern. This zero-modification approach uses conditional compilation via `Directory.Build.props` to include test code only when `NURU_TEST` environment variable is set. Tests use `TestTerminal` to capture output and Jaribu for test discovery/execution.

## Todo List

- [ ] Create `Directory.Build.props` in repo root with conditional test inclusion
- [ ] Create `test-ardalis.cs` test harness with `[ModuleInitializer]`
- [ ] Implement card command tests (display, links, default command)
- [ ] Implement version command test (semver format)
- [ ] Implement help command tests (lists commands, descriptions)
- [ ] Implement error handling test (unknown command returns exit code 1)
- [ ] Implement smoke tests for network-dependent commands (repos, packages, books, courses, recent, dotnetconf-score)
- [ ] Create `run-tests.cs` automated test runner script (optional)
- [ ] Verify tests pass locally with `NURU_TEST=test-ardalis.cs dotnet run`
- [ ] Add CI/CD workflow for e2e tests (`.github/workflows/test.yml`)

## Notes

### Pattern Overview

The Nuru runfile test harness pattern allows testing CLI apps without modifying production code:

1. **`Directory.Build.props`** - Conditionally compiles test file + packages when `NURU_TEST` is set
2. **`[ModuleInitializer]`** - Runs before `Main()`, sets up `NuruTestContext.TestRunner`
3. **`NuruTestContext.TestRunner`** - Intercepts `RunAsync()` to run tests instead of normal routing
4. **`TestTerminal`** - Captures stdout/stderr for assertions

### File 1: `Directory.Build.props`

```xml
<Project>
  <!-- 
    Conditional test harness inclusion for e2e testing.
    
    When NURU_TEST environment variable is set to a filename (e.g., test-ardalis.cs),
    that file is included in the build. This allows test code with [ModuleInitializer]
    to set up NuruTestContext.TestRunner before Main() runs.
    
    Usage:
      export NURU_TEST=test-ardalis.cs
      dotnet clean
      dotnet run
    
    NOTE: Run `dotnet clean` after setting/unsetting NURU_TEST
    to rebuild with the new configuration.
  -->
  <ItemGroup Condition="'$(NURU_TEST)' != ''">
    <Compile Include="$(NURU_TEST)" />
    <PackageReference Include="Shouldly" Version="4.3.0" />
    <PackageReference Include="TimeWarp.Jaribu" Version="3.0.0-beta.3" />
  </ItemGroup>
</Project>
```

### File 2: `test-ardalis.cs`

```csharp
// test-ardalis.cs - E2E Test harness for ardalis CLI using Jaribu
// This file is included at build time via Directory.Build.props when NURU_TEST is set
// Usage: NURU_TEST=test-ardalis.cs dotnet run

using System.Runtime.CompilerServices;
using Shouldly;
using TimeWarp.Jaribu;
using TimeWarp.Nuru;
using static TimeWarp.Jaribu.TestRunner;

public static class TestHarness
{
  internal static NuruCoreApp? App;

  [ModuleInitializer]
  public static void Initialize()
  {
    NuruTestContext.TestRunner = async (app) =>
    {
      App = app;
      return await RunTests<ArdalisCliTests>(clearCache: false);
    };
  }
}

[TestTag("ArdalisCli")]
public class ArdalisCliTests
{
  public static async Task CleanUp()
  {
    TestTerminalContext.Current = null;
    await Task.CompletedTask;
  }

  // Card Command Tests
  public static async Task Should_display_card_with_ardalis_name()
  {
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;

    await TestHarness.App!.RunAsync(["card"]);

    terminal.OutputContains("Ardalis").ShouldBeTrue("Card should contain 'Ardalis'");
    terminal.OutputContains("Steve").ShouldBeTrue("Card should contain 'Steve'");
    await Task.CompletedTask;
  }

  public static async Task Should_display_card_with_website_links()
  {
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;

    await TestHarness.App!.RunAsync(["card"]);

    terminal.OutputContains("ardalis.com").ShouldBeTrue("Card should contain blog URL");
    terminal.OutputContains("nimblepros.com").ShouldBeTrue("Card should contain NimblePros URL");
    await Task.CompletedTask;
  }

  public static async Task Should_display_card_as_default_command()
  {
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;

    await TestHarness.App!.RunAsync([]);

    terminal.OutputContains("Ardalis").ShouldBeTrue("Default command should show card");
    await Task.CompletedTask;
  }

  // Version Command Test
  public static async Task Should_display_version_number()
  {
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;

    await TestHarness.App!.RunAsync(["version"]);

    terminal.Output.ShouldMatch(@"\d+\.\d+\.\d+", "Version should be in semver format");
    await Task.CompletedTask;
  }

  // Help Command Tests
  public static async Task Should_display_help_with_available_commands()
  {
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;

    await TestHarness.App!.RunAsync(["help"]);

    terminal.OutputContains("card").ShouldBeTrue("Help should list 'card' command");
    terminal.OutputContains("blog").ShouldBeTrue("Help should list 'blog' command");
    terminal.OutputContains("github").ShouldBeTrue("Help should list 'github' command");
    await Task.CompletedTask;
  }

  // Error Handling Test
  public static async Task Should_return_error_for_unknown_command()
  {
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;

    int exitCode = await TestHarness.App!.RunAsync(["unknown-command-xyz"]);

    exitCode.ShouldBe(1, "Unknown command should return exit code 1");
    await Task.CompletedTask;
  }

  // Smoke Tests for Network-Dependent Commands
  public static async Task Should_handle_repos_command()
  {
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;
    int exitCode = await TestHarness.App!.RunAsync(["repos"]);
    exitCode.ShouldBeOneOf(0, 1);
    await Task.CompletedTask;
  }

  public static async Task Should_handle_packages_command()
  {
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;
    int exitCode = await TestHarness.App!.RunAsync(["packages"]);
    exitCode.ShouldBeOneOf(0, 1);
    await Task.CompletedTask;
  }

  public static async Task Should_handle_books_command()
  {
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;
    int exitCode = await TestHarness.App!.RunAsync(["books"]);
    exitCode.ShouldBeOneOf(0, 1);
    await Task.CompletedTask;
  }

  public static async Task Should_handle_courses_command()
  {
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;
    int exitCode = await TestHarness.App!.RunAsync(["courses"]);
    exitCode.ShouldBeOneOf(0, 1);
    await Task.CompletedTask;
  }

  public static async Task Should_handle_recent_command()
  {
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;
    int exitCode = await TestHarness.App!.RunAsync(["recent"]);
    exitCode.ShouldBeOneOf(0, 1);
    await Task.CompletedTask;
  }

  public static async Task Should_handle_dotnetconf_score_command()
  {
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;
    int exitCode = await TestHarness.App!.RunAsync(["dotnetconf-score"]);
    exitCode.ShouldBeOneOf(0, 1);
    await Task.CompletedTask;
  }
}
```

### File 3: `.github/workflows/test.yml` (CI/CD)

```yaml
name: E2E Tests

on:
  push:
    branches: [main, master]
  pull_request:
    branches: [main, master]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      
      - name: Run E2E Tests
        env:
          NURU_TEST: test-ardalis.cs
        run: |
          dotnet clean
          dotnet run
```

### How to Run Tests Locally

```bash
# Set env var and run
export NURU_TEST=test-ardalis.cs
dotnet clean
dotnet run

# Cleanup after testing
unset NURU_TEST
dotnet clean
```

### Key Points

- **Zero changes to Program.cs** - production code stays untouched
- **Conditional compilation** - test code only included when `NURU_TEST` is set
- **Must `dotnet clean`** after changing `NURU_TEST` - cache doesn't track env vars
- **TestTerminal** captures output - use `OutputContains()`, `ErrorContains()`, `GetOutputLines()`
- **Jaribu naming convention** - test methods must start with `Should_`

### Reference Documentation

- Analysis: `.agent/workspace/2025-12-10T15-00-00_e2e-testing-implementation-plan.md`
- Nuru samples: https://github.com/TimeWarpEngineering/timewarp-nuru/tree/master/samples/testing/runfile-test-harness

## Results

_To be filled after completion._
