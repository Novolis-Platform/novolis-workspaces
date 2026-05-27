using System.IO.Abstractions;
using Novolis.Workspaces.FileSystem;

namespace Novolis.Workspaces.Projects.Timeline;

/// <summary>Default include rules for a single project subtree.</summary>
public sealed class DefaultProjectSnapshotPolicy : IProjectSnapshotPolicy
{
    public bool ShouldInclude(IProject project, IFileInfo file)
    {
        var relative = GetRelativePath(project.Root.FullName, file.FullName);
        if (string.IsNullOrEmpty(relative))
            return false;

        var normalized = relative.Replace('\\', '/');
        if (normalized.Contains("/cache/", StringComparison.OrdinalIgnoreCase)
            || normalized.Contains("/temp/", StringComparison.OrdinalIgnoreCase)
            || normalized.Contains("/outputs/", StringComparison.OrdinalIgnoreCase))
            return false;

        return normalized.EndsWith($"/{WorkspaceLayout.ProjectManifestFile}", StringComparison.OrdinalIgnoreCase)
               || normalized.Contains("/documents/", StringComparison.OrdinalIgnoreCase)
               || normalized.Contains("/assets/", StringComparison.OrdinalIgnoreCase)
               || normalized.Contains("/settings/", StringComparison.OrdinalIgnoreCase)
               || normalized.Contains("/presets/", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetRelativePath(string root, string fullPath)
    {
        var rootWithSep = root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;
        return fullPath.StartsWith(rootWithSep, StringComparison.OrdinalIgnoreCase)
            ? fullPath[rootWithSep.Length..]
            : string.Empty;
    }
}
