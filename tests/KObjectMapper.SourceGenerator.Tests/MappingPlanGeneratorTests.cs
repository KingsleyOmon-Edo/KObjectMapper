using System.Collections.Immutable;
using System.Linq;
using KObjectMapper.SourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Xunit;

namespace KObjectMapper.SourceGenerator.Tests;

/// <summary>
/// TDD tests for MappingPlanGenerator (US-07).
/// Verifies that the incremental source generator produces correct compile-time
/// mapping implementations for registered source/target pairs.
/// </summary>
public class MappingPlanGeneratorTests
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
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out ImmutableArray<Diagnostic> diagnostics);

        GeneratorDriverRunResult runResult = driver.GetRunResult();
        string[] generatedSources = runResult.GeneratedTrees.Select(t => t.ToString()).ToArray();

        return (diagnostics, generatedSources);
    }

    // ── US-07 AC1: Incremental source generator produces mapping implementations ──

    [Fact]
    public void Generator_ProducesMapperClass_ForSimpleTypePair()
    {
        string source = """
            public class Source { public string Name { get; set; } = ""; public int Age { get; set; } }
            public class Target { public string Name { get; set; } = ""; public int Age { get; set; } }
            public class MyProfile
            {
                public void Configure() { CreateMap<Source, Target>(); }
                static object CreateMap<TSource, TTarget>() => null!;
            }
            """;

        (_, string[] sources) = RunGenerator(source);

        sources.ShouldNotBeEmpty();
        sources[0].ShouldContain("SourceToTargetMapper");
        sources[0].ShouldContain("target.Name = source.Name;");
        sources[0].ShouldContain("target.Age = source.Age;");
    }

    [Fact]
    public void Generator_ProducesNoOutput_WhenNoCreateMapCallPresent()
    {
        string source = """
            namespace MyApp;
            public class Source { public string Name { get; set; } = ""; }
            public class Target { public string Name { get; set; } = ""; }
            public class NotAProfile { }
            """;

        (_, string[] sources) = RunGenerator(source);

        sources.ShouldBeEmpty();
    }

    // ── US-07 AC2: Generated mappers compile without consumer-written partial methods ──

    [Fact]
    public void Generator_ProducesStaticClass_WithNoPartialMethodRequirement()
    {
        string source = """
            public class OrderSource { public string OrderId { get; set; } = ""; public decimal Total { get; set; } }
            public class OrderTarget { public string OrderId { get; set; } = ""; public decimal Total { get; set; } }
            public class OrderProfile
            {
                public void Configure() { CreateMap<OrderSource, OrderTarget>(); }
                static object CreateMap<TSource, TTarget>() => null!;
            }
            """;

        (_, string[] sources) = RunGenerator(source);

        sources.ShouldNotBeEmpty();
        sources[0].ShouldContain("public static class OrderSourceToOrderTargetMapper");
        sources[0].ShouldContain("public static void Map(");
        sources[0].ShouldNotContain("partial");
    }

    // ── US-07 AC3: KOM001/KOM002 diagnostics emitted for unsupported members ──

    [Fact]
    public void Generator_EmitsKOM001_ForUnmatchedSourceProperty()
    {
        string source = """
            public class Source { public string ExtraField { get; set; } = ""; }
            public class Target { }
            public class Profile
            {
                public void Configure() { CreateMap<Source, Target>(); }
                static object CreateMap<TSource, TTarget>() => null!;
            }
            """;

        (ImmutableArray<Diagnostic> diagnostics, _) = RunGenerator(source);

        diagnostics.ShouldContain(d => d.Id == "KOM001");
    }

    [Fact]
    public void Generator_ProducesMultipleMappers_ForMultipleCreateMapCalls()
    {
        string source = """
            public class A { public int X { get; set; } }
            public class B { public int X { get; set; } }
            public class C { public int X { get; set; } }
            public class D { public int X { get; set; } }
            public class MultiProfile
            {
                public void Configure()
                {
                    CreateMap<A, B>();
                    CreateMap<C, D>();
                }
                static object CreateMap<TSource, TTarget>() => null!;
            }
            """;

        (_, string[] sources) = RunGenerator(source);

        sources.Length.ShouldBe(2);
        sources.ShouldContain(s => s.Contains("AToB"));
        sources.ShouldContain(s => s.Contains("CToD"));
    }

    [Fact]
    public void Generator_EmitsKOM002_ForTypeMismatch()
    {
        string source = """
            public class Source { public int Age { get; set; } }
            public class Target { public string Age { get; set; } = ""; }
            public class Profile
            {
                public void Configure() { CreateMap<Source, Target>(); }
                static object CreateMap<TSource, TTarget>() => null!;
            }
            """;

        (ImmutableArray<Diagnostic> diagnostics, _) = RunGenerator(source);

        diagnostics.ShouldContain(d => d.Id == "KOM002");
    }
}
