# Create PostHog Pipeline Behavior

## Summary

Implement PostHog tracking as a cross-cutting concern via single pipeline behavior instead of injecting PostHogService into every handler.

## Todo List

- [ ] Create `Behaviors/` directory
- [ ] Create `PostHogTrackingBehavior.cs` implementing `IPipelineBehavior<TMessage, TResponse>`
- [ ] Inject `PostHogService` and `RouteExecutionContext`
- [ ] Extract command name from `RouteExecutionContext.RoutePattern`
- [ ] Call `_postHog.TrackCommand(commandName)` before `next()`
- [ ] Verify `dotnet build` succeeds

## Notes

This is Phase 3 of the TimeWarp.Nuru migration.

The behavior uses `RouteExecutionContext.RoutePattern` to extract the command name. This works for both delegate routes and Mediator commands since delegates are wrapped as `DelegateRequest` and flow through the same pipeline.

Pattern extraction: `"packages --all? --page-size {size:int?}"` -> `"packages"`

File to create:
- `Behaviors/PostHogTrackingBehavior.cs`

Reference: `.agent/workspace/2025-12-02T16-30-00_timewarp-nuru-migration-plan.md` - Phase 3

## Results

