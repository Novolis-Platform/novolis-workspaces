<!-- novolis-pkg-brand:start -->
[![Novolis](https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-icon.png)](https://novolis-platform.github.io/.github/novolis-workspaces/)

[Novolis](https://github.com/Novolis-Platform) · [Docs](https://novolis-platform.github.io/.github/novolis-workspaces/) · [Source](https://github.com/Novolis-Platform/novolis-workspaces)
<!-- novolis-pkg-brand:end -->

# Novolis.Workspaces.Snapshots

Captures an entire **`IProjectWorkspace`** tree as zip archives using **`IWorkspaceSnapshotPolicy`** include/exclude rules. Timeline data under `.novolis/timeline/` is excluded from capture and preserved across restore.

## Install

```bash
dotnet add package Novolis.Workspaces.Snapshots
```

Depends on **System.IO.Abstractions**, `Novolis.Workspaces.Abstractions`, `Novolis.Workspaces.FileSystem`, and `Novolis.Snapshots.Abstractions`.

## Quick start

```csharp
using System.IO.Abstractions;
using Novolis.Snapshots;
using Novolis.Workspaces.FileSystem;
using Novolis.Workspaces.Snapshots;

var fs = new FileSystem();
var snapshotsRoot = fs.DirectoryInfo.New(
    Path.Combine(workspace.Root.FullName, ".novolis", "snapshots"));

var store = new ZipWorkspaceSnapshotStore(fs, snapshotsRoot);
var snapshot = await store.SaveAsync(
    workspace,
    new SnapshotRequest("Before export", SnapshotKinds.ExportCheckpoint));
```

## API

| Type | Role |
|------|------|
| `ZipWorkspaceSnapshotStore` | `ISnapshotStore<IProjectWorkspace, ZipSnapshotRef>` |
| `IWorkspaceSnapshotPolicy` | `ShouldInclude(workspace, file)` |
| `DefaultWorkspaceSnapshotPolicy` | Default include/exclude for documents, assets, manifests |

Default policy excludes `.novolis/timeline/`, `cache/`, `temp/`, `outputs/`, and similar paths.

## Related

| Package | Role |
|---------|------|
| `Novolis.Workspaces.Abstractions` | `IProjectWorkspace` being captured |
| `Novolis.Workspaces.FileSystem` | Layout paths and open/create |
| `Novolis.Snapshots.Abstractions` | `ZipSnapshotRef`, `SnapshotRequest` |
| `Novolis.Snapshots.Zip` | Single-state zip store (not whole workspace) |
| `Novolis.Workspaces.Timeline` | Orchestrates snapshots + timeline |

## Notes

Restore backs up and restores the timeline folder separately so history survives workspace rollback. See [design.md](../../docs/design.md).

