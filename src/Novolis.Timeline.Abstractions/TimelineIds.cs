namespace Novolis.Timeline;

/// <summary>Identifies a node in a timeline graph.</summary>
public readonly record struct TimelineNodeId(Guid Value)
{
    public static TimelineNodeId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString("D");
}

/// <summary>Identifies a branch in a timeline.</summary>
public readonly record struct BranchId(Guid Value)
{
    public static BranchId New() => new(Guid.NewGuid());

    public static BranchId Main { get; } = new(Guid.Parse("00000000-0000-0000-0000-000000000001"));

    public override string ToString() => Value.ToString("D");
}
