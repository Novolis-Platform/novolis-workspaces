using System.Security.Cryptography;
using System.Text;
using Novolis.Workspaces.DotNet.MSBuild;
using Novolis.Workspaces.DotNet.Roslyn;
using Novolis.Workspaces.DotNet.Slnx;

namespace Novolis.Workspaces.DotNet.Indexing;

/// <summary>Immutable, portable facts derived from one solution workspace snapshot.</summary>
public sealed record SolutionCatalog(
    string SnapshotId,
    SolutionCatalogProvenance Provenance,
    EvaluationContext Context,
    IReadOnlyList<SolutionCatalogProject> Projects,
    IReadOnlyList<WorkspaceDiagnostic> Diagnostics);
