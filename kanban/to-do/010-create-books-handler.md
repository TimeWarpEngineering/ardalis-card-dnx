# Create Books Handler

## Summary

Create BooksHandler using Nuru panel widget to display published books with paging support.

## Todo List

- [ ] Create `Handlers/BooksHandler.cs`
- [ ] Use `NuruTerminal.Default` for output
- [ ] Port book fetching logic from existing BooksCommand
- [ ] Port fallback books list
- [ ] Accept `bool noPaging` and `int pageSize` parameters
- [ ] Display each book in a panel
- [ ] Implement paging logic (Space to continue)
- [ ] Verify `dotnet build` succeeds

## Notes

This is Phase 4g of the TimeWarp.Nuru migration.

BooksHandler does NOT include --with-covers (removed in task 002). Parameters are `--no-paging?` and `--page-size {size:int?}`.

File to create:
- `Handlers/BooksHandler.cs`

Reference: `.agent/workspace/2025-12-02T16-30-00_timewarp-nuru-migration-plan.md` - Phase 4

## Results

