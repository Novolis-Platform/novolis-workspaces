using System.Security.Cryptography;
using System.Text;
using Novolis.Workspaces.DotNet.MSBuild;
using Novolis.Workspaces.DotNet.Roslyn;
using Novolis.Workspaces.DotNet.Slnx;

namespace Novolis.Workspaces.DotNet.Indexing;

/// <summary>A cataloged project together with its evaluated and semantic projections.</summary>
public sealed record SolutionCatalogProject(
    SlnxProjectEntry Project,
    EvaluatedProject Evaluation,
    SemanticProject Semantic);

/// <summary>Portable provenance for a catalog snapshot.</summary>
public sealed record SolutionCatalogProvenance(
    string WorkspaceRootPath,
    string SolutionPath,
    DateTimeOffset CreatedAt);

/// <summary>Immutable, portable facts derived from one solution workspace snapshot.</summary>
public sealed record SolutionCatalog(
    string SnapshotId,
    SolutionCatalogProvenance Provenance,
    EvaluationContext Context,
    IReadOnlyList<SolutionCatalogProject> Projects,
    IReadOnlyList<WorkspaceDiagnostic> Diagnostics);

/// <summary>Creates deterministic solution catalog snapshots from SLNX, MSBuild, and Roslyn projections.</summary>
public sealed class SolutionCatalogBuilder
{
    private readonly SlnxSolutionReader _slnxReader;
    private readonly MsBuildProjectEvaluator _evaluator;
    private readonly RoslynSemanticProjectLoader _semanticLoader;

    public SolutionCatalogBuilder(
        SlnxSolutionReader? slnxReader = null,
        MsBuildProjectEvaluator? evaluator = null,
        RoslynSemanticProjectLoader? semanticLoader = null)
    {
        _slnxReader = slnxReader ?? new SlnxSolutionReader();
        _evaluator = evaluator ?? new MsBuildProjectEvaluator();
        _semanticLoader = semanticLoader ?? new RoslynSemanticProjectLoader();
    }

    public async ValueTask<SolutionCatalog> BuildAsync(
        SolutionWorkspace workspace,
        EvaluationContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(context);

        var topology = await _slnxReader.ReadAsync(workspace, cancellationToken).ConfigureAwait(false);
        var diagnostics = topology.Diagnostics.ToList();
        var projects = new List<SolutionCatalogProject>();

        foreach (var project in topology.Projects)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var evaluation = await _evaluator.EvaluateAsync(project.FullPath, context, cancellationToken)
                .ConfigureAwait(false);
            diagnostics.AddRange(evaluation.Diagnostics);

            var semantic = context.AllowDesignTimeBuilds
                ? await _semanticLoader.LoadAsync(project.FullPath, context, cancellationToken).ConfigureAwait(false)
                : new SemanticProject(project.FullPath, [], [], []);
            diagnostics.AddRange(semantic.Diagnostics);
            projects.Add(new SolutionCatalogProject(project, evaluation, semantic));
        }

        return new SolutionCatalog(
            CreateSnapshotId(workspace, context, projects),
            new SolutionCatalogProvenance(
                workspace.Root.FullName,
                workspace.SolutionFile.FullPath,
                DateTimeOffset.UtcNow),
            context,
            projects,
            diagnostics);
    }

    private static string CreateSnapshotId(
        SolutionWorkspace workspace,
        EvaluationContext context,
        IReadOnlyList<SolutionCatalogProject> projects)
    {
        var builder = new StringBuilder();
        builder.Append(workspace.SolutionFile.FullPath);
        builder.Append('|');
        builder.Append(context.Configuration);
        builder.Append('|');
        builder.Append(context.Platform);
        builder.Append('|');
        builder.Append(context.TargetFramework);

        foreach (var project in projects.OrderBy(project => project.Project.FullPath, StringComparer.OrdinalIgnoreCase))
        {
            builder.Append('|');
            builder.Append(project.Project.FullPath);
            builder.Append('|');
            builder.Append(File.Exists(project.Project.FullPath)
                ? File.GetLastWriteTimeUtc(project.Project.FullPath).Ticks
                : 0);
            foreach (var import in project.Evaluation.Imports.OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
            {
                builder.Append('|');
                builder.Append(import);
                builder.Append('|');
                builder.Append(File.Exists(import) ? File.GetLastWriteTimeUtc(import).Ticks : 0);
            }
        }

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString()))).ToLowerInvariant();
    }
}
