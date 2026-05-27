# Release

Packages publish from `main` to GitHub Packages on merge. nuget.org on GitHub Release.

Version: `build/version.json` + CI build number (`YEAR.MAJOR.MINOR.BUILD`).

Consumers:

```xml
<PackageReference Include="Novolis.Workspaces.Timeline" Version="2026.1.*" />
```
