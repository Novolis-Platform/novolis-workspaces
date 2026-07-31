# Novolis.Workspaces.Timeline

Composes **workspace zip snapshots** with a **branchable timeline**: save points capture the workspace tree and append a timeline node; restore points roll back disk state (with a safety save first).

## Install

```bash
dotnet add package Novolis.Workspaces.Timeline
```

Depends on `Novolis.Workspaces.Abstractions`, `Novolis.Workspaces.Snapshots`, `Novolis.Timeline.Abstractions`, and `Novolis.Snapshots.Abstractions`.

## Quick start

```csharp
using System.IO.Abstractions;
using Novolis.Snapshots;
using Novolis.Timeline.FileSystem;
using Novolis.Workspaces.FileSystem;
using Novolis.Workspaces.Snapshots;
using Novolis.Workspaces.Timeline;

var fs = new FileSystem();
var snapshotsRoot = fs.DirectoryInfo.New(Path.Combine(workspace.Root.FullName, ".novolis", "snapshots"));
var timelineRoot = fs.DirectoryInfo.New(WorkspaceLayout.TimelinePath(workspace.Root.FullName));

var store = new ZipWorkspaceSnapshotStore(fs, snapshotsRoot);
var timeline = new FileSystemTimeline<ZipSnapshotRef>(fs, timelineRoot);
var workspaceTimeline = new WorkspaceTimeline(timeline, store);

await workspaceTimeline.SavePointAsync(
    workspace,
    new SavePointRequest("Before export", SnapshotKinds.ExportCheckpoint));
```

## API

| Type | Role |
|------|------|
| `WorkspaceTimeline` | `SavePointAsync`, `RestorePointAsync`, `BranchFromAsync` |
| `SavePointRequest` | Label/kind/properties; maps to snapshot + timeline metadata |

`RestorePointAsync` creates a safety save point, restores the workspace from the node's snapshot, and optionally moves the branch head.

## Related

| Package | Role |
|---------|------|
| `Novolis.Workspaces.Snapshots` | `ZipWorkspaceSnapshotStore` |
| `Novolis.Timeline.FileSystem` | Durable timeline graph |
| `Novolis.Timeline.Presentation` | UI projection of nodes |
| `Novolis.Workspaces.Projects.Timeline` | Project-scoped variant |
| `Novolis.Workspaces.FileSystem` | Workspace layout paths |

## Notes

End-to-end walkthrough: [getting-started.md](../../docs/getting-started.md).
