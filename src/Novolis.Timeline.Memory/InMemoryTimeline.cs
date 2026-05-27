using System.Collections.Concurrent;

namespace Novolis.Timeline.Memory;

/// <summary>Process-local timeline graph. Single-writer recommended.</summary>
public sealed class InMemoryTimeline<TSnapshotRef> : ITimeline<TSnapshotRef>
{
    private readonly ConcurrentDictionary<TimelineNodeId, TimelineNode<TSnapshotRef>> _nodes = new();
    private readonly ConcurrentDictionary<BranchId, Branch> _branches = new();
    private readonly ConcurrentDictionary<BranchId, TimelineNodeId> _heads = new();

    public InMemoryTimeline()
    {
        _branches[BranchId.Main] = new Branch(BranchId.Main, new BranchName("main"), null);
    }

    public ValueTask<TimelineNode<TSnapshotRef>> AddAsync(
        TSnapshotRef snapshot,
        TimelineMetadata metadata,
        TimelineNodeId? parentId = null,
        BranchId? branchId = null,
        CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        var branch = branchId ?? BranchId.Main;
        if (!_branches.ContainsKey(branch))
            throw new TimelineException($"Branch '{branch}' does not exist.");

        var resolvedParent = parentId;
        if (resolvedParent is null && _heads.TryGetValue(branch, out var head))
            resolvedParent = head;

        var node = new TimelineNode<TSnapshotRef>(
            TimelineNodeId.New(),
            resolvedParent,
            branch,
            snapshot,
            metadata,
            DateTimeOffset.UtcNow);

        _nodes[node.Id] = node;
        _heads[branch] = node.Id;
        return ValueTask.FromResult(node);
    }

    public ValueTask<Branch> BranchAsync(BranchName name, TimelineNodeId from, CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        if (!_nodes.ContainsKey(from))
            throw new TimelineException($"Node '{from}' was not found.");

        var branch = new Branch(BranchId.New(), name, from);
        _branches[branch.Id] = branch;
        _heads[branch.Id] = from;
        return ValueTask.FromResult(branch);
    }

    public ValueTask MoveHeadAsync(BranchId branch, TimelineNodeId node, CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        if (!_branches.ContainsKey(branch))
            throw new TimelineException($"Branch '{branch}' does not exist.");
        if (!_nodes.TryGetValue(node, out var timelineNode) || timelineNode.BranchId != branch)
            throw new TimelineException($"Node '{node}' is not on branch '{branch}'.");

        _heads[branch] = node;
        return ValueTask.CompletedTask;
    }

    public ValueTask<IReadOnlyList<TimelineNode<TSnapshotRef>>> GetNodesAsync(CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        IReadOnlyList<TimelineNode<TSnapshotRef>> nodes = _nodes.Values.OrderBy(n => n.CreatedAt).ToArray();
        return ValueTask.FromResult(nodes);
    }

    public ValueTask<IReadOnlyList<Branch>> GetBranchesAsync(CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        IReadOnlyList<Branch> branches = _branches.Values.OrderBy(b => b.Name.Value).ToArray();
        return ValueTask.FromResult(branches);
    }

    public ValueTask<TimelineHead> GetHeadAsync(CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        return ValueTask.FromResult(new TimelineHead(new Dictionary<BranchId, TimelineNodeId>(_heads)));
    }
}
