namespace Novolis.Timeline.Presentation;

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
