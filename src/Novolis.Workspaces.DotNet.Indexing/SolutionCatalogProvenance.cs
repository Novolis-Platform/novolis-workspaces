using System.Security.Cryptography;
using System.Text;
using Novolis.Workspaces.DotNet.MSBuild;
using Novolis.Workspaces.DotNet.Roslyn;
using Novolis.Workspaces.DotNet.Slnx;

namespace Novolis.Workspaces.DotNet.Indexing;

/// <summary>Portable provenance for a catalog snapshot.</summary>
public sealed record SolutionCatalogProvenance(
    string WorkspaceRootPath,
    string SolutionPath,
    DateTimeOffset CreatedAt);
