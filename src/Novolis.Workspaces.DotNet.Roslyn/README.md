# Novolis.Workspaces.DotNet.Roslyn

Portable C# semantic projections loaded through Roslyn.

## Install

```powershell
dotnet add package Novolis.Workspaces.DotNet.Roslyn
```

## Quick start

Set `EvaluationContext.AllowDesignTimeBuilds` to `true` only for trusted roots. The loader returns
documents, declared types, and diagnostics without exposing Roslyn runtime objects.
