using System.Linq.Expressions;
using KObjectMapper.Abstractions;
using KObjectMapper.Projections;

namespace KObjectMapper.UnitTests.Projections;

public class QueryableProjectionTests
{
    private readonly IObjectMapper _mapper;

    public QueryableProjectionTests()
    {
        ServiceCollection services = new();
        services.AddKObjectMapper();
        _mapper = services.BuildServiceProvider().GetRequiredService<IObjectMapper>();
    }

    // --- Happy path tests ---

    [Fact]
    public void GetProjectionExpression_ReturnsExpression_ForMatchingProperties()
    {
        Expression<Func<SourceA, TargetA>> expr = _mapper.GetProjectionExpression<SourceA, TargetA>();
        expr.ShouldNotBeNull();
    }

    [Fact]
    public void ProjectTo_ReturnsProjectedQueryable_ForSimpleTypes()
    {
        IQueryable<SourceA> source = new List<SourceA>
        {
            new() { Id = 1, Name = "Alice" }
        }.AsQueryable();

        IQueryable<TargetA> result = _mapper.ProjectTo<SourceA, TargetA>(source);
        List<TargetA> list = result.ToList();

        list.Count.ShouldBe(1);
        list[0].Id.ShouldBe(1);
        list[0].Name.ShouldBe("Alice");
    }

    [Fact]
    public void ProjectTo_MapsAllMatchingProperties()
    {
        IQueryable<SourceA> source = new List<SourceA>
        {
            new() { Id = 42, Name = "Bob" }
        }.AsQueryable();

        List<TargetA> result = _mapper.ProjectTo<SourceA, TargetA>(source).ToList();

        result[0].Id.ShouldBe(42);
        result[0].Name.ShouldBe("Bob");
    }

    [Fact]
    public void ProjectTo_IgnoresNonMatchingProperties()
    {
        IQueryable<SourceWithExtra> source = new List<SourceWithExtra>
        {
            new() { Id = 1, Name = "Carol", Extra = "ignored" }
        }.AsQueryable();

        Should.NotThrow(() => _mapper.ProjectTo<SourceWithExtra, TargetA>(source).ToList());
    }

    [Fact]
    public void GetProjectionExpression_IsCached_ForSameTypePair()
    {
        Expression<Func<SourceA, TargetA>> expr1 = _mapper.GetProjectionExpression<SourceA, TargetA>();
        Expression<Func<SourceA, TargetA>> expr2 = _mapper.GetProjectionExpression<SourceA, TargetA>();

        ReferenceEquals(expr1, expr2).ShouldBeTrue();
    }

    // --- EF Core translation-friendly tests ---

    [Fact]
    public void ProjectTo_WorksWithLinqSelect_OnInMemoryQueryable()
    {
        IQueryable<SourceA> source = new List<SourceA>
        {
            new() { Id = 5, Name = "Dave" }
        }.AsQueryable();

        Expression<Func<SourceA, TargetA>> expr = _mapper.GetProjectionExpression<SourceA, TargetA>();
        List<TargetA> result = source.Select(expr).ToList();

        result[0].Id.ShouldBe(5);
        result[0].Name.ShouldBe("Dave");
    }

    [Fact]
    public void ProjectTo_ExpressionIsComposable()
    {
        IQueryable<SourceA> source = new List<SourceA>
        {
            new() { Id = 1, Name = "Eve" },
            new() { Id = 2, Name = "Frank" }
        }.AsQueryable();

        List<TargetA> result = _mapper.ProjectTo<SourceA, TargetA>(source)
            .Where(t => t.Id == 2)
            .ToList();

        result.Count.ShouldBe(1);
        result[0].Name.ShouldBe("Frank");
    }

    // --- Fallback/error tests ---

    [Fact]
    public void GetProjectionExpression_ThrowsProjectionException_WhenNoMappableProperties()
    {
        Should.Throw<ProjectionException>(() =>
            _mapper.GetProjectionExpression<SourceNoMatch, TargetNoMatch>());
    }

    [Fact]
    public void ProjectTo_ThrowsProjectionException_WhenSourceIsNull()
    {
        Should.Throw<ProjectionException>(() =>
            _mapper.ProjectTo<SourceA, TargetA>(null!));
    }

    // --- Edge cases ---

    [Fact]
    public void ProjectTo_HandlesEmptySourceCollection()
    {
        IQueryable<SourceA> source = new List<SourceA>().AsQueryable();
        List<TargetA> result = _mapper.ProjectTo<SourceA, TargetA>(source).ToList();
        result.ShouldBeEmpty();
    }

    [Fact]
    public void ProjectTo_HandlesNullableProperties()
    {
        IQueryable<SourceNullable> source = new List<SourceNullable>
        {
            new() { Id = null, Name = null }
        }.AsQueryable();

        List<TargetNullable> result = _mapper.ProjectTo<SourceNullable, TargetNullable>(source).ToList();

        result[0].Id.ShouldBeNull();
        result[0].Name.ShouldBeNull();
    }

    [Fact]
    public void GetProjectionExpression_WorksForMultipleTypePairs()
    {
        Expression<Func<SourceA, TargetA>> exprA = _mapper.GetProjectionExpression<SourceA, TargetA>();
        Expression<Func<SourceB, TargetB>> exprB = _mapper.GetProjectionExpression<SourceB, TargetB>();

        exprA.ShouldNotBeNull();
        exprB.ShouldNotBeNull();
        ReferenceEquals(exprA, exprB).ShouldBeFalse();
    }

    // --- Test models ---

    private sealed class SourceA
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private sealed class TargetA
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private sealed class SourceWithExtra
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Extra { get; set; } = string.Empty;
    }

    private sealed class SourceNoMatch
    {
        public string Foo { get; set; } = string.Empty;
    }

    private sealed class TargetNoMatch
    {
        public string Bar { get; set; } = string.Empty;
    }

    private sealed class SourceNullable
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
    }

    private sealed class TargetNullable
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
    }

    private sealed class SourceB
    {
        public int Code { get; set; }
        public string Label { get; set; } = string.Empty;
    }

    private sealed class TargetB
    {
        public int Code { get; set; }
        public string Label { get; set; } = string.Empty;
    }
}
