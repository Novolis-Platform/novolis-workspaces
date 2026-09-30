using System.Xml.Linq;

namespace Novolis.Workspaces.DotNet.Slnx;

/// <summary>One solution folder declared by an SLNX solution document.</summary>
public sealed record SlnxSolutionFolder(string Name);
