namespace Novolis.Snapshots;

/// <summary>Metadata for a snapshot write (manual save, autosave, safety checkpoint, etc.).</summary>
public sealed record SnapshotRequest(
    string? Label,
    string Kind,
    IReadOnlyDictionary<string, string>? Properties = null);
