# Update README.md Post Nuru Migration

## Summary

Remove outdated Spectre.Console references and deleted `--with-covers` option from README.md following the TimeWarp.Nuru migration in PR #53.

## Todo List

- [ ] Remove `--with-covers` option from books command example (line 103)
- [ ] Remove `--with-covers` from permanent installation examples (line 259)
- [ ] Replace Spectre.Console reference with TimeWarp.Nuru in Features section (line 288)

## Notes

**Context:** PR #53 migrated from Spectre.Console to TimeWarp.Nuru and removed ImageSharp dependency along with the `--with-covers` option.

**Current books command signature:** `books --no-paging? --page-size? {size:int?}`

**Line 288 change:**
- Current: `Beautiful terminal UI with [Spectre.Console](https://spectreconsole.net/)`
- Should be: `Beautiful terminal UI with [TimeWarp.Nuru](https://github.com/TimeWarpEngineering/timewarp-nuru)`
