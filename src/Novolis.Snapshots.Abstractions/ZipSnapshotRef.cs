namespace Novolis.Snapshots;

/// <summary>Reference to a zip archive entry containing serialized state.</summary>
public sealed record ZipSnapshotRef(string ObjectId, string RelativePath);
