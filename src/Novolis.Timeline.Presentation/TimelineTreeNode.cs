namespace Novolis.Timeline.Presentation;

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
