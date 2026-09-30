using System.Security.Cryptography;
using System.Text;
using Novolis.Workspaces.DotNet.MSBuild;
using Novolis.Workspaces.DotNet.Roslyn;
using Novolis.Workspaces.DotNet.Slnx;

namespace Novolis.Workspaces.DotNet.Indexing;

/// <summary>A cataloged project together with its evaluated and semantic projections.</summary>
public sealed record SolutionCatalogProject(
    SlnxProjectEntry Project,
    EvaluatedProject Evaluation,
    SemanticProject Semantic);
