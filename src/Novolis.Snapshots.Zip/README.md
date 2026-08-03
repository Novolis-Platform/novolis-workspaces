<!-- novolis-pkg-brand:start -->
<p align="center">
  <a href="https://github.com/Novolis-Platform/novolis-workspaces">
    <img src="https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-icon.svg" width="72" alt="Novolis"/>
  </a>
</p>
<!-- novolis-pkg-brand:end -->

# Novolis.Snapshots.Zip

Zip archive **`ISnapshotStore`** that writes `{objectId}.zip` under a sharded root. Each archive contains `state.dat` (serialized state) and `manifest.json` (label, kind, properties from `SnapshotRequest`).

## Install

```bash
dotnet add package Novolis.Snapshots.Zip
```

Depends on **System.IO.Abstractions** and `Novolis.Snapshots.Abstractions`.

## Quick start

```csharp
using System.IO.Abstractions;
using Novolis.Snapshots;
using Novolis.Snapshots.Json;
using Novolis.Snapshots.Zip;

var fs = new FileSystem();
var root = fs.DirectoryInfo.New(@"C:\snapshots");
var store = new ZipSnapshotStore<MyState>(fs, root, new JsonStateSerializer<MyState>());

var snapshot = await store.SaveAsync(
    state,
    new SnapshotRequest("Before export", SnapshotKinds.ExportCheckpoint));
```

Entry names: `ZipSnapshotStore<TState>.StateEntryName` (`state.dat`), `ManifestEntryName` (`manifest.json`).

## API

| Type | Role |
|------|------|
| `ZipSnapshotStore<TState>` | `ISnapshotStore<TState, ZipSnapshotRef>` |
| `ZipSnapshotStore<TState>.StateEntryName` | Serialized state entry |
| `ZipSnapshotStore<TState>.ManifestEntryName` | Snapshot metadata entry |

## Related

| Package | Role |
|---------|------|
| `Novolis.Snapshots.Abstractions` | `ZipSnapshotRef`, store contract |
| `Novolis.Snapshots.Json` | Typical serializer pairing |
| `Novolis.Workspaces.Snapshots` | Whole-workspace zip capture |
| `Novolis.Workspaces.Timeline` | Save/restore points using `ZipSnapshotRef` |
| `Novolis.Timeline.Abstractions` | Timeline nodes hold `ZipSnapshotRef` |

## Notes

Workspace-level zip snapshots (full tree, not single-state serialization) are in `Novolis.Workspaces.Snapshots`. See [design.md](../../docs/design.md).

