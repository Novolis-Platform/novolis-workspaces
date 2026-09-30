using System.IO.Abstractions;
using RootWorkspace = Novolis.IO.Workspace.IWorkspace;

namespace Novolis.Workspaces.DotNet;

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
