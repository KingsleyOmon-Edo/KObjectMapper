# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [0.1.0-alpha] - 2026-07-24

### Added

#### Core Mapping (US-01, US-02)
- `Mapper` class with `Map`, `MapTo`, and `MapFrom` methods for property-based object mapping
- Generic and non-generic overloads for all mapping methods
- `Mapper.Create()` static factory method
- `IObjectMapper` interface defining the core mapping contract

#### Async Mapping (US-03)
- `AsyncMapper` class implementing `IAsyncObjectMapper`
- `MapAsync` and `TryMapAsync` overloads with `CancellationToken` support
- `RegisterAsyncConverter<TSource, TTarget>` for runtime async converter registration

#### Mapping Profiles & Configuration (US-04, US-05)
- `MappingProfile` abstract base class for defining typed mapping configurations
- `MappingTypeMapConfiguration<TSource, TTarget>` fluent API with:
  - `ForMember` — custom member mapping via expressions
  - `Ignore` — exclude target members from mapping
  - `WithNullPolicy` — per-map null handling
  - `SubstituteNullWith` — provide fallback values for null sources
  - `AddConverter` — attach per-member type converters
- `MappingProfileOptions` for global configuration (strict mode, null policy, converters, source generation)
- `MappingProfileValidationException` with structured `Errors` collection

#### Null Mapping Policy (US-06)
- `NullMappingPolicy` enum (`Propagate`, `Ignore`)
- Global and per-map null policy support in `MappingProfileOptions` and `MappingTypeMapConfiguration`

#### Type Converters (US-07)
- `ITypeConverter<TSource, TTarget>` interface
- `IAsyncTypeConverter<TSource, TTarget>` interface
- `TypeConverters` static class with built-in converters: `StringToInt32`, `StringToInt64`, `StringToDouble`, `StringToDecimal`, `StringToBool`, `StringToGuid`, `StringToDateTime`, `StringToDateTimeOffset`, `StringToTimeSpan`

#### Enum Conversion (US-08)
- `IEnumConverter<TSource, TEnum>` interface with safe, no-throw conversion
- `EnumConversionResult<TEnum>` result type with `IsSuccess`, `Value`, and `Error`
- `EnumConverter` static class: `FromString<TEnum>()`, `FromString<TEnum>(bool ignoreCase)`, `FromInt32<TEnum>()`
- `AsTypeConverter()` wrapper that throws `InvalidOperationException` on failure

#### Collection Mapping (US-09)
- `CollectionMergeMode` enum (`Replace`, `MergeByKey`, `Append`)
- `CollectionMappingOptions<TSource, TTarget>` with `WithMergeMode` and `WithKeySelector`
- `Mapper.Map<TSource, TTarget>(IList<TSource>, IList<TTarget>, CollectionMappingOptions)` overload

#### Dependency Injection (US-10)
- `ServiceCollectionExtensions.AddKObjectMapper()` extension for `IServiceCollection`
- Profile validation at DI registration time (throws `MappingProfileValidationException` on invalid configuration)
- Registers both `IObjectMapper` (`Mapper`) and `IAsyncObjectMapper` (`AsyncMapper`) as singletons

#### Queryable Projections (US-11)
- `IQueryableMapper` interface
- `Mapper.ProjectTo<TSource, TTarget>` and `Mapper.GetProjectionExpression<TSource, TTarget>`
- `ProjectionException` for projection-specific errors

#### Extension Methods (US-12)
- `MapperExtensions` static class providing `MapTo` and `MapFrom` as extension methods on `object`
- Collection extension overloads: `MapTo<TSource, TTarget>` and `MapFrom<TSource, TTarget>`

#### Graph Mapping (US-13)
- `GraphMappingOptions` with `WithPreserveReferences()` and `WithMaxDepth(int)`
- `Mapper.Map(object, object, GraphMappingOptions)` and generic overload for circular-reference-safe deep mapping

#### Safe Mapping / Result Pattern (US-14)
- `MappingResult` with `IsSuccess`, `Errors`, `Success()`, and `Failure()` factory methods
- `MappingError` record with `MemberPath`, `SourceType`, `TargetType`, `Reason`
- `Mapper.TryMap` and `AsyncMapper.TryMapAsync` no-throw variants
- `OnMappingError` and `OnMappingCompleted` hooks in `MappingProfileOptions`

#### Source Generation (US-15)
- `MappingTypeMapConfiguration.UseSourceGeneration()` per-map opt-in
- `MappingProfileOptions.WithSourceGenerationGlobally()` global opt-in
- `IGeneratedMapperRegistry` and `GeneratedMapperRegistry` for source-generated mapper dispatch
- Roslyn source generator (`KObjectMapper.SourceGenerator`) emitting compile-time mapping plans

#### Assembly Scanning (US-16)
- `MappingProfileOptions.AddProfilesFromAssembly(Assembly)` for automatic profile discovery

#### Public API Stabilization (US-17)
- `docs/api/PublicApi.md` — manually curated public API baseline
- `CHANGELOG.md` — version history, upgrade guidance, and semantic versioning policy
- `.github/workflows/api-compat.yml` — CI workflow for API compatibility checks on pull requests

### Breaking Changes

_None. This is the initial alpha release._

### Deprecated

_Nothing deprecated in this release._

---

## Upgrade Guide

### Upgrading to 0.1.0-alpha (initial release)

This is the first public release. No migration is required.

**Recommended setup with dependency injection:**

```csharp
services.AddKObjectMapper(options =>
{
    options
        .AddProfile<MyMappingProfile>()
        .WithGlobalNullPolicy(NullMappingPolicy.Ignore)
        .WithStrictMode();
});
```

**Defining a mapping profile:**

```csharp
public class MyMappingProfile : MappingProfile
{
    public MyMappingProfile()
    {
        CreateMap<Source, Target>()
            .ForMember(t => t.FullName, s => s.Name)
            .Ignore(t => t.InternalId)
            .WithNullPolicy(NullMappingPolicy.Ignore);
    }
}
```

**Using extension methods (no DI):**

```csharp
var target = new TargetDto();
source.MapTo(target);
// or
target.MapFrom(source);
```

---

## Semantic Versioning Policy

This project follows [Semantic Versioning 2.0.0](https://semver.org/):

| Version segment | When to bump |
|---|---|
| **MAJOR** (`X.0.0`) | Any breaking change to the public API (see `docs/api/PublicApi.md` for what constitutes a breaking change) |
| **MINOR** (`0.X.0`) | New backwards-compatible features or additions to the public API |
| **PATCH** (`0.0.X`) | Backwards-compatible bug fixes, documentation updates, internal refactoring |

### Pre-release labels

- `alpha` — early development; API may change without a major bump
- `beta` — feature-complete; API is stabilizing; breaking changes will be called out explicitly
- `rc` — release candidate; no planned breaking changes

### API Stability Guarantee

Once version `1.0.0` is released:
- All types and members listed in `docs/api/PublicApi.md` are considered stable
- Breaking changes will only be introduced in major versions
- Every breaking change will be documented in this changelog under a "Breaking Changes" section
- A migration guide will be provided for every major version bump

### Checking for Breaking Changes

Before merging a pull request that modifies the `KObjectMapper` library:

1. Review `docs/api/PublicApi.md` and compare it against your changes
2. If any public type, method, property, or enum value was removed, renamed, or had its signature changed, it is a **breaking change**
3. Update `docs/api/PublicApi.md` to reflect the new API surface
4. Bump the version accordingly and document the change in this file

The CI workflow (`.github/workflows/api-compat.yml`) will remind reviewers to perform this check on every pull request targeting `main`.

[0.1.0-alpha]: https://github.com/your-org/KObjectMapper/releases/tag/v0.1.0-alpha
