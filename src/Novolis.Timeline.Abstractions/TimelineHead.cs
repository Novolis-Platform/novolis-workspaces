namespace Novolis.Timeline;

/// <summary>Current head node per branch.</summary>
public sealed record TimelineHead(IReadOnlyDictionary<BranchId, TimelineNodeId> NodesByBranch);
