namespace Novolis.Snapshots;

/// <summary>Persists and restores application state as opaque snapshots (save-game primitive).</summary>
public interface ISnapshotStore<TState, TSnapshotRef>
{
    ValueTask<TSnapshotRef> SaveAsync(
        TState state,
        SnapshotRequest request,
        CancellationToken cancellationToken = default);

    ValueTask RestoreAsync(
        TState target,
        TSnapshotRef snapshot,
        CancellationToken cancellationToken = default);
}
