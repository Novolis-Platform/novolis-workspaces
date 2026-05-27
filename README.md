# novolis-workspaces

Reusable .NET libraries for editor and studio apps:

- **Novolis.Workspaces** — workspace / project / document layout on disk
- **Novolis.Snapshots** — save-game style state capture and restore
- **Novolis.Timeline** — branchable history over snapshot references

Adapters compose the three (`Novolis.Workspaces.Snapshots`, `Novolis.Workspaces.Timeline`, `Novolis.Workspaces.Projects.Timeline`).

## Packages

| Package | Role |
|---------|------|
| `Novolis.Workspaces.Abstractions` | `IWorkspace`, `IProject`, manifests |
| `Novolis.Workspaces.FileSystem` | On-disk layout, open/create |
| `Novolis.Snapshots.Abstractions` | `ISnapshotStore`, refs |
| `Novolis.Snapshots.Memory` / `.Json` / `.FileSystem` / `.Zip` | Snapshot backends |
| `Novolis.Timeline.Abstractions` | `ITimeline`, branches |
| `Novolis.Timeline.Memory` / `.FileSystem` / `.Presentation` | Timeline storage and UI projection |
| `Novolis.Workspaces.Snapshots` | Zip workspace snapshots + policy |
| `Novolis.Workspaces.Timeline` | Save points / restore points |
| `Novolis.Workspaces.Projects.Timeline` | Project-scoped timeline |

## Docs

- [design.md](docs/design.md) — boundaries and on-disk layout
- [getting-started.md](docs/getting-started.md) — quick start

## Sample

```bash
dotnet run --project samples/MinimalWorkspaceTimeline
```

## Boundaries

Not version control: no merge, rebase, remotes, or conflict resolution. Distinct from `Novolis.IO.Workspace` (storage file root) and `Novolis.Simulation.Replay` (tick replay).
