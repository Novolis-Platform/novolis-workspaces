<!-- novolis-marketing:start -->
<p align="center">
  <a href="https://github.com/Novolis-Platform">
    <img src="https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-brand-transparent.svg" width="360" alt="Novolis"/>
  </a>
</p>

<p align="center">
  <img src="https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/banners/novolis-workspaces.svg" width="100%" alt="novolis-workspaces"/>
</p>

<p align="center">
  <strong>Snapshots and timelines</strong><br/>
  Workspace, snapshot, and timeline libraries for editor and studio apps.
</p>

<p align="center">
  <a href="https://github.com/Novolis-Platform/novolis-workspaces/actions"><img src="https://img.shields.io/github/actions/workflow/status/Novolis-Platform/novolis-workspaces/merge.yml?branch=main&label=merge&logo=github" alt="merge"/></a>
  <a href="https://github.com/orgs/Novolis-Platform/packages?repo_name=novolis-workspaces"><img src="https://img.shields.io/badge/packages-GitHub%20Packages-0a7ea3?logo=nuget" alt="packages"/></a>
  <a href="https://github.com/Novolis-Platform"><img src="https://img.shields.io/badge/org-Novolis--Platform-111827" alt="org"/></a>
</p>

<p align="center">
  <a href="https://nuget.pkg.github.com/Novolis-Platform/index.json"><code>https://nuget.pkg.github.com/Novolis-Platform/index.json</code></a>
  ·
  <a href="https://github.com/Novolis-Platform/.github/blob/main/profile/README.md">Org landing</a>
  ·
  <a href="https://github.com/Novolis-Platform/novolis-governance">Governance</a>
</p>

---
<!-- novolis-marketing:end -->
<!-- novolis-package-index:start -->
> **GitHub Packages shows this repository README on every package page** (upstream limitation).
> Open the **package README** for install and quick start — embedded in each .nupkg and linked below.

## Published packages

| Package | Install | Package README |
|---------|---------|----------------|
| `Novolis.Snapshots.Abstractions` | `dotnet add package Novolis.Snapshots.Abstractions` | [README](https://github.com/Novolis-Platform/novolis-workspaces/blob/main/src/Novolis.Snapshots.Abstractions/README.md) |
| `Novolis.Snapshots.FileSystem` | `dotnet add package Novolis.Snapshots.FileSystem` | [README](https://github.com/Novolis-Platform/novolis-workspaces/blob/main/src/Novolis.Snapshots.FileSystem/README.md) |
| `Novolis.Snapshots.Json` | `dotnet add package Novolis.Snapshots.Json` | [README](https://github.com/Novolis-Platform/novolis-workspaces/blob/main/src/Novolis.Snapshots.Json/README.md) |
| `Novolis.Snapshots.Memory` | `dotnet add package Novolis.Snapshots.Memory` | [README](https://github.com/Novolis-Platform/novolis-workspaces/blob/main/src/Novolis.Snapshots.Memory/README.md) |
| `Novolis.Snapshots.Zip` | `dotnet add package Novolis.Snapshots.Zip` | [README](https://github.com/Novolis-Platform/novolis-workspaces/blob/main/src/Novolis.Snapshots.Zip/README.md) |
| `Novolis.Timeline.Abstractions` | `dotnet add package Novolis.Timeline.Abstractions` | [README](https://github.com/Novolis-Platform/novolis-workspaces/blob/main/src/Novolis.Timeline.Abstractions/README.md) |
| `Novolis.Timeline.FileSystem` | `dotnet add package Novolis.Timeline.FileSystem` | [README](https://github.com/Novolis-Platform/novolis-workspaces/blob/main/src/Novolis.Timeline.FileSystem/README.md) |
| `Novolis.Timeline.Memory` | `dotnet add package Novolis.Timeline.Memory` | [README](https://github.com/Novolis-Platform/novolis-workspaces/blob/main/src/Novolis.Timeline.Memory/README.md) |
| `Novolis.Timeline.Presentation` | `dotnet add package Novolis.Timeline.Presentation` | [README](https://github.com/Novolis-Platform/novolis-workspaces/blob/main/src/Novolis.Timeline.Presentation/README.md) |
| `Novolis.Workspaces.Abstractions` | `dotnet add package Novolis.Workspaces.Abstractions` | [README](https://github.com/Novolis-Platform/novolis-workspaces/blob/main/src/Novolis.Workspaces.Abstractions/README.md) |
| `Novolis.Workspaces.FileSystem` | `dotnet add package Novolis.Workspaces.FileSystem` | [README](https://github.com/Novolis-Platform/novolis-workspaces/blob/main/src/Novolis.Workspaces.FileSystem/README.md) |
| `Novolis.Workspaces.Projects.Timeline` | `dotnet add package Novolis.Workspaces.Projects.Timeline` | [README](https://github.com/Novolis-Platform/novolis-workspaces/blob/main/src/Novolis.Workspaces.Projects.Timeline/README.md) |
| `Novolis.Workspaces.Snapshots` | `dotnet add package Novolis.Workspaces.Snapshots` | [README](https://github.com/Novolis-Platform/novolis-workspaces/blob/main/src/Novolis.Workspaces.Snapshots/README.md) |
| `Novolis.Workspaces.Timeline` | `dotnet add package Novolis.Workspaces.Timeline` | [README](https://github.com/Novolis-Platform/novolis-workspaces/blob/main/src/Novolis.Workspaces.Timeline/README.md) |

For NuGet.org and Visual Studio, the **embedded** README.md inside each package is authoritative.

<!-- novolis-package-index:end -->
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

