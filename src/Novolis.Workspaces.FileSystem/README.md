# Novolis.Workspaces.FileSystem

Creates and opens **disk-backed workspaces** with the standard `.novolis/` layout, project folders, and JSON manifests.

## Install

```bash
dotnet add package Novolis.Workspaces.FileSystem
```

Depends on **System.IO.Abstractions** and `Novolis.Workspaces.Abstractions`.

## Quick start

```csharp
using System.IO.Abstractions;
using Novolis.Workspaces;
using Novolis.Workspaces.FileSystem;

var fs = new FileSystem();
var service = new WorkspaceFileSystemService(fs);

var workspace = await service.CreateAsync(@"C:\work\my-studio", "My Studio");
var project = await service.AddProjectAsync(workspace, "Voice Pack", ProjectKind.VoicePack);

var reopened = await service.OpenAsync(workspace.Root.FullName);
```

## API

| Type | Role |
|------|------|
| `WorkspaceFileSystemService` | `CreateAsync`, `OpenAsync`, `AddProjectAsync` |
| `PhysicalWorkspace` | `IWorkspace` implementation |
| `PhysicalProject` | `IProject` implementation |
| `WorkspaceLayout` | Path constants and helpers (`TimelinePath`, `ProjectsPath`, etc.) |
| `WorkspaceLayout.CurrentSchemaVersion` | Manifest schema version (`1`) |

## Related

| Package | Role |
|---------|------|
| `Novolis.Workspaces.Abstractions` | `IWorkspace`, manifests, ids |
| `Novolis.Workspaces.Snapshots` | Zip capture of workspace tree |
| `Novolis.Workspaces.Timeline` | Timeline under `.novolis/timeline` |
| `Novolis.Timeline.FileSystem` | Persists timeline JSON |
| `Novolis.Snapshots.Json` | JSON patterns used by manifests |

## Notes

Folder layout and include rules: [design.md](../../docs/design.md).
