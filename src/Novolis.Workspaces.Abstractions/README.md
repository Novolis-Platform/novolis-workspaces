<!-- novolis-pkg-brand:start -->
<p align="center">
  <a href="https://github.com/Novolis-Platform/novolis-workspaces">
    <img src="https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-icon.svg" width="72" alt="Novolis"/>
  </a>
</p>
<!-- novolis-pkg-brand:end -->

# Novolis.Workspaces.Abstractions

Core contracts for **editor/studio workspace containers**: a workspace holds projects, manifests, and well-known folder semantics. Implementations live in `Novolis.Workspaces.FileSystem`.

## Install

```bash
dotnet add package Novolis.Workspaces.Abstractions
```

Depends on **System.IO.Abstractions** (`IDirectoryInfo` on workspace/project roots).

## Quick start

```csharp
using Novolis.Workspaces;

IWorkspace workspace = /* PhysicalWorkspace from FileSystem package */;

foreach (var project in workspace.Projects)
{
    Console.WriteLine($"{project.Name} ({project.Kind}) @ {project.Root.FullName}");
}
```

## API

| Type | Role |
|------|------|
| `IWorkspace` | Id, name, root, manifest, projects |
| `IProject` | Id, name, kind, root, manifest |
| `WorkspaceManifest` | `workspace.json` shape |
| `ProjectManifest` | `project.json` shape |
| `ProjectReference` | Entry in workspace manifest |
| `WorkspaceId` / `ProjectId` | Strongly typed ids |
| `ProjectKind` | `Generic`, `VoicePack`, `Scenario`, `GameSave` |
| `WorkspaceException` | Workspace operation failed |

## Related

| Package | Role |
|---------|------|
| `Novolis.Workspaces.FileSystem` | Create/open workspaces on disk |
| `Novolis.Workspaces.Snapshots` | Zip snapshots of `IWorkspace` |
| `Novolis.Workspaces.Timeline` | Save/restore points |
| `Novolis.Workspaces.Projects.Timeline` | Project-scoped timeline |
| `Novolis.Snapshots.Abstractions` | Snapshot primitives composed by adapters |

## Notes

On-disk layout: [design.md](../../docs/design.md). Distinct from `Novolis.IO.Workspace` (storage file root).

