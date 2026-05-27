namespace Novolis.Timeline.Presentation;

/// <summary>Projects storage timeline nodes into UI tree models.</summary>
public interface ITimelineProjector<TSnapshotRef>
{
    TimelineTreeView ToTree(
        IReadOnlyList<TimelineNode<TSnapshotRef>> nodes,
        IReadOnlyList<Branch> branches,
        TimelineHead head);

    IReadOnlyList<TimelineTreeRow> ToRows(
        IReadOnlyList<TimelineNode<TSnapshotRef>> nodes,
        IReadOnlyList<Branch> branches,
        TimelineHead head,
        BranchId? currentBranchId = null);
}
