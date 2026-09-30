using System.Text;
using Novolis.Workspaces.DotNet.Roslyn;
using Novolis.Workspaces.DotNet.Slnx;

namespace Novolis.Workspaces.DotNet.Indexing;

/// <summary>Options for generating a typed C# exploration façade from a catalog.</summary>
public sealed record SolutionExplorationGenerationOptions(bool PublicTypesOnly = true);
