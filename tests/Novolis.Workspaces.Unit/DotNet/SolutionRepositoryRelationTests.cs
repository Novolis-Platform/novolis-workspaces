using Novolis.IO.Git;
using Novolis.Workspaces.DotNet;
using Novolis.Workspaces.DotNet.Git;
using TUnit.Core;

namespace Novolis.Workspaces.Unit.DotNet;

public sealed class SolutionRepositoryRelationTests
{
    [Test]
    public async Task Create_ClassifiesSolutionNestedInRepository()
    {
        var repositoryRoot = Path.Combine(Path.GetTempPath(), "novolis-repo-" + Guid.NewGuid().ToString("N"));
        var solutionRoot = Path.Combine(repositoryRoot, "src");
        Directory.CreateDirectory(solutionRoot);
        try
        {
            var solutionPath = Path.Combine(solutionRoot, "Demo.slnx");
            await File.WriteAllTextAsync(solutionPath, "<Solution />");
            var fileSystem = new System.IO.Abstractions.FileSystem();
            var solution = new SolutionWorkspace(
                fileSystem.DirectoryInfo.New(solutionRoot),
                new SolutionFilePath(solutionPath));
            var repository = new GitRepositoryWorkspace(
                fileSystem.DirectoryInfo.New(repositoryRoot),
                "demo",
                GitWorktreeKind.Main);

            var relation = SolutionRepositoryRelation.Create(solution, repository);

            await Assert.That(relation.Kind).IsEqualTo(SolutionRepositoryRelationKind.SolutionNestedInRepository);
        }
        finally
        {
            Directory.Delete(repositoryRoot, recursive: true);
        }
    }
}
