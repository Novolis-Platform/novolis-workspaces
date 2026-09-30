namespace Novolis.Timeline.Presentation;

/// <summary>Hierarchical timeline view for tree controls.</summary>
public sealed record TimelineTreeView(IReadOnlyList<TimelineTreeNode> Roots);
