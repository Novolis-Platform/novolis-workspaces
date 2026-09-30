namespace Novolis.Timeline;

/// <summary>Identifies a node in a timeline graph.</summary>
public readonly record struct TimelineNodeId(Guid Value)
{
    public static TimelineNodeId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString("D");
}
