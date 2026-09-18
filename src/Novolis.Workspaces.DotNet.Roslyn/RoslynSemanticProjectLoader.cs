using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Novolis.Workspaces.DotNet.MSBuild;

namespace Novolis.Workspaces.DotNet.Roslyn;

/// <summary>A portable semantic description of one declared C# type.</summary>
public sealed record SemanticType(
    string MetadataName,
    string Namespace,
    string Kind,
    bool IsPublic,
    string? SourcePath);

/// <summary>A portable Roslyn projection for one evaluated project.</summary>
public sealed record SemanticProject(
    string ProjectPath,
    IReadOnlyList<string> Documents,
    IReadOnlyList<SemanticType> Types,
    IReadOnlyList<WorkspaceDiagnostic> Diagnostics);

/// <summary>Loads an MSBuild project through Roslyn and emits portable semantic facts.</summary>
public sealed class RoslynSemanticProjectLoader
{
    public async ValueTask<SemanticProject> LoadAsync(
        string projectPath,
        EvaluationContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPath);
        ArgumentNullException.ThrowIfNull(context);

        var fullPath = Path.GetFullPath(projectPath);
        var diagnostics = new List<WorkspaceDiagnostic>();
        if (!context.AllowDesignTimeBuilds)
        {
            diagnostics.Add(new WorkspaceDiagnostic(
                "NWS3000",
                "Roslyn project loading requires EvaluationContext.AllowDesignTimeBuilds.",
                WorkspaceDiagnosticSeverity.Warning,
                fullPath));
            return new SemanticProject(fullPath, [], [], diagnostics);
        }

        try
        {
            MsBuildHostRegistration.EnsureRegistered();
            using var workspace = MSBuildWorkspace.Create(context.ToGlobalProperties()
                .ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.OrdinalIgnoreCase));
            using var registration = workspace.RegisterWorkspaceFailedHandler(args => diagnostics.Add(new WorkspaceDiagnostic(
                "NWS3001",
                args.Diagnostic.Message,
                args.Diagnostic.Kind == WorkspaceDiagnosticKind.Failure
                    ? WorkspaceDiagnosticSeverity.Error
                    : WorkspaceDiagnosticSeverity.Warning,
                fullPath)));

            var project = await workspace.OpenProjectAsync(fullPath, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            var compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
            if (compilation is null)
            {
                diagnostics.Add(new WorkspaceDiagnostic(
                    "NWS3002",
                    "Roslyn did not produce a compilation for the project.",
                    WorkspaceDiagnosticSeverity.Error,
                    fullPath));
                return new SemanticProject(fullPath, [], [], diagnostics);
            }

            var documents = project.Documents
                .Select(document => document.FilePath ?? document.Name)
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            var types = EnumerateTypes(compilation.Assembly.GlobalNamespace)
                .OrderBy(type => type.MetadataName, StringComparer.Ordinal)
                .ToArray();

            return new SemanticProject(fullPath, documents, types, diagnostics);
        }
        catch (Exception exception) when (exception is IOException
                                          or UnauthorizedAccessException
                                          or InvalidOperationException
                                          or ArgumentException)
        {
            diagnostics.Add(new WorkspaceDiagnostic(
                "NWS3003",
                exception.Message,
                WorkspaceDiagnosticSeverity.Error,
                fullPath));
            return new SemanticProject(fullPath, [], [], diagnostics);
        }
    }

    private static IEnumerable<SemanticType> EnumerateTypes(INamespaceSymbol namespaceSymbol)
    {
        foreach (var childNamespace in namespaceSymbol.GetNamespaceMembers())
        {
            foreach (var type in EnumerateTypes(childNamespace))
                yield return type;
        }

        foreach (var type in namespaceSymbol.GetTypeMembers())
        {
            yield return new SemanticType(
                type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                type.ContainingNamespace.ToDisplayString(),
                type.TypeKind.ToString(),
                type.DeclaredAccessibility == Accessibility.Public,
                type.Locations.FirstOrDefault(location => location.IsInSource)?.SourceTree?.FilePath);
        }
    }
}
