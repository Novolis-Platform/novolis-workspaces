# novolis-workspaces design

## Three libraries, one repo

| Library | Owns | Must not know |
|---------|------|----------------|
| **Novolis.Snapshots** | Serialize state, snapshot refs | Timeline, workspace manifests |
| **Novolis.Timeline** | Graph over snapshot refs | Files, zip, workspace layout |
| **Novolis.Workspaces** | Workspace/project structure | Branching graph internals |

Adapters (`Workspaces.Snapshots`, `Workspaces.Timeline`, `Workspaces.Projects.Timeline`) compose all three.

## On-disk workspace layout

```text
my-workspace/
  .novolis/
    workspace.json
    settings.json
    timeline/          # branch/head/nodes (Timeline.FileSystem)
    snapshots/         # zip blobs (Workspaces.Snapshots)
  projects/
    {folder}/
      project.json
      documents/
      assets/
      outputs/
      cache/
      temp/
```

## Restore rules

1. Create a **safety** save point before restore.
2. Restore working files from the selected zip snapshot.
3. **Preserve** `.novolis/timeline/` (history is not rolled back with working tree).

## Naming (user-facing)

Use: Save Point, Restore Point, Timeline, Branch, Current.

Avoid: Commit, Checkout, Repository, Merge, Rebase.

## Platform boundaries

| Existing | Relationship |
|----------|----------------|
| `Novolis.IO.Workspace` | Low-level file root for Storage.Json — optional future bridge |
| `ISnapshotCapableEventStore` | Stream compaction — unrelated |
| `SimulationTimeline<TState>` | Tick replay — unrelated |

## Filesystem

All disk facets use **System.IO.Abstractions** (`IFileSystem`) for testability.
