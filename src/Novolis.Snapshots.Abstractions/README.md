# Novolis.Snapshots.Abstractions

Core contracts for **save-game style state capture**: serialize application state, persist opaque snapshots, and restore into a live instance. Backends (`Memory`, `FileSystem`, `Zip`) and serializers (`Json`) live in sibling packages.

## Install

```bash
dotnet add package Novolis.Snapshots.Abstractions
```

## Quick start

```csharp
using Novolis.Snapshots;

ISnapshotStore<MyState, MemorySnapshotRef> store = /* Memory, FileSystem, or Zip backend */;

var snapshot = await store.SaveAsync(
    state,
    new SnapshotRequest("Before export", SnapshotKinds.Manual));

await store.RestoreAsync(state, snapshot);
```

Implement `IStateSerializer<TState>` (or use `Novolis.Snapshots.Json`) to plug in your state type.

## API

| Type | Role |
|------|------|
| `ISnapshotStore<TState, TSnapshotRef>` | `SaveAsync` / `RestoreAsync` |
| `IStateSerializer<TState>` | Write/read state to a `Stream` |
| `SnapshotRequest` | Label, kind, optional properties |
| `SnapshotKinds` | `Manual`, `Autosave`, `Safety`, `Quick`, `ExportCheckpoint` |
| `MemorySnapshotRef` | In-process snapshot id |
| `FileSnapshotRef` | Relative path + content hash |
| `ZipSnapshotRef` | Object id + zip relative path |
| `SnapshotException` | Store operation failed |

## Related

| Package | Role |
|---------|------|
| `Novolis.Snapshots.Memory` | In-process dictionary backend |
| `Novolis.Snapshots.Json` | JSON `IStateSerializer<TState>` |
| `Novolis.Snapshots.FileSystem` | Blob files under a root directory |
| `Novolis.Snapshots.Zip` | Zip archives with manifest entry |
| `Novolis.Timeline.Abstractions` | Branchable graph over snapshot refs |
| `Novolis.Workspaces.Snapshots` | Whole-workspace zip snapshots |

## Notes

On-disk layout and boundaries: [design.md](../../docs/design.md).
