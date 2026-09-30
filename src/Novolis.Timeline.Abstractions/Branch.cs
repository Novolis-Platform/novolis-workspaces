namespace Novolis.Timeline;

/// <summary>Named alternate path in a timeline.</summary>
public sealed record Branch(BranchId Id, BranchName Name, TimelineNodeId? ForkedFromNodeId);
