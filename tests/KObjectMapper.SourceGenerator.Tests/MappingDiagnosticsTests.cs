using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using KObjectMapper.SourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Xunit;

namespace KObjectMapper.SourceGenerator.Tests;

public class MappingDiagnosticsTests
{
    private static (ImmutableArray<Diagnostic> Diagnostics, string[] Sources) RunGenerator(string source)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "TestAssembly",
            syntaxTrees: [syntaxTree],
            references:
            [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
            ],
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        MappingPlanGenerator generator = new();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics);

        GeneratorDriverRunResult runResult = driver.GetRunResult();
        string[] generatedSources = runResult.GeneratedTrees.Select(t => t.ToString()).ToArray();

        return (diagnostics, generatedSources);
    }

    [Fact]
    public void KOM001_IncludesSourcePropertyName_InMessage()
    {
        string source = """
            public class Source { public string ExtraField { get; set; } }
            public class Target { }
            public class Profile
            {
                public void Configure() { CreateMap<Source, Target>(); }
                static object CreateMap<TSource, TTarget>() => null!;
            }
            """;

        (ImmutableArray<Diagnostic> diagnostics, _) = RunGenerator(source);

        Diagnostic? kom001 = diagnostics.FirstOrDefault(d => d.Id == "KOM001");
        kom001.ShouldNotBeNull();
        kom001!.GetMessage(CultureInfo.InvariantCulture).ShouldContain("ExtraField");
    }

    [Fact]
    public void KOM001_HasWarning_Severity()
    {
        string source = """
            public class Source { public string ExtraField { get; set; } }
            public class Target { }
            public class Profile
            {
                public void Configure() { CreateMap<Source, Target>(); }
                static object CreateMap<TSource, TTarget>() => null!;
            }
            """;

        (ImmutableArray<Diagnostic> diagnostics, _) = RunGenerator(source);

        Diagnostic? kom001 = diagnostics.FirstOrDefault(d => d.Id == "KOM001");
        kom001.ShouldNotBeNull();
        kom001!.Severity.ShouldBe(DiagnosticSeverity.Warning);
    }

    [Fact]
    public void KOM002_IncludesMemberName_SourceType_AndTargetType_InMessage()
    {
        string source = """
            public class Source { public int Age { get; set; } }
            public class Target { public string Age { get; set; } }
            public class Profile
            {
                public void Configure() { CreateMap<Source, Target>(); }
                static object CreateMap<TSource, TTarget>() => null!;
            }
            """;

        (ImmutableArray<Diagnostic> diagnostics, _) = RunGenerator(source);

        Diagnostic? kom002 = diagnostics.FirstOrDefault(d => d.Id == "KOM002");
        kom002.ShouldNotBeNull();
        string message = kom002!.GetMessage(CultureInfo.InvariantCulture);
        message.ShouldContain("Age");
        message.ShouldContain("Int32");
        message.ShouldContain("String");
    }

    [Fact]
    public void KOM002_HasWarning_Severity()
    {
        string source = """
            public class Source { public int Age { get; set; } }
            public class Target { public string Age { get; set; } }
            public class Profile
            {
                public void Configure() { CreateMap<Source, Target>(); }
                static object CreateMap<TSource, TTarget>() => null!;
            }
            """;

        (ImmutableArray<Diagnostic> diagnostics, _) = RunGenerator(source);

        Diagnostic? kom002 = diagnostics.FirstOrDefault(d => d.Id == "KOM002");
        kom002.ShouldNotBeNull();
        kom002!.Severity.ShouldBe(DiagnosticSeverity.Warning);
    }

    [Fact]
    public void Diagnostics_AreDeterministic_ForSameInput()
    {
        string source = """
            public class Source { public string ExtraField { get; set; } }
            public class Target { }
            public class Profile
            {
                public void Configure() { CreateMap<Source, Target>(); }
                static object CreateMap<TSource, TTarget>() => null!;
            }
            """;

        (ImmutableArray<Diagnostic> first, _) = RunGenerator(source);
        (ImmutableArray<Diagnostic> second, _) = RunGenerator(source);

        ImmutableArray<Diagnostic> firstKom001 = first.Where(d => d.Id == "KOM001").ToImmutableArray();
        ImmutableArray<Diagnostic> secondKom001 = second.Where(d => d.Id == "KOM001").ToImmutableArray();

        firstKom001.Length.ShouldBe(secondKom001.Length);
        for (int i = 0; i < firstKom001.Length; i++)
        {
            firstKom001[i].GetMessage(CultureInfo.InvariantCulture).ShouldBe(secondKom001[i].GetMessage(CultureInfo.InvariantCulture));
        }
    }
}
