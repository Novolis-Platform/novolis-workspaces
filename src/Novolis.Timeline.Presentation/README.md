<!-- novolis-pkg-brand:start -->
<p align="center">
  <a href="https://github.com/Novolis-Platform/novolis-workspaces">
    <img src="https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-icon.svg" width="72" alt="Novolis"/>
  </a>
</p>
<!-- novolis-pkg-brand:end -->

# Novolis.Timeline.Presentation

Projects storage timeline nodes into **UI tree models** and optional **git-log-style graph rows** for virtualized lists and Avalonia/WPF hosts.

## Install

```bash
dotnet add package Novolis.Timeline.Presentation
```

Depends on `Novolis.Timeline.Abstractions` and `Novolis.Snapshots.Abstractions`.

## Quick start

```csharp
using Novolis.Snapshots;
using Novolis.Timeline;
using Novolis.Timeline.Presentation;

var projector = new TimelineTreeProjector<ZipSnapshotRef>();
var tree = projector.ToTree(nodes, branches, head);
var rows = projector.ToRows(nodes, branches, head, currentBranchId: BranchId.Main);
```

Git-graph style rows (MeshBench timeline panel):

```csharp
using Novolis.Timeline.Presentation.GitGraph;

var graphRows = GitGraphTimelineBuilder.Build(tree);
```

## API

| Type | Role |
|------|------|
| `ITimelineProjector<TSnapshotRef>` | `ToTree` / `ToRows` |
| `TimelineTreeProjector<TSnapshotRef>` | Default projector |
| `TimelineTreeView` / `TimelineTreeNode` | Hierarchical view |
| `TimelineTreeRow` | Flat row for virtualization |
| `TimelinePresentationMetadata` | UI-safe label/kind/properties |
| `GitGraphTimelineBuilder` | ASCII graph prefixes from tree or flat nodes |
| `GitGraphTimelineRow` | Graph column + subject + branch colors |
| `GitGraphPalette` | Stable branch/kind colors |
| `GraphRgb` | RGB triple for UI brushes |

## Related

| Package | Role |
|---------|------|
| `Novolis.Timeline.Abstractions` | Source node/branch/head models |
| `Novolis.Timeline.FileSystem` | Typical persistence backend |
| `Novolis.Workspaces.Timeline` | Produces nodes for projection |
| `Novolis.Snapshots.Abstractions` | `ZipSnapshotRef` on timeline nodes |

## Notes

Used by **MeshBench** sample for save-point UI. See [getting-started.md](../../docs/getting-started.md).

