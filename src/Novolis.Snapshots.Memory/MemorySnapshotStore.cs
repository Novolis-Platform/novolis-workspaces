using System.Collections.Concurrent;

namespace Novolis.Snapshots.Memory;

/// <summary>Stores snapshots in a process-local dictionary using an <see cref="IStateSerializer{TState}"/> round-trip clone.</summary>
public sealed class MemorySnapshotStore<TState> : ISnapshotStore<TState, MemorySnapshotRef>
{
    private readonly IStateSerializer<TState> _serializer;
    private readonly Func<TState> _factory;
    private readonly ConcurrentDictionary<Guid, byte[]> _blobs = new();

    public MemorySnapshotStore(IStateSerializer<TState> serializer, Func<TState> stateFactory)
    {
        _serializer = serializer;
        _factory = stateFactory;
    }

    public async ValueTask<MemorySnapshotRef> SaveAsync(
        TState state,
        SnapshotRequest request,
        CancellationToken cancellationToken = default)
    {
        _ = request;
        using var ms = new MemoryStream();
        await _serializer.WriteAsync(state, ms, cancellationToken).ConfigureAwait(false);
        var id = Guid.NewGuid();
        _blobs[id] = ms.ToArray();
        return new MemorySnapshotRef(id);
    }

    public async ValueTask RestoreAsync(
        TState target,
        MemorySnapshotRef snapshot,
        CancellationToken cancellationToken = default)
    {
        if (!_blobs.TryGetValue(snapshot.Id, out var bytes))
            throw new SnapshotException($"Memory snapshot '{snapshot.Id}' was not found.");

        using var ms = new MemoryStream(bytes, writable: false);
        await _serializer.ReadAsync(target, ms, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Creates a new state instance restored from the snapshot.</summary>
    public async ValueTask<TState> LoadAsync(MemorySnapshotRef snapshot, CancellationToken cancellationToken = default)
    {
        var state = _factory();
        await RestoreAsync(state, snapshot, cancellationToken).ConfigureAwait(false);
        return state;
    }
}
