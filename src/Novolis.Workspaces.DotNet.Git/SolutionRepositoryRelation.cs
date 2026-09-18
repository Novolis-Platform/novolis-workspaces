using Novolis.IO.Git;

namespace Novolis.Workspaces.DotNet.Git;

/// <summary>Physical relation of a solution workspace to a Git repository workspace.</summary>
public enum SolutionRepositoryRelationKind
{
    SameRoot = 0,
    SolutionNestedInRepository = 1,
    Unrelated = 2,
}

/// <summary>Explicitly relates one typed solution workspace to one typed Git repository workspace.</summary>
public sealed record SolutionRepositoryRelation(
    SolutionWorkspace Solution,
    GitRepositoryWorkspace Repository,
    SolutionRepositoryRelationKind Kind)
{
    public static SolutionRepositoryRelation Create(
        SolutionWorkspace solution,
        GitRepositoryWorkspace repository)
    {
        ArgumentNullException.ThrowIfNull(solution);
        ArgumentNullException.ThrowIfNull(repository);

        var solutionRoot = Path.GetFullPath(solution.Root.FullName);
        var repositoryRoot = Path.GetFullPath(repository.Root.FullName);
        var kind = string.Equals(solutionRoot, repositoryRoot, StringComparison.OrdinalIgnoreCase)
            ? SolutionRepositoryRelationKind.SameRoot
            : solutionRoot.StartsWith(
                repositoryRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar,
                StringComparison.OrdinalIgnoreCase)
                ? SolutionRepositoryRelationKind.SolutionNestedInRepository
                : SolutionRepositoryRelationKind.Unrelated;
        return new SolutionRepositoryRelation(solution, repository, kind);
    }
}
