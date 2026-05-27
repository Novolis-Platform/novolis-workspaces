namespace Novolis.Timeline;

/// <summary>A node in the timeline graph referencing a snapshot.</summary>
public sealed record TimelineNode<TSnapshotRef>(
    TimelineNodeId Id,
    TimelineNodeId? ParentId,
    BranchId BranchId,
    TSnapshotRef Snapshot,
    TimelineMetadata Metadata,
    DateTimeOffset CreatedAt);
