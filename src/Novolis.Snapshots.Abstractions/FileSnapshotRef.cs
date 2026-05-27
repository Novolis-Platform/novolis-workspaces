namespace Novolis.Snapshots;

/// <summary>Reference to a blob stored on the file system.</summary>
public sealed record FileSnapshotRef(string RelativePath, string ContentId);
