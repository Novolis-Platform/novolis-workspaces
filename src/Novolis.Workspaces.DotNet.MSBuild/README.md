# Novolis.Workspaces.DotNet.MSBuild

Context-aware, target-free MSBuild project evaluation.

## Install

```powershell
dotnet add package Novolis.Workspaces.DotNet.MSBuild
```

## Quick start

Set `EvaluationContext.AllowEvaluation` to `true` before evaluating a project. Results include
effective properties, items, imports, and structured diagnostics.
