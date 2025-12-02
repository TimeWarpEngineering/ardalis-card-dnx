# Create Courses Handler

## Summary

Create CoursesHandler using Nuru panel widget to display available courses with paging support.

## Todo List

- [x] Create `Handlers/CoursesHandler.cs`
- [x] Use `NuruTerminal.Default` for output
- [x] Port `FetchCoursesFromUrl()` logic from existing CoursesCommand
- [x] Port fallback courses list
- [x] Accept `bool all` and `int pageSize` parameters
- [x] Group courses by platform
- [x] Display each course in a panel
- [x] Implement paging logic (Space to continue)
- [x] Verify `dotnet build` succeeds

## Notes

This is Phase 4h of the TimeWarp.Nuru migration.

CoursesHandler groups courses by platform (Pluralsight, Dometrain, etc.) before displaying.

File to create:
- `Handlers/CoursesHandler.cs`

Reference: `.agent/workspace/2025-12-02T16-30-00_timewarp-nuru-migration-plan.md` - Phase 4

## Results

- Created `Handlers/CoursesHandler.cs` (197 lines)
- Uses `NuruTerminal.Default` for all output
- Async `ExecuteAsync(bool showAll, int pageSize)` method
- Fetches courses from ardalis.com/courses.json with fallback to hardcoded list
- Courses grouped by platform (Pluralsight, Dometrain, etc.) with headers
- Panel widget with course name, description, and clickable link
- Paging with Space key to continue (or showAll to display without paging)
- UTM tracking on all links via `UrlHelper.AddUtmSource()`
- Build succeeds with 0 warnings, 0 errors
