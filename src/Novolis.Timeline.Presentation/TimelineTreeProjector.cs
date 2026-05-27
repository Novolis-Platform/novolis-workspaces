namespace Novolis.Timeline.Presentation;

/// <summary>Default tree and flat-row projection for timelines.</summary>
public sealed class TimelineTreeProjector<TSnapshotRef> : ITimelineProjector<TSnapshotRef>
{
    public TimelineTreeView ToTree(
        IReadOnlyList<TimelineNode<TSnapshotRef>> nodes,
        IReadOnlyList<Branch> branches,
        TimelineHead head)
    {
        var branchNames = branches.ToDictionary(b => b.Id, b => b.Name.Value);
        var childrenByParent = nodes
            .Where(n => n.ParentId is not null)
            .GroupBy(n => n.ParentId!.Value)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<TimelineNode<TSnapshotRef>>)g.OrderBy(n => n.CreatedAt).ToArray());

        var roots = BuildLevel(parentId: null, depth: 0, branchNames, childrenByParent, nodes, head);
        return new TimelineTreeView(roots);
    }

    public IReadOnlyList<TimelineTreeRow> ToRows(
        IReadOnlyList<TimelineNode<TSnapshotRef>> nodes,
        IReadOnlyList<Branch> branches,
        TimelineHead head,
        BranchId? currentBranchId = null)
    {
        var tree = ToTree(nodes, branches, head);
        var rows = new List<TimelineTreeRow>();
        foreach (var root in tree.Roots)
            AppendRows(root, rows, currentBranchId ?? BranchId.Main);
        return rows;
    }

    private static void AppendRows(TimelineTreeNode node, List<TimelineTreeRow> rows, BranchId currentBranchId)
    {
        rows.Add(new TimelineTreeRow(
            node.Id,
            node.Depth,
            node.Presentation.Label,
            node.BranchName,
            node.IsHead,
            node.IsBranchPoint,
            node.BranchName.Equals(GetBranchName(currentBranchId), StringComparison.OrdinalIgnoreCase),
            node.CreatedAt));

        foreach (var child in node.Children)
            AppendRows(child, rows, currentBranchId);
    }

    private static string GetBranchName(BranchId branchId) =>
        branchId == BranchId.Main ? "main" : branchId.ToString();

    private IReadOnlyList<TimelineTreeNode> BuildLevel(
        TimelineNodeId? parentId,
        int depth,
        IReadOnlyDictionary<BranchId, string> branchNames,
        IReadOnlyDictionary<TimelineNodeId, IReadOnlyList<TimelineNode<TSnapshotRef>>> childrenByParent,
        IReadOnlyList<TimelineNode<TSnapshotRef>> allNodes,
        TimelineHead head)
    {
        var level = parentId is null
            ? allNodes.Where(n => n.ParentId is null).OrderBy(n => n.CreatedAt).ToArray()
            : childrenByParent.TryGetValue(parentId.Value, out var children)
                ? children
                : [];

        var result = new List<TimelineTreeNode>();
        var siblingIndex = 0;
        foreach (var node in level)
        {
            var childNodes = BuildLevel(node.Id, depth + 1, branchNames, childrenByParent, allNodes, head);
            var branchName = branchNames.TryGetValue(node.BranchId, out var name) ? name : node.BranchId.ToString();
            var isHead = head.NodesByBranch.TryGetValue(node.BranchId, out var headNode) && headNode == node.Id;
            var presentation = new TimelinePresentationMetadata(
                node.Metadata.Label ?? node.Metadata.Kind,
                node.Metadata.Kind,
                node.Metadata.Properties);

            result.Add(new TimelineTreeNode(
                node.Id,
                node.ParentId,
                depth,
                siblingIndex++,
                childNodes.Count > 0,
                childNodes.Count > 1,
                childNodes.Count == 0,
                isHead,
                branchName,
                presentation,
                childNodes,
                node.CreatedAt));
        }

        return result;
    }
}
