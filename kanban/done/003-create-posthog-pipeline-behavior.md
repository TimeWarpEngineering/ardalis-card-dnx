# Create PostHog Pipeline Behavior

## Summary

Implement PostHog tracking as a cross-cutting concern via single pipeline behavior instead of injecting PostHogService into every handler.

## Todo List

- [x] Create `Behaviors/` directory
- [x] Create `PostHogTrackingBehavior.cs` implementing `IPipelineBehavior<DelegateRequest, DelegateResponse>`
- [x] Inject `PostHogService` and `RouteExecutionContext`
- [x] Extract command name from `RouteExecutionContext.RoutePattern`
- [x] Call `_postHog.TrackCommand(commandName)` before `next()`
- [x] Verify `dotnet build` succeeds

## Notes

This is Phase 3 of the TimeWarp.Nuru migration.

The behavior uses `RouteExecutionContext.RoutePattern` to extract the command name. This works for delegate routes which are wrapped as `DelegateRequest` and flow through the Mediator pipeline.

Pattern extraction: `"packages --all? --page-size {size:int?}"` -> `"packages"`

File to create:
- `Behaviors/PostHogTrackingBehavior.cs`

Reference: `.agent/workspace/2025-12-02T16-30-00_timewarp-nuru-migration-plan.md` - Phase 3

## Results

- Created `Behaviors/PostHogTrackingBehavior.cs`
- Implements `IPipelineBehavior<DelegateRequest, DelegateResponse>` for delegate routes
- Injects `PostHogService`, `RouteExecutionContext`, and `ILogger`
- Extracts command name by splitting route pattern on space and taking first segment
- Build succeeds with 0 warnings, 0 errors
