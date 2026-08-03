<!-- novolis-pkg-brand:start -->
<p align="center">
  <a href="https://github.com/Novolis-Platform/novolis-workspaces">
    <img src="https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-icon.svg" width="72" alt="Novolis"/>
  </a>
</p>
<!-- novolis-pkg-brand:end -->

# Novolis.Timeline.Memory

Process-local **`ITimeline<TSnapshotRef>`** backed by concurrent dictionaries. Creates a `main` branch on construction. Single-writer usage is recommended.

## Install

```bash
dotnet add package Novolis.Timeline.Memory
```

Depends on `Novolis.Timeline.Abstractions`.

## Quick start

```csharp
using Novolis.Snapshots;
using Novolis.Timeline;
using Novolis.Timeline.Memory;

var timeline = new InMemoryTimeline<ZipSnapshotRef>();

var node = await timeline.AddAsync(
    new ZipSnapshotRef("abc", "ab/abc.zip"),
    new TimelineMetadata("Checkpoint", TimelineKinds.SavePoint, []));

var nodes = await timeline.GetNodesAsync();
var head = await timeline.GetHeadAsync();
```

## API

| Type | Role |
|------|------|
| `InMemoryTimeline<TSnapshotRef>` | Full `ITimeline<TSnapshotRef>` implementation |

## Related

| Package | Role |
|---------|------|
| `Novolis.Timeline.Abstractions` | `ITimeline`, node/branch models |
| `Novolis.Timeline.FileSystem` | Durable JSON persistence |
| `Novolis.Snapshots.Memory` | Matching in-process snapshot store |
| `Novolis.Timeline.Presentation` | UI tree projection |
| `Novolis.Workspaces.Timeline` | Workspace-level save points |

