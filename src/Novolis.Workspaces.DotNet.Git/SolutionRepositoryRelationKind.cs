using Novolis.IO.Git;

namespace Novolis.Workspaces.DotNet.Git;

/// <summary>Physical relation of a solution workspace to a Git repository workspace.</summary>
public enum SolutionRepositoryRelationKind
{
    SameRoot = 0,
    SolutionNestedInRepository = 1,
    Unrelated = 2,
}
