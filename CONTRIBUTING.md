# Contributing to ardalis-card-dnx

## For Maintainers

### Publishing a New Version

This project uses GitHub Actions to automatically publish to NuGet.org when a new release is created.

#### Steps to Publish

1. **Update the version and release notes** in `ardalis.csproj`:

   ```xml
   <Version>1.3.0</Version>
   <ReleaseNotes>Added 'repos' command - Display popular GitHub repositories.</ReleaseNotes>
   ```

2. **Update the README.md** if needed to document new features.

3. **Commit and push** your changes:

   ```bash
   git add .
   git commit -m "Bump version to 1.3.0"
   git push origin main
   ```

4. **Create a GitHub Release**:
   - Go to [GitHub Releases](https://github.com/ardalis/ardalis-card-dnx/releases/new)
   - Create a **new tag** (e.g., `v1.3.0`) directly in the release form
   - Add release notes describing the changes
   - Click **Publish release**

5. **Monitor the workflow**:
   - The GitHub Action will automatically trigger and publish to NuGet.org
   - Check progress at [GitHub Actions](https://github.com/ardalis/ardalis-card-dnx/actions)

**Note**: Creating the release will automatically create the tag, so you don't need to create and push tags separately. This prevents duplicate workflow runs.

#### Manual Publishing (Alternative)

If you need to publish manually:

```bash
dotnet pack -c Release
dotnet nuget push bin\Release\ardalis.1.0.3.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

### GitHub Action Setup

The automated workflow requires:

- **GitHub Secret**: `NUGET_USER` set to your NuGet.org username
- **NuGet.org Configuration**: Package configured to trust GitHub Actions OIDC for `ardalis/ardalis-card-dnx`

## For Contributors

Contributions are welcome! Feel free to:

- Report issues
- Suggest improvements
- Submit pull requests

This is a simple project, so just ensure your code works and follows the existing style.
