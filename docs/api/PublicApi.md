# KObjectMapper Public API Baseline

> **Version:** 0.1.0-alpha  
> **Last updated:** 2026-07-24  
> This document is the manually curated baseline of the public API surface for the `KObjectMapper` library.  
> Any change to a type or member listed here must be reviewed for breaking-change impact and reflected in `CHANGELOG.md`.

---

## Namespace: `KObjectMapper`

### `class Mapper : IObjectMapper`

**Constructors**
```csharp
public Mapper()
public Mapper(IEnumerable<MappingProfile> profiles)
```

**Methods**
```csharp
public static Mapper Create()

// Map (mutates existing target)
public void Map(object source, object target)
public void Map<TSource, TTarget>(TSource source, TTarget target)
public void Map(object source, object target, GraphMappingOptions options)
public void Map<TSource, TTarget>(TSource source, TTarget target, GraphMappingOptions options)

// Map (creates new target)
public TTarget Map<TSource, TTarget>(TSource source)
public IEnumerable<TTarget> Map<TSource, TTarget>(IEnumerable<TSource> sources)

// MapTo / MapFrom aliases
public void MapTo(object source, object target)
public void MapTo<TSource, TTarget>(TSource source, TTarget target)
public void MapFrom(object source, object target)
public void MapFrom<TSource, TTarget>(TSource source, TTarget target)

// Collection merge
public List<TTarget> Map<TSource, TTarget>(
    IList<TSource> sourceList,
    IList<TTarget> targetList,
    CollectionMappingOptions<TSource, TTarget> options)
    where TTarget : new()

// Safe mapping (no-throw)
public MappingResult TryMap(object source, object target)
public MappingResult TryMap<TSource, TTarget>(TSource source, TTarget target)

// Queryable projections
public IQueryable<TTarget> ProjectTo<TSource, TTarget>(IQueryable<TSource> source)
    where TTarget : new()
public Expression<Func<TSource, TTarget>> GetProjectionExpression<TSource, TTarget>()
    where TTarget : new()
```

---

### `class AsyncMapper : IAsyncObjectMapper`

**Constructors**
```csharp
public AsyncMapper(IObjectMapper mapper, IEnumerable<(Type, Type, object)> converters)
```

**Methods**
```csharp
public void RegisterAsyncConverter<TSource, TTarget>(IAsyncTypeConverter<TSource, TTarget> converter)

public Task MapAsync(object source, object target, CancellationToken cancellationToken = default)
public Task MapAsync<TSource, TTarget>(TSource source, TTarget target, CancellationToken cancellationToken = default)

public Task<MappingResult> TryMapAsync(object source, object target, CancellationToken cancellationToken = default)
public Task<MappingResult> TryMapAsync<TSource, TTarget>(TSource source, TTarget target, CancellationToken cancellationToken = default)
```

---

## Namespace: `KObjectMapper.Abstractions`

### `interface IObjectMapper`
```csharp
public interface IObjectMapper
{
    void Map(object source, object target);
    void Map<TSource, TTarget>(TSource source, TTarget target);
    TTarget Map<TSource, TTarget>(TSource source);
    IEnumerable<TTarget> Map<TSource, TTarget>(IEnumerable<TSource> sources);
    MappingResult TryMap(object source, object target);
    MappingResult TryMap<TSource, TTarget>(TSource source, TTarget target);
    IQueryable<TTarget> ProjectTo<TSource, TTarget>(IQueryable<TSource> source) where TTarget : new();
    Expression<Func<TSource, TTarget>> GetProjectionExpression<TSource, TTarget>() where TTarget : new();
}
```

### `interface IAsyncObjectMapper`
```csharp
public interface IAsyncObjectMapper
{
    Task MapAsync(object source, object target, CancellationToken cancellationToken = default);
    Task MapAsync<TSource, TTarget>(TSource source, TTarget target, CancellationToken cancellationToken = default);
    Task<MappingResult> TryMapAsync(object source, object target, CancellationToken cancellationToken = default);
    Task<MappingResult> TryMapAsync<TSource, TTarget>(TSource source, TTarget target, CancellationToken cancellationToken = default);
}
```

### `interface ITypeConverter<TSource, TTarget>`
```csharp
public interface ITypeConverter<TSource, TTarget>
{
    TTarget Convert(TSource source);
}
```

### `interface IAsyncTypeConverter<TSource, TTarget>`
```csharp
public interface IAsyncTypeConverter<TSource, TTarget>
{
    Task<TTarget> ConvertAsync(TSource source, CancellationToken cancellationToken = default);
}
```

### `interface IEnumConverter<TSource, TEnum> where TEnum : struct, Enum`
```csharp
public interface IEnumConverter<TSource, TEnum> where TEnum : struct, Enum
{
    EnumConversionResult<TEnum> Convert(TSource source);
    ITypeConverter<TSource, TEnum> AsTypeConverter();
}
```

### `interface IQueryableMapper`
```csharp
public interface IQueryableMapper
{
    IQueryable<TTarget> ProjectTo<TSource, TTarget>(IQueryable<TSource> source) where TTarget : new();
    Expression<Func<TSource, TTarget>> GetProjectionExpression<TSource, TTarget>() where TTarget : new();
}
```

### `sealed class EnumConversionResult<TEnum> where TEnum : struct, Enum`
```csharp
public bool IsSuccess { get; }
public TEnum Value { get; }
public string? Error { get; }
```

### `sealed class MappingResult`
```csharp
public bool IsSuccess { get; }
public IReadOnlyList<MappingError> Errors { get; }

public static MappingResult Success()
public static MappingResult Failure(IReadOnlyList<MappingError> errors)
public static MappingResult Failure(MappingError error)
```

### `sealed class MappingError`
```csharp
public string MemberPath { get; init; }
public Type? SourceType { get; init; }
public Type? TargetType { get; init; }
public string Reason { get; init; }
```

---

## Namespace: `KObjectMapper.Collections`

### `enum CollectionMergeMode`
```csharp
public enum CollectionMergeMode
{
    Replace,
    MergeByKey,
    Append
}
```

### `sealed class CollectionMappingOptions<TSource, TTarget>`
```csharp
public CollectionMergeMode MergeMode { get; }
public Func<TSource, object>? SourceKeySelector { get; }
public Func<TTarget, object>? TargetKeySelector { get; }

public CollectionMappingOptions<TSource, TTarget> WithMergeMode(CollectionMergeMode mode)
public CollectionMappingOptions<TSource, TTarget> WithKeySelector(
    Func<TSource, object> sourceKeySelector,
    Func<TTarget, object> targetKeySelector)
```

---

## Namespace: `KObjectMapper.Configuration`

### `class MappingProfile`
```csharp
public abstract class MappingProfile
{
    public IReadOnlyList<MappingTypeMap> TypeMaps { get; }
    protected MappingTypeMapConfiguration<TSource, TTarget> CreateMap<TSource, TTarget>() where TTarget : new()
}
```

### `class MappingProfileOptions`
```csharp
public IReadOnlyList<MappingProfile> Profiles { get; }
public NullMappingPolicy? GlobalNullPolicy { get; }
public bool IsStrictMode { get; }
public bool UseSourceGenerationGlobally { get; }

public MappingProfileOptions AddProfile<TProfile>() where TProfile : MappingProfile, new()
public MappingProfileOptions AddProfile(MappingProfile profile)
public MappingProfileOptions AddProfilesFromAssembly(Assembly assembly)
public MappingProfileOptions WithGlobalNullPolicy(NullMappingPolicy policy)
public MappingProfileOptions WithStrictMode()
public MappingProfileOptions AddConverter<TSource, TTarget>(ITypeConverter<TSource, TTarget> converter)
public MappingProfileOptions AddAsyncConverter<TSource, TTarget>(IAsyncTypeConverter<TSource, TTarget> converter)
public MappingProfileOptions WithSourceGenerationGlobally()
public MappingProfileOptions OnMappingError(Action<MappingError> handler)
public MappingProfileOptions OnMappingCompleted(Action<MappingResult> handler)
```

### `sealed class MappingTypeMapConfiguration<TSource, TTarget>`
```csharp
public MappingTypeMapConfiguration<TSource, TTarget> ForMember<TMember>(
    Expression<Func<TTarget, TMember>> targetMember,
    Expression<Func<TSource, TMember>> sourceMember)

public MappingTypeMapConfiguration<TSource, TTarget> Ignore<TMember>(
    Expression<Func<TTarget, TMember>> targetMember)

public MappingTypeMapConfiguration<TSource, TTarget> WithNullPolicy(NullMappingPolicy policy)

public MappingTypeMapConfiguration<TSource, TTarget> SubstituteNullWith<TMember>(
    Expression<Func<TTarget, TMember>> targetMember,
    TMember substituteValue)

public MappingTypeMapConfiguration<TSource, TTarget> AddConverter<TMember>(
    Expression<Func<TTarget, TMember>> targetMember,
    ITypeConverter<object, TMember> converter)

public MappingTypeMapConfiguration<TSource, TTarget> UseSourceGeneration()
```

### `enum NullMappingPolicy`
```csharp
public enum NullMappingPolicy
{
    Propagate,
    Ignore
}
```

### `class GraphMappingOptions`
```csharp
public bool PreserveReferences { get; }
public int MaxDepth { get; }

public GraphMappingOptions WithPreserveReferences()
public GraphMappingOptions WithMaxDepth(int maxDepth)
```

### `sealed class MappingProfileValidationException : InvalidOperationException`
```csharp
public MappingProfileValidationException(IReadOnlyCollection<string> errors)
public IReadOnlyCollection<string> Errors { get; }
```

---

## Namespace: `KObjectMapper.Converters`

### `static class TypeConverters`
```csharp
public static ITypeConverter<string, int>     StringToInt32 { get; }
public static ITypeConverter<string, long>    StringToInt64 { get; }
public static ITypeConverter<string, double>  StringToDouble { get; }
public static ITypeConverter<string, decimal> StringToDecimal { get; }
public static ITypeConverter<string, bool>    StringToBool { get; }
public static ITypeConverter<string, Guid>    StringToGuid { get; }
public static ITypeConverter<string, DateTime> StringToDateTime { get; }
public static ITypeConverter<string, DateTimeOffset> StringToDateTimeOffset { get; }
public static ITypeConverter<string, TimeSpan> StringToTimeSpan { get; }
```

### `static class EnumConverter`
```csharp
public static IEnumConverter<string, TEnum> FromString<TEnum>() where TEnum : struct, Enum
public static IEnumConverter<string, TEnum> FromString<TEnum>(bool ignoreCase) where TEnum : struct, Enum
public static IEnumConverter<int, TEnum>    FromInt32<TEnum>() where TEnum : struct, Enum
```

---

## Namespace: `KObjectMapper.DependencyInjection`

### `static class ServiceCollectionExtensions`
```csharp
public static IServiceCollection AddKObjectMapper(this IServiceCollection services)
public static IServiceCollection AddKObjectMapper(
    this IServiceCollection services,
    Action<MappingProfileOptions> configure)
```

---

## Namespace: `KObjectMapper.Extensions`

### `static class MapperExtensions`

Extension methods available on all objects:

```csharp
// Non-generic
public static object MapTo(this object source, object target)
public static object MapFrom(this object target, object source)

// Generic overloads
public static TTarget MapTo<TTarget>(this object source, object target)
public static object  MapFrom<TSource>(this object target, object source)

// Collection overloads
public static IEnumerable<TTarget> MapTo<TSource, TTarget>(this IEnumerable<TSource> sources)
    where TTarget : new()
public static IEnumerable<TTarget> MapFrom<TSource, TTarget>(this IEnumerable<TTarget> targets, IEnumerable<TSource> sources)
    where TTarget : new()
```

---

## Namespace: `KObjectMapper.Projections`

### `sealed class ProjectionException : Exception`
```csharp
public ProjectionException(string message)
public ProjectionException(string message, Exception innerException)
```

---

## Breaking Change Policy

A change is considered **breaking** if it:

- Removes or renames a public type, method, property, or enum value
- Changes a method signature (parameter types, return type, generic constraints)
- Changes the accessibility of a public member to non-public
- Adds a required parameter to an existing public method
- Changes the semantics of an existing method in a way that alters observable behavior

Non-breaking changes include:
- Adding new public types or members
- Adding optional parameters with defaults
- Adding new enum values (unless exhaustive switch is expected)
- Internal/private implementation changes

Any breaking change requires a **major version bump** per the semantic versioning policy documented in `CHANGELOG.md`.
