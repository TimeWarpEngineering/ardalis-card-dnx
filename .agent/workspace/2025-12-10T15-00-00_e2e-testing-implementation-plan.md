# E2E Testing Implementation Plan for Ardalis CLI

## Executive Summary

This document provides a complete implementation plan for adding e2e testing to the ardalis CLI using the Nuru **runfile test harness** pattern. This approach requires **zero modification** to the production `Program.cs` - tests are conditionally compiled via `Directory.Build.props` when the `NURU_TEST` environment variable is set.

## Scope

- Add e2e testing capability using Nuru's `TestTerminal` and `NuruTestContext`
- Use TimeWarp.Jaribu for test discovery and execution
- Create tests for key CLI commands: `card`, `quote`, `tip`, `version`, `help`
- Provide automated test runner script

## Files to Create

### 1. `Directory.Build.props` (new file in repo root)

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
    
    The test harness receives the fully configured NuruCoreApp and can run 
    multiple test scenarios against it.
    
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

### 2. `test-ardalis.cs` (new file in repo root)

```csharp
// test-ardalis.cs - E2E Test harness for ardalis CLI using Jaribu
// This file is included at build time via Directory.Build.props when NURU_TEST is set
// Usage: NURU_TEST=test-ardalis.cs dotnet run
//
// The ModuleInitializer sets up NuruTestContext.TestRunner before Main() runs.
// When Program.cs calls RunAsync(), control is handed to our test runner.

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
      App = app;  // Capture the real app
      return await RunTests<ArdalisCliTests>(clearCache: false);
    };
  }
}

[TestTag("ArdalisCli")]
public class ArdalisCliTests
{
  public static async Task CleanUp()
  {
    // Reset terminal context after each test
    TestTerminalContext.Current = null;
    await Task.CompletedTask;
  }

  // ========================================
  // Card Command Tests
  // ========================================

  public static async Task Should_display_card_with_ardalis_name()
  {
    // Arrange
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;

    // Act
    await TestHarness.App!.RunAsync(["card"]);

    // Assert
    terminal.OutputContains("Ardalis").ShouldBeTrue("Card should contain 'Ardalis'");
    terminal.OutputContains("Steve").ShouldBeTrue("Card should contain 'Steve'");
    await Task.CompletedTask;
  }

  public static async Task Should_display_card_with_website_links()
  {
    // Arrange
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;

    // Act
    await TestHarness.App!.RunAsync(["card"]);

    // Assert
    terminal.OutputContains("ardalis.com").ShouldBeTrue("Card should contain blog URL");
    terminal.OutputContains("nimblepros.com").ShouldBeTrue("Card should contain NimblePros URL");
    await Task.CompletedTask;
  }

  public static async Task Should_display_card_as_default_command()
  {
    // Arrange
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;

    // Act - no arguments should show card
    await TestHarness.App!.RunAsync([]);

    // Assert
    terminal.OutputContains("Ardalis").ShouldBeTrue("Default command should show card");
    await Task.CompletedTask;
  }

  // ========================================
  // Version Command Tests
  // ========================================

  public static async Task Should_display_version_number()
  {
    // Arrange
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;

    // Act
    await TestHarness.App!.RunAsync(["version"]);

    // Assert - version should match semantic versioning pattern
    string output = terminal.Output;
    output.ShouldMatch(@"\d+\.\d+\.\d+", "Version should be in semver format");
    await Task.CompletedTask;
  }

  // ========================================
  // Help Command Tests
  // ========================================

  public static async Task Should_display_help_with_available_commands()
  {
    // Arrange
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;

    // Act
    await TestHarness.App!.RunAsync(["help"]);

    // Assert - help should list available commands
    terminal.OutputContains("card").ShouldBeTrue("Help should list 'card' command");
    terminal.OutputContains("blog").ShouldBeTrue("Help should list 'blog' command");
    terminal.OutputContains("github").ShouldBeTrue("Help should list 'github' command");
    terminal.OutputContains("quote").ShouldBeTrue("Help should list 'quote' command");
    await Task.CompletedTask;
  }

  public static async Task Should_display_help_with_descriptions()
  {
    // Arrange
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;

    // Act
    await TestHarness.App!.RunAsync(["help"]);

    // Assert - help should include command descriptions
    terminal.OutputContains("business card").ShouldBeTrue("Help should describe card command");
    await Task.CompletedTask;
  }

  // ========================================
  // Error Handling Tests
  // ========================================

  public static async Task Should_return_error_for_unknown_command()
  {
    // Arrange
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;

    // Act
    int exitCode = await TestHarness.App!.RunAsync(["unknown-command-xyz"]);

    // Assert
    exitCode.ShouldBe(1, "Unknown command should return exit code 1");
    await Task.CompletedTask;
  }

  // ========================================
  // URL Command Tests (verify they don't crash)
  // ========================================

  // Note: URL commands open a browser, so we just verify they don't throw
  // In a real test, you'd mock the URL opener

  // ========================================
  // Commands with Options Tests
  // ========================================

  public static async Task Should_handle_packages_command()
  {
    // Arrange
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;

    // Act - this may fail if network is unavailable, but shouldn't crash
    int exitCode = await TestHarness.App!.RunAsync(["packages"]);

    // Assert - command should complete (0) or fail gracefully (non-zero)
    // We're mainly testing that it doesn't throw
    exitCode.ShouldBeOneOf(0, 1);
    await Task.CompletedTask;
  }

  public static async Task Should_handle_books_command()
  {
    // Arrange
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;

    // Act
    int exitCode = await TestHarness.App!.RunAsync(["books"]);

    // Assert
    exitCode.ShouldBeOneOf(0, 1);
    await Task.CompletedTask;
  }

  public static async Task Should_handle_repos_command()
  {
    // Arrange
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;

    // Act
    int exitCode = await TestHarness.App!.RunAsync(["repos"]);

    // Assert
    exitCode.ShouldBeOneOf(0, 1);
    await Task.CompletedTask;
  }

  public static async Task Should_handle_courses_command()
  {
    // Arrange
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;

    // Act
    int exitCode = await TestHarness.App!.RunAsync(["courses"]);

    // Assert
    exitCode.ShouldBeOneOf(0, 1);
    await Task.CompletedTask;
  }

  public static async Task Should_handle_recent_command()
  {
    // Arrange
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;

    // Act
    int exitCode = await TestHarness.App!.RunAsync(["recent"]);

    // Assert
    exitCode.ShouldBeOneOf(0, 1);
    await Task.CompletedTask;
  }

  public static async Task Should_handle_dotnetconf_score_command()
  {
    // Arrange
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;

    // Act
    int exitCode = await TestHarness.App!.RunAsync(["dotnetconf-score"]);

    // Assert
    exitCode.ShouldBeOneOf(0, 1);
    await Task.CompletedTask;
  }

  public static async Task Should_handle_dotnetconf_score_with_year()
  {
    // Arrange
    using TestTerminal terminal = new();
    TestTerminalContext.Current = terminal;

    // Act
    int exitCode = await TestHarness.App!.RunAsync(["dotnetconf-score", "2024"]);

    // Assert
    exitCode.ShouldBeOneOf(0, 1);
    await Task.CompletedTask;
  }
}
```

### 3. `run-tests.cs` (optional - automated test runner)

```csharp
#!/usr/bin/dotnet run
// run-tests.cs - Automated test runner for ardalis CLI e2e tests
// Run with: dotnet run run-tests.cs
// Or make executable: chmod +x run-tests.cs && ./run-tests.cs

using System;
using System.Diagnostics;

Console.WriteLine("=== Ardalis CLI E2E Test Runner ===\n");

string testFile = "test-ardalis.cs";
string projectDir = AppContext.BaseDirectory;

// Step 1: Set up test environment
Console.WriteLine($"1. Setting NURU_TEST={testFile}");
Environment.SetEnvironmentVariable("NURU_TEST", testFile);

// Step 2: Clean to force rebuild with test harness
Console.WriteLine("2. Cleaning project to force rebuild...");
var cleanProcess = Process.Start(new ProcessStartInfo
{
    FileName = "dotnet",
    Arguments = "clean",
    WorkingDirectory = projectDir,
    RedirectStandardOutput = true,
    RedirectStandardError = true
});
cleanProcess?.WaitForExit();

if (cleanProcess?.ExitCode != 0)
{
    Console.WriteLine("   Clean failed!");
    return 1;
}
Console.WriteLine("   Clean succeeded");

// Step 3: Run tests
Console.WriteLine("3. Running tests...\n");
var testProcess = Process.Start(new ProcessStartInfo
{
    FileName = "dotnet",
    Arguments = "run",
    WorkingDirectory = projectDir,
    RedirectStandardOutput = true,
    RedirectStandardError = true
});

string stdout = testProcess?.StandardOutput.ReadToEnd() ?? "";
string stderr = testProcess?.StandardError.ReadToEnd() ?? "";
testProcess?.WaitForExit();

Console.WriteLine(stdout);
if (!string.IsNullOrEmpty(stderr))
    Console.WriteLine(stderr);

bool testsPassed = testProcess?.ExitCode == 0;

// Step 4: Clean up
Console.WriteLine("\n4. Cleaning up test environment...");
Environment.SetEnvironmentVariable("NURU_TEST", null);

Console.WriteLine("5. Cleaning project to rebuild without test harness...");
var cleanupProcess = Process.Start(new ProcessStartInfo
{
    FileName = "dotnet",
    Arguments = "clean",
    WorkingDirectory = projectDir,
    RedirectStandardOutput = true,
    RedirectStandardError = true
});
cleanupProcess?.WaitForExit();

// Summary
Console.WriteLine("\n=== Summary ===");
if (testsPassed)
{
    Console.WriteLine("All tests passed!");
    return 0;
}
else
{
    Console.WriteLine("Tests failed!");
    return 1;
}
```

## How to Run Tests

### Manual Approach

```bash
# 1. Set environment variable
export NURU_TEST=test-ardalis.cs    # Linux/macOS
$env:NURU_TEST = "test-ardalis.cs"  # PowerShell

# 2. Clean to force rebuild with test harness
dotnet clean

# 3. Run - tests execute instead of normal app
dotnet run

# 4. Clean up after testing
unset NURU_TEST                      # Linux/macOS
Remove-Item Env:NURU_TEST            # PowerShell
dotnet clean
```

### Automated Approach (with run-tests.cs)

```bash
dotnet run run-tests.cs
```

## How It Works

```
┌─────────────────────────────────────────────────────────────────┐
│                    Build Time (MSBuild)                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Directory.Build.props checks NURU_TEST env var                 │
│                                                                  │
│  If NURU_TEST="test-ardalis.cs":                                │
│    - Compile Include="test-ardalis.cs"                          │
│    - PackageReference Include="Shouldly"                        │
│    - PackageReference Include="TimeWarp.Jaribu"                 │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Runtime Execution                             │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  [ModuleInitializer] in test-ardalis.cs runs BEFORE Main()      │
│    └── Sets NuruTestContext.TestRunner = async (app) => {...}   │
│                                                                  │
│  Main() runs normally                                            │
│    └── Builds NuruCoreApp with all routes                       │
│    └── Calls app.RunAsync(args)                                  │
│                                                                  │
│  NuruCoreApp.RunAsync() checks NuruTestContext.TestRunner       │
│    └── If set: Calls TestRunner(app) instead of routing         │
│    └── TestRunner captures app, runs Jaribu tests               │
│                                                                  │
│  Jaribu discovers test methods (Should_*) and executes them     │
│    └── Each test uses TestTerminal to capture output            │
│    └── Assertions verify CLI behavior                            │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

## Key Concepts

### 1. Zero Production Code Changes
The `Program.cs` remains unchanged. The test harness is conditionally compiled only when `NURU_TEST` is set.

### 2. `[ModuleInitializer]`
This C# 9 attribute marks a method to run before `Main()`. It sets up `NuruTestContext.TestRunner` which intercepts `RunAsync()`.

### 3. `TestTerminal`
A fake terminal that captures stdout/stderr for assertions:
- `terminal.Output` - captured stdout
- `terminal.ErrorOutput` - captured stderr  
- `terminal.OutputContains("text")` - check if output contains text
- `terminal.GetOutputLines()` - get output as array of lines

### 4. `TestTerminalContext.Current`
Nuru checks this before using the real terminal. Set it to a `TestTerminal` instance to capture output.

### 5. Jaribu Test Framework
- Test methods use `Should_` naming convention
- `CleanUp()` method runs after each test
- `[TestTag]` for organization
- `[Skip("reason")]` to skip tests
- `[Input]` for data-driven tests

## Test Coverage Recommendations

| Command | Test Priority | Notes |
|---------|--------------|-------|
| `card` | High | Core functionality, always available |
| `version` | High | Version format validation |
| `help` | High | Lists all commands |
| Default (no args) | High | Should show card |
| Unknown command | High | Error handling |
| `quote` | Medium | Depends on external API |
| `tip` | Medium | Depends on external API |
| `repos` | Medium | Depends on GitHub API |
| `packages` | Medium | Depends on NuGet API |
| `books` | Medium | Depends on external API |
| `courses` | Medium | Depends on external API |
| `recent` | Medium | Depends on RSS feeds |
| `dotnetconf-score` | Low | Depends on YouTube API |
| URL commands | Low | Just verify no crash |

## CI/CD Integration

Add to `.github/workflows/test.yml`:

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

## References

- [Nuru Testing Documentation](https://github.com/TimeWarpEngineering/timewarp-nuru/tree/master/samples/testing)
- [Jaribu Test Framework](https://github.com/TimeWarpEngineering/timewarp-jaribu)
- [Shouldly Assertions](https://github.com/shouldly/shouldly)
