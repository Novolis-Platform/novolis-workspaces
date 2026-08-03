<!-- novolis-pkg-brand:start -->
<p align="center">
  <a href="https://github.com/Novolis-Platform/novolis-workspaces">
    <img src="https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-icon.svg" width="72" alt="Novolis"/>
  </a>
</p>
<!-- novolis-pkg-brand:end -->

# Novolis.Snapshots.FileSystem

File-system **`ISnapshotStore`** that writes serialized state blobs under a root directory (`{hash-prefix}/{guid}.dat`) and returns a `FileSnapshotRef` with a SHA-256 content id.

## Install

```bash
dotnet add package Novolis.Snapshots.FileSystem
```

Depends on **System.IO.Abstractions** and `Novolis.Snapshots.Abstractions`.

## Quick start

```csharp
using System.IO.Abstractions;
using Novolis.Snapshots;
using Novolis.Snapshots.FileSystem;
using Novolis.Snapshots.Json;

var fs = new FileSystem();
var root = fs.DirectoryInfo.New(@"C:\snapshots");
var serializer = new JsonStateSerializer<MyState>();
var store = new FileSnapshotStore<MyState>(fs, root, serializer);

var snapshot = await store.SaveAsync(state, new SnapshotRequest(null, SnapshotKinds.Autosave));
await store.RestoreAsync(state, snapshot);
```

## API

| Type | Role |
|------|------|
| `FileSnapshotStore<TState>` | `ISnapshotStore<TState, FileSnapshotRef>` |

Constructor: `(IFileSystem fileSystem, IDirectoryInfo root, IStateSerializer<TState> serializer)`. Creates the root directory if missing.

## Related

| Package | Role |
|---------|------|
| `Novolis.Snapshots.Abstractions` | `FileSnapshotRef`, store contract |
| `Novolis.Snapshots.Json` | Typical serializer pairing |
| `Novolis.Snapshots.Zip` | Zip archive alternative with manifest |
| `Novolis.Snapshots.Memory` | Non-durable test backend |
| `Novolis.Timeline.FileSystem` | Timeline nodes referencing snapshot refs |

