# Update TimeWarp.Nuru to beta.15

## Summary

Update the TimeWarp.Nuru package from version 3.0.0-beta.12 to 3.0.0-beta.15.

## Todo List

- [x] Update `ardalis.csproj` PackageReference version from `3.0.0-beta.12` to `3.0.0-beta.15`
- [x] Run `dotnet restore` to fetch the new package
- [x] Run `dotnet build` to verify compilation succeeds
- [x] Update `Program.cs` to remove return statement from MapDefault (API change)

## Notes

Original target was beta.13, but updated to beta.15 to get latest improvements.

## Results

- Updated to beta.15 instead of beta.13
- Removed `return 0;` from MapDefault handler - Nuru API change means MapDefault now uses Action delegate instead of Func<int>
