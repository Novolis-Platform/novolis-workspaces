using Novolis.Snapshots;
using Novolis.Timeline;

namespace Novolis.Workspaces.Timeline;

/// <summary>Composes workspace zip snapshots with a branchable timeline.</summary>
public sealed class WorkspaceTimeline
{
    private readonly ITimeline<ZipSnapshotRef> _timeline;
    private readonly ISnapshotStore<IWorkspace, ZipSnapshotRef> _snapshots;

    public WorkspaceTimeline(
        ITimeline<ZipSnapshotRef> timeline,
        ISnapshotStore<IWorkspace, ZipSnapshotRef> snapshots)
    {
        _timeline = timeline;
        _snapshots = snapshots;
    }

    public async ValueTask<TimelineNode<ZipSnapshotRef>> SavePointAsync(
        IWorkspace workspace,
        SavePointRequest request,
        CancellationToken cancellationToken = default)
    {
        var snapshot = await _snapshots.SaveAsync(workspace, request.ToSnapshotRequest(), cancellationToken).ConfigureAwait(false);
        return await _timeline.AddAsync(snapshot, request.ToMetadata(), cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask RestorePointAsync(
        IWorkspace workspace,
        TimelineNodeId nodeId,
        bool moveHead = true,
        CancellationToken cancellationToken = default)
    {
        var nodes = await _timeline.GetNodesAsync(cancellationToken).ConfigureAwait(false);
        var node = nodes.FirstOrDefault(n => n.Id == nodeId)
            ?? throw new TimelineException($"Timeline node '{nodeId}' was not found.");

        await SavePointAsync(
            workspace,
            new SavePointRequest("Before restore", SnapshotKinds.Safety),
            cancellationToken).ConfigureAwait(false);

        await _snapshots.RestoreAsync(workspace, node.Snapshot, cancellationToken).ConfigureAwait(false);

        if (moveHead)
            await _timeline.MoveHeadAsync(node.BranchId, node.Id, cancellationToken).ConfigureAwait(false);
    }

    public ValueTask<Branch> BranchFromAsync(
        BranchName name,
        TimelineNodeId from,
        CancellationToken cancellationToken = default) =>
        _timeline.BranchAsync(name, from, cancellationToken);
}
