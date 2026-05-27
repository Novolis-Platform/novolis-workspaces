using System.IO.Abstractions;
using System.Text.Json;

namespace Novolis.Workspaces.FileSystem;

/// <summary>Creates and opens workspaces on disk.</summary>
public sealed class WorkspaceFileSystemService
{
    private readonly IFileSystem _fileSystem;
    private readonly JsonSerializerOptions _jsonOptions;

    public WorkspaceFileSystemService(IFileSystem fileSystem, JsonSerializerOptions? jsonOptions = null)
    {
        _fileSystem = fileSystem;
        _jsonOptions = jsonOptions ?? new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    }

    public async ValueTask<PhysicalWorkspace> CreateAsync(
        string workspaceRootPath,
        string name,
        CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        var root = _fileSystem.DirectoryInfo.New(workspaceRootPath);
        if (!root.Exists)
            root.Create();

        CreateLayout(root.FullName);

        var workspaceId = WorkspaceId.New();
        var manifest = new WorkspaceManifest(workspaceId, name, WorkspaceLayout.CurrentSchemaVersion, []);
        WriteWorkspaceManifest(root.FullName, manifest);

        return await OpenAsync(workspaceRootPath, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<PhysicalWorkspace> OpenAsync(
        string workspaceRootPath,
        CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        var root = _fileSystem.DirectoryInfo.New(workspaceRootPath);
        if (!root.Exists)
            throw new WorkspaceException($"Workspace root '{workspaceRootPath}' does not exist.");

        var manifestPath = WorkspaceLayout.WorkspaceManifestPath(root.FullName);
        if (!_fileSystem.File.Exists(manifestPath))
            throw new WorkspaceException($"Workspace manifest was not found at '{manifestPath}'.");

        var manifest = JsonSerializer.Deserialize<WorkspaceManifest>(
            await _fileSystem.File.ReadAllTextAsync(manifestPath, cancellationToken).ConfigureAwait(false),
            _jsonOptions) ?? throw new WorkspaceException("Workspace manifest is invalid.");

        var projects = LoadProjects(root.FullName, manifest);
        return new PhysicalWorkspace(manifest, root, projects);
    }

    public async ValueTask<PhysicalProject> AddProjectAsync(
        PhysicalWorkspace workspace,
        string name,
        ProjectKind kind,
        string? folderName = null,
        CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        var projectId = ProjectId.New();
        folderName ??= Slugify(name) + "-" + projectId.Value.ToString("N")[..8];
        var projectRoot = WorkspaceLayout.ProjectRoot(workspace.Root.FullName, folderName);
        _fileSystem.Directory.CreateDirectory(projectRoot);
        _fileSystem.Directory.CreateDirectory(_fileSystem.Path.Combine(projectRoot, WorkspaceLayout.DocumentsFolder));
        _fileSystem.Directory.CreateDirectory(_fileSystem.Path.Combine(projectRoot, WorkspaceLayout.AssetsFolder));
        _fileSystem.Directory.CreateDirectory(_fileSystem.Path.Combine(projectRoot, WorkspaceLayout.OutputsFolder));
        _fileSystem.Directory.CreateDirectory(_fileSystem.Path.Combine(projectRoot, WorkspaceLayout.CacheFolder));
        _fileSystem.Directory.CreateDirectory(_fileSystem.Path.Combine(projectRoot, WorkspaceLayout.TempFolder));

        var projectManifest = new ProjectManifest(
            projectId,
            name,
            kind,
            WorkspaceLayout.CurrentSchemaVersion,
            new Dictionary<string, string>());

        var manifestPath = _fileSystem.Path.Combine(projectRoot, WorkspaceLayout.ProjectManifestFile);
        await _fileSystem.File.WriteAllTextAsync(
            manifestPath,
            JsonSerializer.Serialize(projectManifest, _jsonOptions),
            cancellationToken).ConfigureAwait(false);

        var references = workspace.Manifest.Projects.ToList();
        references.Add(new ProjectReference(projectId, folderName, name, kind));
        var updated = workspace.Manifest with { Projects = references };
        WriteWorkspaceManifest(workspace.Root.FullName, updated);

        return new PhysicalProject(projectManifest, _fileSystem.DirectoryInfo.New(projectRoot));
    }

    private void CreateLayout(string workspaceRoot)
    {
        _fileSystem.Directory.CreateDirectory(WorkspaceLayout.NovolisPath(workspaceRoot));
        _fileSystem.Directory.CreateDirectory(WorkspaceLayout.ProjectsPath(workspaceRoot));
        _fileSystem.Directory.CreateDirectory(WorkspaceLayout.TimelinePath(workspaceRoot));

        var settingsPath = _fileSystem.Path.Combine(WorkspaceLayout.NovolisPath(workspaceRoot), WorkspaceLayout.SettingsFile);
        if (!_fileSystem.File.Exists(settingsPath))
            _fileSystem.File.WriteAllText(settingsPath, "{}");
    }

    private void WriteWorkspaceManifest(string workspaceRoot, WorkspaceManifest manifest)
    {
        var path = WorkspaceLayout.WorkspaceManifestPath(workspaceRoot);
        _fileSystem.File.WriteAllText(path, JsonSerializer.Serialize(manifest, _jsonOptions));
    }

    private IReadOnlyList<IProject> LoadProjects(string workspaceRoot, WorkspaceManifest manifest)
    {
        var projects = new List<IProject>();
        foreach (var reference in manifest.Projects)
        {
            var projectRoot = WorkspaceLayout.ProjectRoot(workspaceRoot, reference.FolderName);
            var manifestPath = _fileSystem.Path.Combine(projectRoot, WorkspaceLayout.ProjectManifestFile);
            if (!_fileSystem.File.Exists(manifestPath))
                continue;

            var projectManifest = JsonSerializer.Deserialize<ProjectManifest>(
                _fileSystem.File.ReadAllText(manifestPath), _jsonOptions);
            if (projectManifest is null)
                continue;

            projects.Add(new PhysicalProject(projectManifest, _fileSystem.DirectoryInfo.New(projectRoot)));
        }

        return projects;
    }

    private static string Slugify(string name)
    {
        var chars = name.Trim().ToLowerInvariant()
            .Select(c => char.IsLetterOrDigit(c) ? c : '-')
            .ToArray();
        var slug = new string(chars).Trim('-');
        while (slug.Contains("--", StringComparison.Ordinal))
            slug = slug.Replace("--", "-", StringComparison.Ordinal);
        return string.IsNullOrEmpty(slug) ? "project" : slug;
    }
}
