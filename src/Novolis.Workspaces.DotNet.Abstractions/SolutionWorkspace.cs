using System.IO.Abstractions;
using RootWorkspace = Novolis.IO.Workspace.IWorkspace;

namespace Novolis.Workspaces.DotNet;

/// <summary>Absolute path to a .slnx or .sln solution file.</summary>
public sealed record SolutionFilePath
{
    public SolutionFilePath(string fullPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullPath);
        FullPath = Path.GetFullPath(fullPath);
        if (!Path.HasExtension(FullPath)
            || (!FullPath.EndsWith(".slnx", StringComparison.OrdinalIgnoreCase)
                && !FullPath.EndsWith(".sln", StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("A solution path must end in .slnx or .sln.", nameof(fullPath));
        }
    }

    public string FullPath { get; }
}

/// <summary>A .NET solution rooted at the directory containing its solution file.</summary>
public sealed class SolutionWorkspace : RootWorkspace
{
    public SolutionWorkspace(IDirectoryInfo root, SolutionFilePath solutionFile)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(solutionFile);
        if (!IsContainedBy(root.FullName, solutionFile.FullPath))
            throw new ArgumentException("The solution file must be contained by the workspace root.", nameof(solutionFile));

        Root = root;
        SolutionFile = solutionFile;
    }

    public IDirectoryInfo Root { get; }
    public SolutionFilePath SolutionFile { get; }

    private static bool IsContainedBy(string rootPath, string filePath)
    {
        var normalizedRoot = rootPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;
        return filePath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>An arbitrary set of solutions with no shared-root guarantee.</summary>
public sealed record SolutionWorkspaceSet(IReadOnlyList<SolutionWorkspace> Members);

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

/// <summary>Severity of a workspace operation diagnostic.</summary>
public enum WorkspaceDiagnosticSeverity
{
    Info = 0,
    Warning = 1,
    Error = 2,
}

/// <summary>A portable diagnostic tied to a workspace file or root.</summary>
public sealed record WorkspaceDiagnostic(
    string Code,
    string Message,
    WorkspaceDiagnosticSeverity Severity,
    string? Path = null);

/// <summary>Named inputs used when evaluating an MSBuild project.</summary>
public sealed record EvaluationContext(
    string Configuration = "Debug",
    string Platform = "AnyCPU",
    string? TargetFramework = null,
    bool AllowEvaluation = false,
    bool AllowDesignTimeBuilds = false,
    IReadOnlyDictionary<string, string>? GlobalProperties = null)
{
    public IReadOnlyDictionary<string, string> ToGlobalProperties()
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Configuration"] = Configuration,
            ["Platform"] = Platform,
        };

        if (!string.IsNullOrWhiteSpace(TargetFramework))
            values["TargetFramework"] = TargetFramework;

        if (GlobalProperties is not null)
        {
            foreach (var pair in GlobalProperties)
                values[pair.Key] = pair.Value;
        }

        return values;
    }
}
