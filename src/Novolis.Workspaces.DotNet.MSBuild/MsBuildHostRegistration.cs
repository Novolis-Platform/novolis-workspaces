using Microsoft.Build.Locator;

namespace Novolis.Workspaces.DotNet.MSBuild;

/// <summary>Registers one process-wide MSBuild toolset before evaluation or Roslyn loading.</summary>
public static class MsBuildHostRegistration
{
    private static readonly object Gate = new();

    public static void EnsureRegistered()
    {
        lock (Gate)
        {
            if (!MSBuildLocator.IsRegistered)
                MSBuildLocator.RegisterDefaults();
        }
    }
}
