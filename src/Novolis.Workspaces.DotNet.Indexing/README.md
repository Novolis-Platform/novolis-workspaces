# Novolis.Workspaces.DotNet.Indexing

Immutable, queryable solution catalog snapshots derived from SLNX, MSBuild, and Roslyn projections,
plus generated typed C# exploration façades.

## Install

```powershell
dotnet add package Novolis.Workspaces.DotNet.Indexing
```

## Quick start

`SolutionCatalogBuilder` produces a deterministic snapshot identifier and portable project,
evaluation, semantic, and diagnostic facts.

`SolutionExplorationGenerator` then emits C# whose members are the solution's named projects,
namespace tree, and types. Compile that source into the consuming project (or compile it
together with a consumer via `InvokeConsumer`) so access is ordinary C#:

```csharp
var solution = new GeneratedSolution(catalog);
SemanticType identityService = solution.Projects.DemoLib.Services.IdentityService;
```
