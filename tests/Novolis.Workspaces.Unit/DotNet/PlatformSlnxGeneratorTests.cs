using Novolis.Workspaces.DotNet.Slnx;
using TUnit.Core;

namespace Novolis.Workspaces.Unit.DotNet;

public sealed class PlatformSlnxGeneratorTests
{
    [Test]
    public async Task Generate_Merges_Repo_Slnx_And_Writes_Package_Map()
    {
        var root = Path.Combine(Path.GetTempPath(), "platform-slnx-" + Guid.NewGuid().ToString("N"));
        var repo = Path.Combine(root, "novolis-demo");
        var src = Path.Combine(repo, "src", "Novolis.Demo");
        Directory.CreateDirectory(src);
        Directory.CreateDirectory(Path.Combine(root, "novolis-governance", "build", "generated"));
        Directory.CreateDirectory(Path.Combine(root, "novolis-governance", "build", "libraryreference"));
        try
        {
            await File.WriteAllTextAsync(Path.Combine(src, "Novolis.Demo.csproj"), """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <IsPackable>true</IsPackable>
                    <PackageId>Novolis.Demo</PackageId>
                  </PropertyGroup>
                </Project>
                """);
            await File.WriteAllTextAsync(Path.Combine(repo, "Novolis.Demo.slnx"), """
                <Solution>
                  <Folder Name="/src/">
                    <Project Path="src/Novolis.Demo/Novolis.Demo.csproj" />
                  </Folder>
                </Solution>
                """);

            var result = PlatformSlnxGenerator.Generate(root);
            await Assert.That(File.Exists(result.OutputPath)).IsTrue();
            var slnx = await File.ReadAllTextAsync(result.OutputPath);
            await Assert.That(slnx).Contains("novolis-demo");
            await Assert.That(slnx).Contains("Novolis.Demo.csproj");
            await Assert.That(result.PackageToProjectCount).IsEqualTo(1);
            await Assert.That(File.Exists(result.PackageToProjectMap)).IsTrue();
            var map = await File.ReadAllTextAsync(result.PackageToProjectMap);
            await Assert.That(map).Contains("Novolis.Demo");
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
