# novolis-workspaces

Reusable .NET libraries for editor and studio apps:

- **Novolis.Workspaces** — workspace / project / document layout on disk
- **Novolis.Snapshots** — save-game style state capture and restore
- **Novolis.Timeline** — branchable history over snapshot references

Adapters compose the three (`Novolis.Workspaces.Snapshots`, `Novolis.Workspaces.Timeline`, `Novolis.Workspaces.Projects.Timeline`).

## Install

`Novolis.*` packages publish to [GitHub Packages](https://github.com/orgs/Novolis-Platform/packages) (`2026.1.*`). Third-party dependencies resolve from [nuget.org](https://www.nuget.org).

```bash
dotnet add package Novolis.Workspaces.Timeline
dotnet add package Novolis.Timeline.Presentation
```

Ensure your `nuget.config` includes the Novolis-Platform GitHub feed and credentials for GPR restore.

## Packages

| Package | Role | README |
|---------|------|--------|
| `Novolis.Workspaces.Abstractions` | `IWorkspace`, `IProject`, manifests | [README](src/Novolis.Workspaces.Abstractions/README.md) |
| `Novolis.Workspaces.FileSystem` | On-disk layout, open/create | [README](src/Novolis.Workspaces.FileSystem/README.md) |
| `Novolis.Snapshots.Abstractions` | `ISnapshotStore`, refs | [README](src/Novolis.Snapshots.Abstractions/README.md) |
| `Novolis.Snapshots.Memory` | In-process snapshot store | [README](src/Novolis.Snapshots.Memory/README.md) |
| `Novolis.Snapshots.Json` | JSON state serializer | [README](src/Novolis.Snapshots.Json/README.md) |
| `Novolis.Snapshots.FileSystem` | Blob snapshot backend | [README](src/Novolis.Snapshots.FileSystem/README.md) |
| `Novolis.Snapshots.Zip` | Zip snapshot backend | [README](src/Novolis.Snapshots.Zip/README.md) |
| `Novolis.Timeline.Abstractions` | `ITimeline`, branches | [README](src/Novolis.Timeline.Abstractions/README.md) |
| `Novolis.Timeline.Memory` | In-memory timeline | [README](src/Novolis.Timeline.Memory/README.md) |
| `Novolis.Timeline.FileSystem` | JSON timeline persistence | [README](src/Novolis.Timeline.FileSystem/README.md) |
| `Novolis.Timeline.Presentation` | Tree/git-graph UI projection | [README](src/Novolis.Timeline.Presentation/README.md) |
| `Novolis.Workspaces.Snapshots` | Zip workspace snapshots + policy | [README](src/Novolis.Workspaces.Snapshots/README.md) |
| `Novolis.Workspaces.Timeline` | Save points / restore points | [README](src/Novolis.Workspaces.Timeline/README.md) |
| `Novolis.Workspaces.Projects.Timeline` | Project-scoped timeline | [README](src/Novolis.Workspaces.Projects.Timeline/README.md) |

Typical stack for a studio app: `Workspaces.FileSystem` + `Workspaces.Timeline` + `Timeline.Presentation`.

## Docs

- [design.md](docs/design.md) — boundaries and on-disk layout
- [getting-started.md](docs/getting-started.md) — quick start

## Dogfood samples

Console timeline walkthrough:

```powershell
dotnet run --project d:\novolis\novolis-dogfooding\apps\workspaces\MinimalWorkspaceTimeline -p:NovolisUseProjectReferences=true
```

**MeshBench** (Avalonia mini CAD — boxes/spheres, path-traced viewport, workspace save points + timeline):

```powershell
dotnet run --project d:\novolis\novolis-dogfooding\apps\rendering\MeshBench -p:NovolisUseProjectReferences=true
```

Workspace files default to `%LocalAppData%/Novolis/MeshBench/default-workspace`.

## Boundaries

Not version control: no merge, rebase, remotes, or conflict resolution. Distinct from `Novolis.IO.Workspace` (storage file root) and `Novolis.Simulation.Replay` (tick replay).
