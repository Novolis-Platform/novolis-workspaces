using System.IO.Abstractions;
using Novolis.Snapshots;
using Novolis.Workspaces.FileSystem;
using Novolis.Workspaces.Snapshots;
using TUnit.Core;

namespace Novolis.Workspaces.Unit.Workspaces;

public sealed class ZipWorkspaceSnapshotStoreTests
{
    [Test]
    public async Task Restore_PreservesTimelineFolder()
    {
        var fs = new System.IO.Abstractions.FileSystem();
        var service = new WorkspaceFileSystemService(fs);
        var root = fs.Path.Combine(fs.Path.GetTempPath(), "novolis-ws-" + Guid.NewGuid().ToString("N"));
        var workspace = await service.CreateAsync(root, "Snapshot Test");
        var project = await service.AddProjectAsync(workspace, "Demo", ProjectKind.Generic);

        var docPath = fs.Path.Combine(project.Root.FullName, WorkspaceLayout.DocumentsFolder, "notes.txt");
        fs.File.WriteAllText(docPath, "version-1");

        var timelineMarker = fs.Path.Combine(WorkspaceLayout.TimelinePath(root), "marker.txt");
        fs.File.WriteAllText(timelineMarker, "keep-me");

        var snapshotsRoot = fs.DirectoryInfo.New(fs.Path.Combine(root, ".novolis", "snapshots"));
        var store = new ZipWorkspaceSnapshotStore(fs, snapshotsRoot);
        var snapshot = await store.SaveAsync(workspace, new SnapshotRequest("v1", SnapshotKinds.Manual));

        fs.File.WriteAllText(docPath, "version-2");
        await store.RestoreAsync(workspace, snapshot);

        await Assert.That(fs.File.ReadAllText(docPath)).IsEqualTo("version-1");
        await Assert.That(fs.File.Exists(timelineMarker)).IsTrue();
        await Assert.That(fs.File.ReadAllText(timelineMarker)).IsEqualTo("keep-me");
    }
}
