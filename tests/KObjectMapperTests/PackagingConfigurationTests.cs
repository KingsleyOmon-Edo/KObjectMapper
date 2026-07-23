using System.Xml.Linq;
using Shouldly;

namespace KObjectMapperTests;

public class PackagingConfigurationTests
{
    [Fact]
    public void KObjectMapperProject_PackagingConfiguration_IsDeterministicAndNotGeneratedOnBuild()
    {
        string projectPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "src", "KObjectMapper", "KObjectMapper.csproj"));
        XDocument projectFile = XDocument.Load(projectPath);

        XElement? generatePackageOnBuild = projectFile.Descendants("GeneratePackageOnBuild").SingleOrDefault();
        XElement? versionPrefix = projectFile.Descendants("VersionPrefix").SingleOrDefault();
        IEnumerable<XElement> versionSuffixes = projectFile.Descendants("VersionSuffix");

        generatePackageOnBuild.ShouldNotBeNull();
        versionPrefix.ShouldNotBeNull();
        versionSuffixes.ShouldNotBeEmpty();

        generatePackageOnBuild!.Value.ShouldBe("False");
        versionPrefix!.Value.ShouldMatch("^\\d+\\.\\d+\\.\\d+$");

        foreach (XElement versionSuffix in versionSuffixes)
        {
            versionSuffix.Value.ShouldNotBeNullOrWhiteSpace();
            versionSuffix.Value.ShouldNotContain("$(");
        }

        File.ReadAllText(projectPath).ShouldNotContain("DateTime::Now");
    }
}
