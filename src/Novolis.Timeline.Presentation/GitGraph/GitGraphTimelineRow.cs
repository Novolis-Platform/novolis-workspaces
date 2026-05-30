using Novolis.Timeline;

namespace Novolis.Timeline.Presentation.GitGraph;

/// <summary>One row in a <c>git log --graph</c>-style timeline list.</summary>
public sealed record GitGraphTimelineRow(
    TimelineNodeId Id,
    string Graph,
    string Subject,
    string BranchName,
    string SnapshotKind,
    GraphRgb BranchColor,
    GraphRgb KindColor,
    bool IsHere,
    bool IsBranchPoint,
    string Marker,
    DateTimeOffset CreatedAt);
