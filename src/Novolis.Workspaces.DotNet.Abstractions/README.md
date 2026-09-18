# Novolis.Workspaces.DotNet.Abstractions

Typed contracts for opening a .NET solution from an I/O workspace root.

## Install

```powershell
dotnet add package Novolis.Workspaces.DotNet.Abstractions
```

## Quick start

Use `SolutionWorkspace` for a solution file and `EvaluationContext` to explicitly opt into
MSBuild evaluation or Roslyn design-time loading.
