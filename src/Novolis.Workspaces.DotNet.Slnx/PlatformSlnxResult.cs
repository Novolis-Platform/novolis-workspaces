using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Novolis.Workspaces.DotNet.Slnx;

/// <summary>Result of generating the platform SLNX and package map.</summary>
public sealed record PlatformSlnxResult(
    string OutputPath,
    string PackageToProjectMap,
    int PackageToProjectCount,
    int RepositoriesFound,
    int RepositoriesIncluded,
    int TotalProjects,
    IReadOnlyList<string> Warnings);
