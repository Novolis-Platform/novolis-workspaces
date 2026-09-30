using System.Text;
using Novolis.Workspaces.DotNet.Roslyn;
using Novolis.Workspaces.DotNet.Slnx;

namespace Novolis.Workspaces.DotNet.Indexing;

/// <summary>Generated C# that exposes named projects, namespaces, and types as real members.</summary>
public sealed record GeneratedSolutionExploration(
    string Source,
    string Namespace,
    string SolutionTypeName,
    string WalkTypeName);
