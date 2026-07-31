# Novolis.Timeline.FileSystem

Disk-backed **`ITimeline<TSnapshotRef>`** persisting `branches.json`, `head.json`, and per-node files under `nodes/{nodeId}.json`.

## Install

```bash
dotnet add package Novolis.Timeline.FileSystem
```

Depends on **System.IO.Abstractions** and `Novolis.Timeline.Abstractions`.

## Quick start

```csharp
using System.IO.Abstractions;
using Novolis.Snapshots;
using Novolis.Timeline.FileSystem;

var fs = new FileSystem();
var root = fs.DirectoryInfo.New(@"C:\work\my-studio\.novolis\timeline");
var timeline = new FileSystemTimeline<ZipSnapshotRef>(fs, root);

var node = await timeline.AddAsync(
    snapshotRef,
    new TimelineMetadata("Autosave", TimelineKinds.Autosave, []));
```

Use `TimelineJsonSerializerOptions.Create()` for consistent JSON options (camelCase, `BranchId` / `TimelineNodeId` converters).

## API

| Type | Role |
|------|------|
| `FileSystemTimeline<TSnapshotRef>` | `ITimeline<TSnapshotRef>` with file layout |
| `TimelineJsonSerializerOptions` | Shared `JsonSerializerOptions` for timeline JSON |

## Related

| Package | Role |
|---------|------|
| `Novolis.Timeline.Abstractions` | Timeline contracts |
| `Novolis.Timeline.Memory` | Non-durable alternative |
| `Novolis.Snapshots.Json` | JSON serialization patterns |
| `Novolis.Workspaces.FileSystem` | `WorkspaceLayout.TimelinePath` |
| `Novolis.Workspaces.Timeline` | Composes timeline + workspace snapshots |

## Notes

On-disk paths: [design.md](../../docs/design.md).
