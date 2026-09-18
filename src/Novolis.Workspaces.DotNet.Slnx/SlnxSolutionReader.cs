using System.Xml.Linq;

namespace Novolis.Workspaces.DotNet.Slnx;

/// <summary>One project entry declared by an SLNX solution document.</summary>
public sealed record SlnxProjectEntry(string RelativePath, string FullPath, string? Name);

/// <summary>One solution folder declared by an SLNX solution document.</summary>
public sealed record SlnxSolutionFolder(string Name);

/// <summary>Read-only physical topology projected from an SLNX file.</summary>
public sealed record SlnxSolutionTopology(
    SolutionWorkspace Workspace,
    IReadOnlyList<SlnxProjectEntry> Projects,
    IReadOnlyList<SlnxSolutionFolder> Folders,
    IReadOnlyList<WorkspaceDiagnostic> Diagnostics);

/// <summary>Reads the topology of modern XML solution files without rewriting them.</summary>
public sealed class SlnxSolutionReader
{
    public async ValueTask<SlnxSolutionTopology> ReadAsync(
        SolutionWorkspace workspace,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(workspace);

        var diagnostics = new List<WorkspaceDiagnostic>();
        if (!workspace.SolutionFile.FullPath.EndsWith(".slnx", StringComparison.OrdinalIgnoreCase))
        {
            diagnostics.Add(new WorkspaceDiagnostic(
                "NWS1001",
                "The physical SLNX reader accepts only .slnx solution files.",
                WorkspaceDiagnosticSeverity.Error,
                workspace.SolutionFile.FullPath));
            return new SlnxSolutionTopology(workspace, [], [], diagnostics);
        }

        try
        {
            await using var stream = File.OpenRead(workspace.SolutionFile.FullPath);
            var document = await XDocument.LoadAsync(stream, LoadOptions.PreserveWhitespace, cancellationToken)
                .ConfigureAwait(false);
            var solutionRoot = document.Root;
            if (solutionRoot is null || !string.Equals(solutionRoot.Name.LocalName, "Solution", StringComparison.Ordinal))
            {
                diagnostics.Add(new WorkspaceDiagnostic(
                    "NWS1002",
                    "The document does not have a Solution root element.",
                    WorkspaceDiagnosticSeverity.Error,
                    workspace.SolutionFile.FullPath));
                return new SlnxSolutionTopology(workspace, [], [], diagnostics);
            }

            var projects = solutionRoot
                .Descendants()
                .Where(element => string.Equals(element.Name.LocalName, "Project", StringComparison.Ordinal))
                .Select(element => ToProjectEntry(workspace, element, diagnostics))
                .Where(entry => entry is not null)
                .Cast<SlnxProjectEntry>()
                .ToArray();

            var folders = solutionRoot
                .Descendants()
                .Where(element => string.Equals(element.Name.LocalName, "Folder", StringComparison.Ordinal))
                .Select(element => (string?)element.Attribute("Name"))
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => new SlnxSolutionFolder(name!))
                .ToArray();

            return new SlnxSolutionTopology(workspace, projects, folders, diagnostics);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or System.Xml.XmlException)
        {
            diagnostics.Add(new WorkspaceDiagnostic(
                "NWS1003",
                exception.Message,
                WorkspaceDiagnosticSeverity.Error,
                workspace.SolutionFile.FullPath));
            return new SlnxSolutionTopology(workspace, [], [], diagnostics);
        }
    }

    private static SlnxProjectEntry? ToProjectEntry(
        SolutionWorkspace workspace,
        XElement element,
        ICollection<WorkspaceDiagnostic> diagnostics)
    {
        var relativePath = (string?)element.Attribute("Path");
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            diagnostics.Add(new WorkspaceDiagnostic(
                "NWS1004",
                "An SLNX Project element is missing its Path attribute.",
                WorkspaceDiagnosticSeverity.Warning,
                workspace.SolutionFile.FullPath));
            return null;
        }

        var fullPath = Path.GetFullPath(Path.Combine(workspace.Root.FullName, relativePath));
        return new SlnxProjectEntry(relativePath, fullPath, (string?)element.Attribute("Name"));
    }
}
