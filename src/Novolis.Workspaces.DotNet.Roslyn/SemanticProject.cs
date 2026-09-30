using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Novolis.Workspaces.DotNet.MSBuild;

namespace Novolis.Workspaces.DotNet.Roslyn;

/// <summary>A portable Roslyn projection for one evaluated project.</summary>
public sealed record SemanticProject(
    string ProjectPath,
    IReadOnlyList<string> Documents,
    IReadOnlyList<SemanticType> Types,
    IReadOnlyList<WorkspaceDiagnostic> Diagnostics);
