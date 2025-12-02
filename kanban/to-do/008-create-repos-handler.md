# Create Repos Handler

## Summary

Create ReposHandler using Nuru table widget to display popular GitHub repositories.

## Todo List

- [ ] Create `Handlers/ReposHandler.cs`
- [ ] Use `NuruTerminal.Default` for output
- [ ] Port `FetchReposAsync()` logic from existing ReposCommand
- [ ] Display repos in table with columns: Repository, Stars, Description
- [ ] Use `terminal.WriteTable()` with Nuru table builder
- [ ] Add hyperlinks to repository names
- [ ] Use URL constant for GitHub profile link
- [ ] Verify `dotnet build` succeeds

## Notes

This is Phase 4e of the TimeWarp.Nuru migration.

ReposHandler demonstrates the Table widget. The existing ReposCommand uses a static HttpClient - keep this pattern for now (see Future Considerations for IHttpClientFactory).

File to create:
- `Handlers/ReposHandler.cs`

Reference: `.agent/workspace/2025-12-02T16-30-00_timewarp-nuru-migration-plan.md` - Phase 4

## Results

