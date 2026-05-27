namespace Novolis.Snapshots;

/// <summary>Snapshot store operation failed.</summary>
public sealed class SnapshotException : Exception
{
    public SnapshotException(string message) : base(message) { }

    public SnapshotException(string message, Exception inner) : base(message, inner) { }
}
