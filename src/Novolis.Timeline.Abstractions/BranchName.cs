namespace Novolis.Timeline;

/// <summary>Display name for a branch.</summary>
public readonly record struct BranchName(string Value)
{
    public override string ToString() => Value;
}
