# Novolis.Workspaces.DotNet.Indexing

Immutable, queryable solution catalog snapshots derived from SLNX, MSBuild, and Roslyn projections,
plus dynamic generation of a typed C# exploration façade.

## Install

```powershell
dotnet add package Novolis.Workspaces.DotNet.Indexing
```

## Quick start

`SolutionCatalogBuilder` produces a deterministic snapshot identifier and portable project,
evaluation, semantic, and diagnostic facts.

`SolutionExplorationGenerator` then emits C# whose members are the solution's named projects,
namespace tree, and types:

```csharp
var generated = SolutionExplorationGenerator.Generate(catalog);
var compiled = SolutionExplorationCompiler.Compile(generated);
var walk = compiled.Walk(catalog);
// solution.Projects.DemoLib.Novolis.Sample.Widget => Novolis.Sample.Widget
```
