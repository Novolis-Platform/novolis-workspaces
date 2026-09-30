using System.Xml.Linq;

namespace Novolis.Workspaces.DotNet.Slnx;

/// <summary>One project entry declared by an SLNX solution document.</summary>
public sealed record SlnxProjectEntry(string RelativePath, string FullPath, string? Name);
