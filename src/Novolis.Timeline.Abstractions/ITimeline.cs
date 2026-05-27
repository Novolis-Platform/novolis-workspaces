namespace Novolis.Timeline;

/// <summary>
/// Graph of snapshot references with branches. Not version control: no merge, rebase, remotes, or conflict resolution.
/// </summary>
public interface ITimeline<TSnapshotRef>
{
    ValueTask<TimelineNode<TSnapshotRef>> AddAsync(
        TSnapshotRef snapshot,
        TimelineMetadata metadata,
        TimelineNodeId? parentId = null,
        BranchId? branchId = null,
        CancellationToken cancellationToken = default);

    ValueTask<Branch> BranchAsync(
        BranchName name,
        TimelineNodeId from,
        CancellationToken cancellationToken = default);

    ValueTask MoveHeadAsync(
        BranchId branch,
        TimelineNodeId node,
        CancellationToken cancellationToken = default);

    ValueTask<IReadOnlyList<TimelineNode<TSnapshotRef>>> GetNodesAsync(
        CancellationToken cancellationToken = default);

    ValueTask<IReadOnlyList<Branch>> GetBranchesAsync(
        CancellationToken cancellationToken = default);

    ValueTask<TimelineHead> GetHeadAsync(CancellationToken cancellationToken = default);
}
