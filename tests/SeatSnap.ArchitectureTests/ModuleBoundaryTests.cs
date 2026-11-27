using System.Xml.Linq;

namespace SeatSnap.ArchitectureTests;

public class ModuleBoundaryTests
{
    [Fact]
    public void Modules_must_not_reference_other_modules_implementations()
    {
        var modulesDir = Path.Combine(FindRepoRoot(), "src", "Modules");
        var violations = new List<string>();

        foreach (var csproj in Directory.EnumerateFiles(modulesDir, "*.csproj", SearchOption.AllDirectories))
        {
            var self = Path.GetFileNameWithoutExtension(csproj);

            var referenced = XDocument.Load(csproj)
                .Descendants("ProjectReference")
                .Select(r => Path.GetFileNameWithoutExtension(((string)r.Attribute("Include")!).Replace('\\', '/')));

            foreach (var reference in referenced)
            {
                var isModule = reference.StartsWith("SeatSnap.Modules.", StringComparison.Ordinal);
                var isContracts = reference.EndsWith(".Contracts", StringComparison.Ordinal);

                if (isModule && !isContracts && reference != self)
                {
                    violations.Add($"{self} -> {reference}");
                }
            }
        }

        Assert.Empty(violations);
    }

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && dir.GetFiles("SeatSnap.sln*").Length == 0)
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("Repository root not found.");
    }
}