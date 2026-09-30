using System.IO.Abstractions;
using RootWorkspace = Novolis.IO.Workspace.IWorkspace;

namespace Novolis.Workspaces.DotNet;

/// <summary>A collection of solutions whose roots all occur beneath one discovery root.</summary>
public sealed class MultiSolutionWorkspace : RootWorkspace
{
    public MultiSolutionWorkspace(IDirectoryInfo root, IReadOnlyList<SolutionWorkspace> members)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(members);

        Root = root;
        Members = members;
        var containedRoot = root.FullName.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;
        if (members.Any(member => !member.Root.FullName.StartsWith(containedRoot, StringComparison.OrdinalIgnoreCase)
                                   && !string.Equals(member.Root.FullName, root.FullName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("All solution workspaces must be contained by the group root.", nameof(members));
        }
    }

    public IDirectoryInfo Root { get; }
    public IReadOnlyList<SolutionWorkspace> Members { get; }
}
