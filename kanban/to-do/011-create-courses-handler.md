# Create Courses Handler

## Summary

Create CoursesHandler using Nuru panel widget to display available courses with paging support.

## Todo List

- [ ] Create `Handlers/CoursesHandler.cs`
- [ ] Use `NuruTerminal.Default` for output
- [ ] Port `FetchCoursesFromUrl()` logic from existing CoursesCommand
- [ ] Port fallback courses list
- [ ] Accept `bool all` and `int pageSize` parameters
- [ ] Group courses by platform
- [ ] Display each course in a panel
- [ ] Implement paging logic (Space to continue)
- [ ] Verify `dotnet build` succeeds

## Notes

This is Phase 4h of the TimeWarp.Nuru migration.

CoursesHandler groups courses by platform (Pluralsight, Dometrain, etc.) before displaying.

File to create:
- `Handlers/CoursesHandler.cs`

Reference: `.agent/workspace/2025-12-02T16-30-00_timewarp-nuru-migration-plan.md` - Phase 4

## Results

