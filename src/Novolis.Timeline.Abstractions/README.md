<!-- novolis-pkg-brand:start -->
<p align="center">
  <a href="https://github.com/Novolis-Platform/novolis-workspaces">
    <img src="https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-icon.svg" width="72" alt="Novolis"/>
  </a>
</p>
<!-- novolis-pkg-brand:end -->

# Novolis.Timeline.Abstractions

Contracts for a **branchable history graph** over snapshot references. Not version control: no merge, rebase, remotes, or conflict resolution.

## Install

```bash
dotnet add package Novolis.Timeline.Abstractions
```

## Quick start

```csharp
using Novolis.Snapshots;
using Novolis.Timeline;

ITimeline<ZipSnapshotRef> timeline = /* InMemoryTimeline or FileSystemTimeline */;

var node = await timeline.AddAsync(
    snapshotRef,
    new TimelineMetadata("Before export", TimelineKinds.SavePoint, new Dictionary<string, string>()));

var branch = await timeline.BranchAsync(new BranchName("experiment"), node.Id);
await timeline.MoveHeadAsync(branch.Id, node.Id);
```

## API

| Type | Role |
|------|------|
| `ITimeline<TSnapshotRef>` | `AddAsync`, `BranchAsync`, `MoveHeadAsync`, queries |
| `TimelineNode<TSnapshotRef>` | Id, parent, branch, snapshot, metadata, timestamp |
| `Branch` / `BranchName` | Named alternate path |
| `BranchId` | Branch identifier (`BranchId.Main` for default) |
| `TimelineNodeId` | Node identifier |
| `TimelineHead` | Current head node per branch |
| `TimelineMetadata` | Label, kind, properties |
| `TimelineKinds` | `SavePoint`, `RestorePoint`, `Safety`, `Autosave` |
| `TimelineException` | Timeline operation failed |

## Related

| Package | Role |
|---------|------|
| `Novolis.Snapshots.Abstractions` | Snapshot refs stored on nodes |
| `Novolis.Timeline.Memory` | Process-local graph |
| `Novolis.Timeline.FileSystem` | JSON persistence under a root |
| `Novolis.Timeline.Presentation` | Tree/flat UI projection |
| `Novolis.Workspaces.Timeline` | Workspace save/restore orchestration |

## Notes

Boundaries vs git and replay: [design.md](../../docs/design.md).

