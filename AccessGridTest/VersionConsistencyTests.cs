namespace AccessGridTest;

using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;

[TestFixture]
public class VersionConsistencyTests
{
    [Test]
    public void ReadmeVersion_ShouldMatchCsprojVersion()
    {
        var repoRoot = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..", ".."));

        var csproj = File.ReadAllText(Path.Combine(repoRoot, "src", "AccessGrid.csproj"));
        var csprojMatch = Regex.Match(csproj, @"<Version>(.+?)</Version>");
        Assert.That(csprojMatch.Success, Is.True, "Could not find <Version> in csproj");

        var readme = File.ReadAllText(Path.Combine(repoRoot, "README.md"));
        var readmeMatch = Regex.Match(readme, @"-Version\s+(\S+)");
        Assert.That(readmeMatch.Success, Is.True, "Could not find version in README");

        Assert.That(readmeMatch.Groups[1].Value, Is.EqualTo(csprojMatch.Groups[1].Value),
            "README version does not match csproj version");
    }
}
