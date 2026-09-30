namespace Novolis.Workspaces;

/// <summary>Identifies a workspace container.</summary>
public readonly record struct WorkspaceId(Guid Value)
{
    public static WorkspaceId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString("D");
}
