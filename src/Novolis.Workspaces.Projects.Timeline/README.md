# Novolis.Workspaces.Projects.Timeline

**Project-scoped** save and restore points: zips one `IProject` subtree (via `IProjectSnapshotPolicy`) and records nodes on a shared workspace timeline.

## Install

```bash
dotnet add package Novolis.Workspaces.Projects.Timeline
```

Depends on **System.IO.Abstractions**, `Novolis.Workspaces.Abstractions`, `Novolis.Workspaces.Timeline`, and related snapshot/timeline packages.

## Quick start

```csharp
using System.IO.Abstractions;
using Novolis.Snapshots;
using Novolis.Timeline.FileSystem;
using Novolis.Workspaces.FileSystem;
using Novolis.Workspaces.Projects.Timeline;

var fs = new FileSystem();
var snapshotsRoot = fs.DirectoryInfo.New(Path.Combine(workspace.Root.FullName, ".novolis", "snapshots"));
var timeline = new FileSystemTimeline<ZipSnapshotRef>(
    fs,
    fs.DirectoryInfo.New(WorkspaceLayout.TimelinePath(workspace.Root.FullName)));

var projectTimeline = new ProjectTimeline(fs, snapshotsRoot, timeline);

await projectTimeline.SavePointAsync(
    project,
    new SavePointRequest("Before mesh edit", SnapshotKinds.Manual));
```

## API

| Type | Role |
|------|------|
| `ProjectTimeline` | `SavePointAsync`, `RestorePointAsync` for one project |
| `IProjectSnapshotPolicy` | `ShouldInclude(project, file)` |
| `DefaultProjectSnapshotPolicy` | Includes documents, assets, settings, manifest; excludes cache/temp/outputs |

Project zips are stored under `projects/{projectId-prefix}/{objectId}.zip` in the snapshots root.

## Related

| Package | Role |
|---------|------|
| `Novolis.Workspaces.Timeline` | Workspace-wide save points |
| `Novolis.Workspaces.Abstractions` | `IProject` |
| `Novolis.Workspaces.Snapshots` | Whole-workspace zip policy patterns |
| `Novolis.Timeline.FileSystem` | Shared timeline persistence |
| `Novolis.Workspaces.FileSystem` | Project folder layout |

## Notes

Use when multiple projects in one workspace need independent rollback without restoring the entire tree. See [design.md](../../docs/design.md).
