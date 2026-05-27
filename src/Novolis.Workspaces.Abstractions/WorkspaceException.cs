namespace Novolis.Workspaces;

/// <summary>Workspace operation failed.</summary>
public sealed class WorkspaceException : Exception
{
    public WorkspaceException(string message) : base(message) { }

    public WorkspaceException(string message, Exception inner) : base(message, inner) { }
}
