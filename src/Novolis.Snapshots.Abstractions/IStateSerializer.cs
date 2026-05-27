namespace Novolis.Snapshots;

/// <summary>Serializes state to and from a byte stream (below <see cref="ISnapshotStore{TState,TSnapshotRef}"/>).</summary>
public interface IStateSerializer<TState>
{
    ValueTask WriteAsync(TState state, Stream destination, CancellationToken cancellationToken = default);

    ValueTask ReadAsync(TState target, Stream source, CancellationToken cancellationToken = default);
}
