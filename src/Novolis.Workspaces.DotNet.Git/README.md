# Novolis.Workspaces.DotNet.Git

Explicit relationships between typed .NET solution and Git repository workspaces.

## Install

```powershell
dotnet add package Novolis.Workspaces.DotNet.Git
```

## Quick start

`SolutionRepositoryRelation.Create` identifies equal roots, solutions nested under a repository,
or unrelated roots without treating the two workspace domains as interchangeable.
