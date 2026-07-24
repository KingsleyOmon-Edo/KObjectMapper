using KObjectMapper.Abstractions;
using KObjectMapper.Collections;

namespace KObjectMapper.UnitTests.Collections;

public class CollectionMappingStrategyTests
{
    private sealed class Source
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private sealed class Target
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private static IObjectMapper BuildMapper()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper();
        return services.BuildServiceProvider().GetRequiredService<IObjectMapper>();
    }

    // ── Replace mode ──────────────────────────────────────────────────────────

    [Fact]
    public void Map_WithReplaceMode_ReplacesTargetCollection()
    {
        IObjectMapper mapper = BuildMapper();
        List<Source> source = [new() { Id = 1, Name = "A" }, new() { Id = 2, Name = "B" }, new() { Id = 3, Name = "C" }];
        List<Target> target = [new() { Id = 9, Name = "X" }, new() { Id = 8, Name = "Y" }];

        var options = new CollectionMappingOptions<Source, Target>().WithMergeMode(CollectionMergeMode.Replace);
        IEnumerable<Target> result = mapper.Map(source, target, options);

        result.Count().ShouldBe(3);
        result.ElementAt(0).Name.ShouldBe("A");
        result.ElementAt(1).Name.ShouldBe("B");
        result.ElementAt(2).Name.ShouldBe("C");
    }

    [Fact]
    public void Map_WithReplaceMode_TrimsExcessTargetItems()
    {
        IObjectMapper mapper = BuildMapper();
        List<Source> source = [new() { Id = 1, Name = "A" }, new() { Id = 2, Name = "B" }];
        List<Target> target = [new() { Id = 9, Name = "X" }, new() { Id = 8, Name = "Y" }, new() { Id = 7, Name = "Z" }];

        var options = new CollectionMappingOptions<Source, Target>().WithMergeMode(CollectionMergeMode.Replace);
        IEnumerable<Target> result = mapper.Map(source, target, options);

        result.Count().ShouldBe(2);
    }

    [Fact]
    public void Map_WithReplaceMode_IsDefaultBehavior()
    {
        IObjectMapper mapper = BuildMapper();
        List<Source> source = [new() { Id = 1, Name = "A" }, new() { Id = 2, Name = "B" }];
        List<Target> target1 = [new() { Id = 9, Name = "X" }];
        List<Target> target2 = [new() { Id = 9, Name = "X" }];

        var options = new CollectionMappingOptions<Source, Target>().WithMergeMode(CollectionMergeMode.Replace);
        IEnumerable<Target> withOptions = mapper.Map(source, target1, options);
        IEnumerable<Target> withoutOptions = mapper.Map<Source, Target>(source, target2);

        withOptions.Count().ShouldBe(withoutOptions.Count());
        withOptions.Select(t => t.Name).ShouldBe(withoutOptions.Select(t => t.Name));
    }

    // ── Append mode ───────────────────────────────────────────────────────────

    [Fact]
    public void Map_WithAppendMode_AppendsSourceItemsToTarget()
    {
        IObjectMapper mapper = BuildMapper();
        List<Source> source = [new() { Id = 3, Name = "C" }, new() { Id = 4, Name = "D" }];
        List<Target> target = [new() { Id = 1, Name = "A" }, new() { Id = 2, Name = "B" }];

        var options = new CollectionMappingOptions<Source, Target>().WithMergeMode(CollectionMergeMode.Append);
        IEnumerable<Target> result = mapper.Map(source, target, options);

        result.Count().ShouldBe(4);
    }

    [Fact]
    public void Map_WithAppendMode_WhenTargetIsEmpty_ReturnsSourceItems()
    {
        IObjectMapper mapper = BuildMapper();
        List<Source> source = [new() { Id = 1, Name = "A" }, new() { Id = 2, Name = "B" }];
        List<Target> target = [];

        var options = new CollectionMappingOptions<Source, Target>().WithMergeMode(CollectionMergeMode.Append);
        IEnumerable<Target> result = mapper.Map(source, target, options);

        result.Count().ShouldBe(2);
        result.ElementAt(0).Name.ShouldBe("A");
        result.ElementAt(1).Name.ShouldBe("B");
    }

    [Fact]
    public void Map_WithAppendMode_WhenSourceIsEmpty_ReturnsTargetItems()
    {
        IObjectMapper mapper = BuildMapper();
        List<Source> source = [];
        List<Target> target = [new() { Id = 1, Name = "A" }, new() { Id = 2, Name = "B" }];

        var options = new CollectionMappingOptions<Source, Target>().WithMergeMode(CollectionMergeMode.Append);
        IEnumerable<Target> result = mapper.Map(source, target, options);

        result.Count().ShouldBe(2);
        result.ElementAt(0).Name.ShouldBe("A");
        result.ElementAt(1).Name.ShouldBe("B");
    }

    // ── MergeByKey mode ───────────────────────────────────────────────────────

    [Fact]
    public void Map_WithMergeByKeyMode_UpdatesMatchedItems()
    {
        IObjectMapper mapper = BuildMapper();
        List<Source> source = [new() { Id = 1, Name = "Updated" }];
        List<Target> target = [new() { Id = 1, Name = "Original" }];

        var options = new CollectionMappingOptions<Source, Target>()
            .WithMergeMode(CollectionMergeMode.MergeByKey)
            .WithKeySelector(s => s.Id, t => t.Id);
        IEnumerable<Target> result = mapper.Map(source, target, options);

        result.Count().ShouldBe(1);
        result.First().Name.ShouldBe("Updated");
    }

    [Fact]
    public void Map_WithMergeByKeyMode_AddsNewItems()
    {
        IObjectMapper mapper = BuildMapper();
        List<Source> source = [new() { Id = 1, Name = "A" }, new() { Id = 99, Name = "New" }];
        List<Target> target = [new() { Id = 1, Name = "A" }];

        var options = new CollectionMappingOptions<Source, Target>()
            .WithMergeMode(CollectionMergeMode.MergeByKey)
            .WithKeySelector(s => s.Id, t => t.Id);
        IEnumerable<Target> result = mapper.Map(source, target, options);

        result.Count().ShouldBe(2);
        result.Any(t => t.Id == 99 && t.Name == "New").ShouldBeTrue();
    }

    [Fact]
    public void Map_WithMergeByKeyMode_RemovesUnmatchedTargetItems()
    {
        IObjectMapper mapper = BuildMapper();
        List<Source> source = [new() { Id = 1, Name = "A" }];
        List<Target> target = [new() { Id = 1, Name = "A" }, new() { Id = 5, Name = "Remove Me" }];

        var options = new CollectionMappingOptions<Source, Target>()
            .WithMergeMode(CollectionMergeMode.MergeByKey)
            .WithKeySelector(s => s.Id, t => t.Id);
        IEnumerable<Target> result = mapper.Map(source, target, options);

        result.Count().ShouldBe(1);
        result.Any(t => t.Id == 5).ShouldBeFalse();
    }

    [Fact]
    public void Map_WithMergeByKeyMode_ThrowsWhenNoKeySelector()
    {
        IObjectMapper mapper = BuildMapper();
        List<Source> source = [new() { Id = 1, Name = "A" }];
        List<Target> target = [new() { Id = 1, Name = "A" }];

        var options = new CollectionMappingOptions<Source, Target>()
            .WithMergeMode(CollectionMergeMode.MergeByKey);

        Should.Throw<InvalidOperationException>(() => mapper.Map(source, target, options).ToList());
    }

    [Fact]
    public void Map_WithMergeByKeyMode_WorksWithCustomKeySelector()
    {
        IObjectMapper mapper = BuildMapper();
        List<Source> source = [new() { Id = 1, Name = "Match" }];
        List<Target> target = [new() { Id = 99, Name = "Match" }];

        var options = new CollectionMappingOptions<Source, Target>()
            .WithMergeMode(CollectionMergeMode.MergeByKey)
            .WithKeySelector(s => s.Name, t => t.Name);
        IEnumerable<Target> result = mapper.Map(source, target, options);

        result.Count().ShouldBe(1);
        result.First().Id.ShouldBe(1);
    }

    // ── Read-only target tests ─────────────────────────────────────────────────

    [Fact]
    public void Map_WithAppendMode_WhenTargetIsReadOnly_ReturnsNewList()
    {
        IObjectMapper mapper = BuildMapper();
        List<Source> source = [new() { Id = 3, Name = "C" }];
        IEnumerable<Target> target = new[] { new Target { Id = 1, Name = "A" }, new Target { Id = 2, Name = "B" } };

        var options = new CollectionMappingOptions<Source, Target>().WithMergeMode(CollectionMergeMode.Append);
        IEnumerable<Target> result = mapper.Map(source, target, options);

        result.Count().ShouldBe(3);
    }

    [Fact]
    public void Map_WithReplaceMode_WhenTargetIsReadOnly_ReturnsNewList()
    {
        IObjectMapper mapper = BuildMapper();
        List<Source> source = [new() { Id = 1, Name = "A" }, new() { Id = 2, Name = "B" }];
        IEnumerable<Target> target = new[] { new Target { Id = 9, Name = "X" } };

        var options = new CollectionMappingOptions<Source, Target>().WithMergeMode(CollectionMergeMode.Replace);
        IEnumerable<Target> result = mapper.Map(source, target, options);

        result.Count().ShouldBe(2);
        result.ElementAt(0).Name.ShouldBe("A");
    }

    // ── Edge cases ────────────────────────────────────────────────────────────

    [Fact]
    public void Map_WithMergeByKeyMode_WhenSourceIsEmpty_RemovesAllTargetItems()
    {
        IObjectMapper mapper = BuildMapper();
        List<Source> source = [];
        List<Target> target = [new() { Id = 1, Name = "A" }, new() { Id = 2, Name = "B" }];

        var options = new CollectionMappingOptions<Source, Target>()
            .WithMergeMode(CollectionMergeMode.MergeByKey)
            .WithKeySelector(s => s.Id, t => t.Id);
        IEnumerable<Target> result = mapper.Map(source, target, options);

        result.ShouldBeEmpty();
    }

    [Fact]
    public void Map_WithAppendMode_PreservesTargetItemValues()
    {
        IObjectMapper mapper = BuildMapper();
        List<Source> source = [new() { Id = 3, Name = "C" }];
        List<Target> target = [new() { Id = 1, Name = "A" }, new() { Id = 2, Name = "B" }];

        var options = new CollectionMappingOptions<Source, Target>().WithMergeMode(CollectionMergeMode.Append);
        IEnumerable<Target> result = mapper.Map(source, target, options);

        result.ElementAt(0).Id.ShouldBe(1);
        result.ElementAt(0).Name.ShouldBe("A");
        result.ElementAt(1).Id.ShouldBe(2);
        result.ElementAt(1).Name.ShouldBe("B");
    }
}
