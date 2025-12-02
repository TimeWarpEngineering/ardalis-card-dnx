# Create Packages Handler

## Summary

Create PackagesHandler using Nuru table widget to display NuGet packages with paging support.

## Todo List

- [ ] Create `Handlers/PackagesHandler.cs`
- [ ] Use `NuruTerminal.Default` for output
- [ ] Port `GetPackagesFromApi()` logic from existing PackagesCommand
- [ ] Port fallback packages list
- [ ] Accept `bool all` and `int pageSize` parameters
- [ ] Display packages in table with columns: Package, Downloads, Description
- [ ] Implement paging logic (Space to continue)
- [ ] Use URL constant for NuGet profile link
- [ ] Verify `dotnet build` succeeds

## Notes

This is Phase 4f of the TimeWarp.Nuru migration.

PackagesHandler has optional parameters `--all?` and `--page-size {size:int?}`. These map to handler parameters.

File to create:
- `Handlers/PackagesHandler.cs`

Reference: `.agent/workspace/2025-12-02T16-30-00_timewarp-nuru-migration-plan.md` - Phase 4

## Results

