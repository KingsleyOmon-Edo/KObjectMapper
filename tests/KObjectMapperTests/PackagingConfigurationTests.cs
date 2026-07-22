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
        XElement? versionSuffix = projectFile.Descendants("VersionSuffix").SingleOrDefault();

        generatePackageOnBuild.ShouldNotBeNull();
        versionPrefix.ShouldNotBeNull();
        versionSuffix.ShouldNotBeNull();

        generatePackageOnBuild!.Value.ShouldBe("False");
        versionPrefix!.Value.ShouldBe("0.0.0");
        versionSuffix!.Value.ShouldBe("local");
        File.ReadAllText(projectPath).ShouldNotContain("DateTime::Now");
    }
}
