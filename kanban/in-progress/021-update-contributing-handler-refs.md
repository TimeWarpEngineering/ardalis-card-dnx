# Update CONTRIBUTING.md Handler References

## Summary

Update "Used by" references in CONTRIBUTING.md from deleted Command classes to current Handler classes following the TimeWarp.Nuru migration.

## Todo List

- [ ] Change `QuoteCommand` to `QuoteHandler` (line 85)
- [ ] Change `BooksCommand` to `BooksHandler` (line 110)
- [ ] Change `TipsCommand` to `TipHandler` (line 133)
- [ ] Change `CoursesCommand` to `CoursesHandler` (line 161)

## Notes

**Context:** PR #53 deleted all `Commands/*.cs` files and created new `Handlers/*.cs` files.

**Current handler files:**
- `Handlers/QuoteHandler.cs` - uses `QuoteHelper`
- `Handlers/BooksHandler.cs` - standalone
- `Handlers/TipHandler.cs` - uses `TipHelper`
- `Handlers/CoursesHandler.cs` - standalone
