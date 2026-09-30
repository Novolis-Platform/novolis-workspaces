namespace Novolis.Workspaces;

/// <summary>Identifies a project within a workspace.</summary>
public readonly record struct ProjectId(Guid Value)
{
    public static ProjectId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString("D");
}
