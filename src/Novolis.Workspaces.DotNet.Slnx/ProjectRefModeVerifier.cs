using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Novolis.Workspaces.DotNet.Slnx;

/// <summary>Verifies the PackageToProject map and LibraryReference copy drift.</summary>
public static class ProjectRefModeVerifier
{
    private static readonly Regex ExcludeRepo = new(
        @"workflows|governance|lab|utilities|apps|installer|experimental|smoketest|template-dotnet",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static IReadOnlyList<string> Verify(string workspaceRoot, bool regenerateMap = true)
    {
        workspaceRoot = Path.GetFullPath(workspaceRoot);
        var failures = new List<string>();
        var govRoot = Path.Combine(workspaceRoot, "novolis-governance");
        var mapPath = Path.Combine(govRoot, "build", "generated", "Novolis.PackageToProject.props");
        if (regenerateMap)
            PackageToProjectMapWriter.Write(workspaceRoot, mapPath);

        var lrSource = Path.Combine(workspaceRoot, "novolis-msbuild", "src", "Novolis.MSBuild.LibraryReference", "build");
        var lrCopy = Path.Combine(govRoot, "build", "libraryreference");
        foreach (var name in new[] { "Novolis.MSBuild.LibraryReference.props", "Novolis.MSBuild.LibraryReference.targets" })
        {
            var src = Path.Combine(lrSource, name);
            var dst = Path.Combine(lrCopy, name);
            if (!File.Exists(src) || !File.Exists(dst))
            {
                failures.Add($"LibraryReference file missing: {src} or {dst}");
                continue;
            }

            if (!CryptographicOperations.FixedTimeEquals(SHA256.HashData(File.ReadAllBytes(src)), SHA256.HashData(File.ReadAllBytes(dst))))
                failures.Add($"LibraryReference drift: {name} in novolis-msbuild does not match novolis-governance/build/libraryreference.");
        }

        if (!File.Exists(Path.Combine(govRoot, "build", "Novolis.LibraryReference.targets")))
            failures.Add("Missing novolis-governance/build/Novolis.LibraryReference.targets");

        if (!File.Exists(mapPath))
        {
            failures.Add($"Map file missing: {mapPath}");
            return failures;
        }

        var mapEntries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (Match m in Regex.Matches(
                     File.ReadAllText(mapPath),
                     @"<NovolisPackageProject Include=""([^""]+)"">\s*<ProjectPath>\$\(NovolisLibraryRoot\)([^<]+)</ProjectPath>"))
        {
            var id = m.Groups[1].Value;
            var rel = m.Groups[2].Value.Trim().TrimStart('\\', '/').Replace('/', '\\');
            if (!mapEntries.TryAdd(id, rel))
                failures.Add($"Duplicate map PackageId: {id}");
        }

        var packable = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var repo in Directory.GetDirectories(workspaceRoot, "novolis-*"))
        {
            if (ExcludeRepo.IsMatch(Path.GetFileName(repo)))
                continue;
            foreach (var csproj in Directory.EnumerateFiles(repo, "*.csproj", SearchOption.AllDirectories))
            {
                if (csproj.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)
                    || csproj.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
                    continue;
                var text = File.ReadAllText(csproj);
                if (!Regex.IsMatch(text, @"<IsPackable>\s*true\s*</IsPackable>", RegexOptions.IgnoreCase))
                    continue;
                var idMatch = Regex.Match(text, @"<PackageId>\s*([^<]+?)\s*</PackageId>");
                var packageId = idMatch.Success ? idMatch.Groups[1].Value.Trim() : Path.GetFileNameWithoutExtension(csproj);
                var rel = Path.GetRelativePath(workspaceRoot, csproj).Replace('/', '\\');
                if (!packable.TryAdd(packageId, rel))
                    failures.Add($"Duplicate packable PackageId in tree: {packageId}");
            }
        }

        foreach (var kv in packable)
        {
            if (!mapEntries.TryGetValue(kv.Key, out var mapped))
            {
                failures.Add($"Packable missing from map: {kv.Key} ({kv.Value})");
                continue;
            }

            if (!string.Equals(mapped, kv.Value, StringComparison.OrdinalIgnoreCase))
                failures.Add($"Map path mismatch for {kv.Key}: map='{mapped}' actual='{kv.Value}'");
        }

        foreach (var kv in mapEntries)
        {
            if (!packable.ContainsKey(kv.Key))
                failures.Add($"Map entry not packable / stale: {kv.Key} -> {kv.Value}");
            if (!File.Exists(Path.Combine(workspaceRoot, kv.Value)))
                failures.Add($"Map path does not exist: {kv.Key} -> {kv.Value}");
        }

        return failures;
    }
}
