// Copyright (c) KObjectMapper contributors. All rights reserved.
using KObjectMapper.Abstractions;
using KObjectMapper.Configuration;

namespace KObjectMapper.UnitTests.Graph;

public class NestedGraphMappingTests
{
    // ── helpers ──────────────────────────────────────────────────────────────

    private static IObjectMapper BuildMapper()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper(_ => { });
        return services.BuildServiceProvider().GetRequiredService<IObjectMapper>();
    }

    // ── model types ──────────────────────────────────────────────────────────

    private class Level5 { public string? Value { get; set; } }
    private class Level4 { public string? Value { get; set; } public Level5? Child { get; set; } }
    private class Level3 { public string? Value { get; set; } public Level4? Child { get; set; } }
    private class Level2 { public string? Value { get; set; } public Level3? Child { get; set; } }
    private class Level1 { public string? Value { get; set; } public Level2? Child { get; set; } }

    private class NodeA { public string? Name { get; set; } public NodeB? B { get; set; } }
    private class NodeB { public string? Name { get; set; } public NodeA? A { get; set; } }

    private class SelfRef { public string? Name { get; set; } public SelfRef? Next { get; set; } }

    private class SharedChild { public string? Value { get; set; } }
    private class SharedParent { public SharedChild? First { get; set; } public SharedChild? Second { get; set; } }

    private class EmptyObj { }

    // ── deep nesting ─────────────────────────────────────────────────────────

    [Fact]
    public void Map_WithGraphOptions_MapsDeepNestedObjects()
    {
        IObjectMapper mapper = BuildMapper();

        Level1 source = new()
        {
            Value = "L1",
            Child = new Level2
            {
                Value = "L2",
                Child = new Level3
                {
                    Value = "L3",
                    Child = new Level4
                    {
                        Value = "L4",
                        Child = new Level5 { Value = "L5" }
                    }
                }
            }
        };

        Level1 target = new();
        GraphMappingOptions options = new GraphMappingOptions().WithMaxDepth(10);

        mapper.Map(source, target, options);

        target.Value.ShouldBe("L1");
        target.Child.ShouldNotBeNull();
        target.Child!.Value.ShouldBe("L2");
        target.Child.Child.ShouldNotBeNull();
        target.Child.Child!.Value.ShouldBe("L3");
        target.Child.Child.Child.ShouldNotBeNull();
        target.Child.Child.Child!.Value.ShouldBe("L4");
        target.Child.Child.Child.Child.ShouldNotBeNull();
        target.Child.Child.Child.Child!.Value.ShouldBe("L5");
    }

    [Fact]
    public void Map_WithGraphOptions_MapsNestedObjectProperties()
    {
        IObjectMapper mapper = BuildMapper();

        Level1 source = new() { Value = "root", Child = new Level2 { Value = "child" } };
        Level1 target = new() { Value = "old", Child = new Level2 { Value = "old-child" } };

        mapper.Map(source, target, new GraphMappingOptions());

        target.Value.ShouldBe("root");
        target.Child!.Value.ShouldBe("child");
    }

    [Fact]
    public void Map_WithoutGraphOptions_MapsNestedObjects()
    {
        IObjectMapper mapper = BuildMapper();

        Level1 source = new() { Value = "root", Child = new Level2 { Value = "child" } };
        Level1 target = new();

        mapper.Map(source, target);

        target.Value.ShouldBe("root");
        target.Child.ShouldNotBeNull();
        target.Child!.Value.ShouldBe("child");
    }

    // ── circular reference detection ─────────────────────────────────────────

    [Fact]
    public void Map_WithGraphOptions_DetectsCircularReference_DoesNotStackOverflow()
    {
        IObjectMapper mapper = BuildMapper();

        NodeA a = new() { Name = "A" };
        NodeB b = new() { Name = "B", A = a };
        a.B = b;

        NodeA targetA = new();

        Should.NotThrow(() => mapper.Map(a, targetA, new GraphMappingOptions()));
        targetA.Name.ShouldBe("A");
    }

    [Fact]
    public void Map_WithGraphOptions_SelfReferentialObject_DoesNotStackOverflow()
    {
        IObjectMapper mapper = BuildMapper();

        SelfRef self = new() { Name = "me" };
        self.Next = self;

        SelfRef target = new();

        Should.NotThrow(() => mapper.Map(self, target, new GraphMappingOptions()));
        target.Name.ShouldBe("me");
    }

    // ── reference preservation ───────────────────────────────────────────────

    [Fact]
    public void Map_WithReferencePreservation_ReusesMappedInstances()
    {
        IObjectMapper mapper = BuildMapper();

        SharedChild sharedChild = new() { Value = "shared" };
        SharedParent source = new() { First = sharedChild, Second = sharedChild };
        SharedParent target = new();

        mapper.Map(source, target, new GraphMappingOptions().WithReferencePreservation());

        target.First.ShouldNotBeNull();
        target.Second.ShouldNotBeNull();
        target.First.ShouldBeSameAs(target.Second);
    }

    [Fact]
    public void Map_WithReferencePreservation_Option_Exists()
    {
        GraphMappingOptions options = new GraphMappingOptions().WithReferencePreservation();
        options.PreserveReferences.ShouldBeTrue();
    }

    // ── MaxDepth ─────────────────────────────────────────────────────────────

    [Fact]
    public void Map_WithGraphOptions_MaxDepth_ThrowsWhenExceeded()
    {
        IObjectMapper mapper = BuildMapper();

        Level1 source = new()
        {
            Value = "L1",
            Child = new Level2
            {
                Value = "L2",
                Child = new Level3 { Value = "L3" }
            }
        };

        Level1 target = new();
        GraphMappingOptions options = new GraphMappingOptions().WithMaxDepth(2);

        Should.Throw<InvalidOperationException>(() => mapper.Map(source, target, options));
    }

    [Fact]
    public void Map_WithGraphOptions_MaxDepth_SucceedsAtExactDepth()
    {
        IObjectMapper mapper = BuildMapper();

        Level1 source = new()
        {
            Value = "L1",
            Child = new Level2
            {
                Value = "L2",
                Child = new Level3 { Value = "L3" }
            }
        };

        Level1 target = new();
        GraphMappingOptions options = new GraphMappingOptions().WithMaxDepth(3);

        Should.NotThrow(() => mapper.Map(source, target, options));
        target.Child!.Child!.Value.ShouldBe("L3");
    }

    // ── edge cases ───────────────────────────────────────────────────────────

    [Fact]
    public void Map_WithGraphOptions_NullNestedProperty_DoesNotThrow()
    {
        IObjectMapper mapper = BuildMapper();

        Level1 source = new() { Value = "root", Child = null };
        Level1 target = new() { Value = "old" };

        Should.NotThrow(() => mapper.Map(source, target, new GraphMappingOptions()));
        target.Value.ShouldBe("root");
    }

    [Fact]
    public void Map_WithGraphOptions_EmptyObject_MapsSuccessfully()
    {
        IObjectMapper mapper = BuildMapper();

        EmptyObj source = new();
        EmptyObj target = new();

        Should.NotThrow(() => mapper.Map(source, target, new GraphMappingOptions()));
    }
}
