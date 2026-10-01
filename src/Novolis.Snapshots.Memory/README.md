<!-- novolis-pkg-brand:start -->
[![Novolis](https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-icon.png)](https://novolis-platform.github.io/.github/novolis-workspaces/)

[Novolis](https://github.com/Novolis-Platform) · [Docs](https://novolis-platform.github.io/.github/novolis-workspaces/) · [Source](https://github.com/Novolis-Platform/novolis-workspaces)
<!-- novolis-pkg-brand:end -->

# Novolis.Snapshots.Memory

In-process **`ISnapshotStore`** that clones state via an `IStateSerializer` round-trip into a `ConcurrentDictionary`. Useful for tests and ephemeral undo stacks.

## Install

```bash
dotnet add package Novolis.Snapshots.Memory
```

Depends on `Novolis.Snapshots.Abstractions`.

## Quick start

```csharp
using Novolis.Snapshots;
using Novolis.Snapshots.Json;
using Novolis.Snapshots.Memory;

var serializer = new JsonStateSerializer<Dictionary<string, string>>();
var store = new MemorySnapshotStore<Dictionary<string, string>>(serializer, () => new());

var snapshot = await store.SaveAsync(
    new Dictionary<string, string> { ["scene"] = "draft" },
    new SnapshotRequest("Checkpoint", SnapshotKinds.Manual));

var restored = await store.LoadAsync(snapshot);
```

## API

| Type | Role |
|------|------|
| `MemorySnapshotStore<TState>` | `ISnapshotStore<TState, MemorySnapshotRef>` |
| `MemorySnapshotStore<TState>.LoadAsync` | Factory + restore into a new instance |

## Related

| Package | Role |
|---------|------|
| `Novolis.Snapshots.Abstractions` | `ISnapshotStore`, `MemorySnapshotRef` |
| `Novolis.Snapshots.Json` | Common serializer for memory stores |
| `Novolis.Snapshots.FileSystem` | Durable blob backend |
| `Novolis.Snapshots.Zip` | Zip archive backend |
| `Novolis.Timeline.Memory` | In-memory timeline over snapshot refs |

