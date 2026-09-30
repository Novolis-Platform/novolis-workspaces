namespace Novolis.Timeline;

/// <summary>Identifies a branch in a timeline.</summary>
public readonly record struct BranchId(Guid Value)
{
    public static BranchId New() => new(Guid.NewGuid());

    public static BranchId Main { get; } = new(Guid.Parse("00000000-0000-0000-0000-000000000001"));

    public override string ToString() => Value.ToString("D");
}
