# Update README.md Post Nuru Migration

## Summary

Remove outdated Spectre.Console references and deleted `--with-covers` option from README.md following the TimeWarp.Nuru migration in PR #53.

## Todo List

- [x] Remove `--with-covers` option from books command example (line 103)
- [x] Remove `--with-covers` from permanent installation examples (line 259)
- [x] Replace Spectre.Console reference with TimeWarp.Nuru in Features section (line 288)

## Results

All three changes completed:
1. Removed `--with-covers` example from Quick Start books section
2. Removed `--with-covers` line from Permanent Installation examples
3. Updated Features section to reference TimeWarp.Nuru instead of Spectre.Console

## Notes

**Context:** PR #53 migrated from Spectre.Console to TimeWarp.Nuru and removed ImageSharp dependency along with the `--with-covers` option.

**Current books command signature:** `books --no-paging? --page-size? {size:int?}`

**Line 288 change:**
- Current: `Beautiful terminal UI with [Spectre.Console](https://spectreconsole.net/)`
- Should be: `Beautiful terminal UI with [TimeWarp.Nuru](https://github.com/TimeWarpEngineering/timewarp-nuru)`
