namespace Novolis.Timeline;

/// <summary>Named alternate path in a timeline.</summary>
public sealed record Branch(BranchId Id, BranchName Name, TimelineNodeId? ForkedFromNodeId);

/// <summary>Display name for a branch.</summary>
public readonly record struct BranchName(string Value)
{
    public override string ToString() => Value;
}
