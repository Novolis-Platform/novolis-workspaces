<!-- novolis-pkg-brand:start -->
[![Novolis](https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-icon.png)](https://novolis-platform.github.io/.github/novolis-workspaces/)

[Novolis](https://github.com/Novolis-Platform) · [Docs](https://novolis-platform.github.io/.github/novolis-workspaces/) · [Source](https://github.com/Novolis-Platform/novolis-workspaces)
<!-- novolis-pkg-brand:end -->

# Novolis.Snapshots.Json

JSON **`IStateSerializer<TState>`** using `System.Text.Json`. Deserializes into a temporary instance, then copies public writable properties (or dictionary entries) into the target object.

## Install

```bash
dotnet add package Novolis.Snapshots.Json
```

Depends on `Novolis.Snapshots.Abstractions`.

## Quick start

```csharp
using Novolis.Snapshots.Json;

var serializer = new JsonStateSerializer<MyState>(new JsonSerializerOptions { WriteIndented = true });

using var ms = new MemoryStream();
await serializer.WriteAsync(state, ms);

ms.Position = 0;
await serializer.ReadAsync(existingState, ms);
```

## API

| Type | Role |
|------|------|
| `JsonStateSerializer<TState>` | `IStateSerializer<TState>` where `TState : class` |

Constructor accepts optional `JsonSerializerOptions` (default: compact JSON).

## Related

| Package | Role |
|---------|------|
| `Novolis.Snapshots.Abstractions` | `IStateSerializer<TState>` contract |
| `Novolis.Snapshots.Memory` | Uses serializer for in-process clones |
| `Novolis.Snapshots.FileSystem` | Persists serializer output to disk |
| `Novolis.Snapshots.Zip` | Embeds serializer output in zip entries |
| `Novolis.Timeline.FileSystem` | Reuses JSON options for timeline persistence |

