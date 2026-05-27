using System.IO.Abstractions;
using Novolis.Workspaces;
using Novolis.Workspaces.FileSystem;
using TUnit.Core;

namespace Novolis.Workspaces.Unit.Workspaces;

public sealed class WorkspaceFileSystemServiceTests
{
    [Test]
    public async Task CreateAndAddProject_WritesManifests()
    {
        var fs = new System.IO.Abstractions.FileSystem();
        var service = new WorkspaceFileSystemService(fs);
        var root = fs.Path.Combine(fs.Path.GetTempPath(), "novolis-ws-" + Guid.NewGuid().ToString("N"));

        var workspace = await service.CreateAsync(root, "Test Workspace");
        var project = await service.AddProjectAsync(workspace, "Pack A", ProjectKind.VoicePack);

        await Assert.That(fs.File.Exists(WorkspaceLayout.WorkspaceManifestPath(root))).IsTrue();
        await Assert.That(fs.File.Exists(fs.Path.Combine(project.Root.FullName, WorkspaceLayout.ProjectManifestFile))).IsTrue();
    }
}
