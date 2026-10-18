using System.Collections;
using System.Reflection;

namespace Novolis.Workspaces.DotNet.MSBuild;

internal static class MsBuildProjectEvaluationCore
{
    public static EvaluatedProject Evaluate(
        string fullPath,
        EvaluationContext context,
        ICollection<WorkspaceDiagnostic> diagnostics)
    {
        MsBuildHostRegistration.EnsureRegistered();

        var projectCollectionType = Type.GetType("Microsoft.Build.Evaluation.ProjectCollection, Microsoft.Build")
            ?? throw new InvalidOperationException("MSBuild evaluation assemblies are not available.");
        var constructor = projectCollectionType.GetConstructor(new[] { typeof(IDictionary<string, string>) })
            ?? throw new InvalidOperationException("The MSBuild ProjectCollection constructor is unavailable.");
        using var collection = (IDisposable)constructor.Invoke(new object[]
        {
            context.ToGlobalProperties()
                .ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.OrdinalIgnoreCase),
        });

        var project = Invoke(collection, "LoadProject", fullPath);
        var properties = Enumerate(Read(project, "AllEvaluatedProperties"))
            .Select(property => new
            {
                Name = StringValue(Read(property, "Name")),
                Value = StringValue(Read(property, "EvaluatedValue")),
            })
            .Where(property => property.Name is not null)
            .GroupBy(property => property.Name!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Last().Value ?? string.Empty, StringComparer.OrdinalIgnoreCase);
        var items = Enumerate(Read(project, "AllEvaluatedItems"))
            .Select(item => new EvaluatedProjectItem(
                StringValue(Read(item, "ItemType")) ?? string.Empty,
                StringValue(Read(item, "EvaluatedInclude")) ?? string.Empty,
                Enumerate(Read(item, "Metadata"))
                    .Select(metadata => new
                    {
                        Name = StringValue(Read(metadata, "Name")),
                        Value = StringValue(Read(metadata, "EvaluatedValue")),
                    })
                    .Where(metadata => metadata.Name is not null)
                    .ToDictionary(metadata => metadata.Name!, metadata => metadata.Value ?? string.Empty, StringComparer.OrdinalIgnoreCase),
                Read(Read(item, "Xml"), "ContainingProject") is { } containingProject
                    ? StringValue(Read(containingProject, "FullPath"))
                    : null))
            .ToArray();
        var imports = Enumerate(Read(project, "Imports"))
            .Select(import => StringValue(Read(Read(import, "ImportedProject"), "FullPath")))
            .Where(path => path is not null)
            .Select(path => path!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new EvaluatedProject(fullPath, context, properties, items, imports, diagnostics.ToArray());
    }

    private static object Invoke(object instance, string methodName, params object[] arguments)
    {
        var method = instance.GetType().GetMethod(methodName, arguments.Select(argument => argument.GetType()).ToArray())
            ?? throw new InvalidOperationException($"MSBuild member '{methodName}' is unavailable.");
        return method.Invoke(instance, arguments)
            ?? throw new InvalidOperationException($"MSBuild member '{methodName}' returned no value.");
    }

    private static object? Read(object? instance, string propertyName) =>
        instance?.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)?.GetValue(instance);

    private static IEnumerable<object> Enumerate(object? value) =>
        value is IEnumerable sequence
            ? sequence.Cast<object>()
            : Enumerable.Empty<object>();

    private static string? StringValue(object? value) => value as string;
}
