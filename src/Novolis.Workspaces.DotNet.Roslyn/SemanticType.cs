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
