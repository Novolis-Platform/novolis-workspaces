namespace Novolis.Timeline.Presentation;

/// <summary>UI-safe metadata for a timeline node.</summary>
public sealed record TimelinePresentationMetadata(
    string Label,
    string Kind,
    IReadOnlyDictionary<string, string> Properties);

/// <summary>Hierarchical timeline view for tree controls.</summary>
public sealed record TimelineTreeView(IReadOnlyList<TimelineTreeNode> Roots);

/// <summary>Single node in a timeline tree.</summary>
public sealed record TimelineTreeNode(
    TimelineNodeId Id,
    TimelineNodeId? ParentId,
    int Depth,
    int SiblingIndex,
    bool HasChildren,
    bool IsBranchPoint,
    bool IsLeaf,
    bool IsHead,
    string BranchName,
    TimelinePresentationMetadata Presentation,
    IReadOnlyList<TimelineTreeNode> Children,
    DateTimeOffset CreatedAt);

/// <summary>Flat row for virtualized lists.</summary>
public sealed record TimelineTreeRow(
    TimelineNodeId Id,
    int Depth,
    string Label,
    string Branch,
    bool IsHead,
    bool IsBranchPoint,
    bool IsCurrentBranch,
    DateTimeOffset CreatedAt);
