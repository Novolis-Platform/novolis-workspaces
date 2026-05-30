namespace Novolis.Timeline.Presentation.GitGraph;

/// <summary>RGB triple for git-graph UI brushes.</summary>
public readonly record struct GraphRgb(byte R, byte G, byte B)
{
    public static GraphRgb FromArgb(byte r, byte g, byte b) => new(r, g, b);
}
