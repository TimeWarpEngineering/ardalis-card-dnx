# Remove ImageSharp Dependency

## Summary

Remove `--with-covers` option from BooksCommand and remove Spectre.Console.ImageSharp package.

## Todo List

- [ ] Remove `WithCovers` property from `BooksCommand.Settings`
- [ ] Remove image rendering code from `DisplayBook` method
- [ ] Update `DisplayBook` signature (remove `withCovers` parameter)
- [ ] Update caller in `ExecuteAsync` 
- [ ] Remove ImageSharp using statements
- [ ] Remove `Spectre.Console.ImageSharp` package reference from csproj
- [ ] Verify `dotnet build` succeeds
- [ ] Verify `ardalis books` works without --with-covers
- [ ] Verify `ardalis books --help` no longer shows --with-covers option

## Notes

This is Phase 2 of the TimeWarp.Nuru migration. The --with-covers feature is being removed entirely, not replaced.

Files to modify:
- `Commands/BooksCommand.cs`
- `ardalis.csproj`

Reference: `.agent/workspace/2025-12-02T16-30-00_timewarp-nuru-migration-plan.md` - Phase 2

## Results

