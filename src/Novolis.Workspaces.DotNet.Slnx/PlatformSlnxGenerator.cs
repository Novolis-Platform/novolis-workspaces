using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Novolis.Workspaces.DotNet.Slnx;

/// <summary>Merges per-repo SLNX files into Novolis.Platform.slnx and the package map.</summary>
public static class PlatformSlnxGenerator
{
    public static readonly string[] DefaultExcludedRepos =
    [
        ".github",
        "novolis-experimental",
        "novolis-smoketest",
        "novolis-template-dotnet",
        "novolis-reach",
        "merglyph",
    ];

    public static PlatformSlnxResult Generate(
        string workspaceRoot,
        string? outputPath = null,
        IReadOnlyCollection<string>? excludeRepos = null,
        bool validateProjectReferences = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspaceRoot);
        workspaceRoot = Path.GetFullPath(workspaceRoot);
        outputPath ??= Path.Combine(workspaceRoot, "Novolis.Platform.slnx");
        var excluded = new HashSet<string>(excludeRepos ?? DefaultExcludedRepos, StringComparer.OrdinalIgnoreCase);

        var slnxFiles = Directory.GetFiles(workspaceRoot, "*.slnx", SearchOption.TopDirectoryOnly)
            .Select(f => new FileInfo(f))
            .Where(_ => false)
            .ToList();
        slnxFiles = Directory.GetDirectories(workspaceRoot, "novolis-*")
            .SelectMany(dir => Directory.GetFiles(dir, "*.slnx", SearchOption.TopDirectoryOnly))
            .Select(f => new FileInfo(f))
            .OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (slnxFiles.Count == 0)
            throw new InvalidOperationException("No .slnx files found in workspace");

        var repos = slnxFiles
            .GroupBy(f => f.DirectoryName!, StringComparer.OrdinalIgnoreCase)
            .Select(g => (Name: Path.GetFileName(g.Key), Directory: g.Key, Files: g.Select(x => x.FullName).ToArray()))
            .Where(r => r.Name.StartsWith("novolis-", StringComparison.OrdinalIgnoreCase) && !excluded.Contains(r.Name))
            .OrderBy(r => r.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var master = new XDocument(new XElement("Solution"));
        var folders = new Dictionary<string, XElement>(StringComparer.Ordinal);
        var warnings = new List<string>();
        var projectsIncluded = 0;

        foreach (var repo in repos)
        {
            foreach (var slnxPath in repo.Files)
            {
                var repoSlnx = XDocument.Load(slnxPath);
                foreach (var sourceFolder in repoSlnx.Root?.Elements().Where(e => e.Name.LocalName == "Folder") ?? [])
                {
                    var folderName = (string?)sourceFolder.Attribute("Name") ?? "";
                    var combined = $"/{repo.Name}{folderName}";
                    if (!folders.TryGetValue(combined, out var masterFolder))
                    {
                        masterFolder = new XElement("Folder", new XAttribute("Name", combined));
                        master.Root!.Add(masterFolder);
                        folders[combined] = masterFolder;
                    }

                    foreach (var sourceProject in sourceFolder.Elements().Where(e => e.Name.LocalName == "Project"))
                    {
                        var projectPath = ((string?)sourceProject.Attribute("Path") ?? "").Replace('\\', '/').TrimStart('/');
                        if (projectPath.Length == 0)
                            continue;
                        var solutionProjectPath = repo.Name + "\\" + projectPath.Replace('/', '\\');
                        if (masterFolder.Elements("Project").Any(p =>
                                string.Equals((string?)p.Attribute("Path"), solutionProjectPath, StringComparison.OrdinalIgnoreCase)))
                            continue;
                        if (Regex.IsMatch(solutionProjectPath, @"(?i)(^|[\\/])Android([\\/]|$)|\.Android\.csproj$"))
                            continue;
                        if (Regex.IsMatch(solutionProjectPath, @"(?i)Novolis\.Agent\.Unit\.csproj$"))
                            continue;
                        if (validateProjectReferences)
                        {
                            var full = Path.Combine(
                                workspaceRoot,
                                repo.Name,
                                projectPath.Replace('/', Path.DirectorySeparatorChar));
                            if (!File.Exists(full))
                            {
                                warnings.Add($"Missing project file: {solutionProjectPath}");
                                continue;
                            }
                        }

                        masterFolder.Add(new XElement("Project", new XAttribute("Path", solutionProjectPath)));
                        projectsIncluded++;
                    }
                }
            }
        }

        var xmlContent = Serialize(master);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        File.WriteAllText(outputPath, xmlContent, Encoding.Unicode);

        var governanceSlnx = Path.Combine(workspaceRoot, "novolis-governance", "build", "Novolis.Platform.slnx");
        var governanceContent = Regex.Replace(
            xmlContent,
            @"Project Path=""(?!\.\.[\\/]\.\.[\\/])([^""]+)""",
            @"Project Path=""..\..\$1""");
        File.WriteAllText(governanceSlnx, governanceContent, Encoding.Unicode);

        var mapPath = Path.Combine(workspaceRoot, "novolis-governance", "build", "generated", "Novolis.PackageToProject.props");
        var mapCount = PackageToProjectMapWriter.Write(workspaceRoot, mapPath);

        return new PlatformSlnxResult(
            outputPath,
            mapPath,
            mapCount,
            slnxFiles.Select(f => f.DirectoryName).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
            repos.Length,
            projectsIncluded,
            warnings);
    }

    private static string Serialize(XDocument document)
    {
        var settings = new System.Xml.XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  ",
            Encoding = Encoding.UTF8,
            OmitXmlDeclaration = false,
        };
        using var sw = new StringWriter();
        using var xw = System.Xml.XmlWriter.Create(sw, settings);
        document.Save(xw);
        xw.Flush();
        return sw.ToString();
    }
}
