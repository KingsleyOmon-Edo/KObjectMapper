# KObjectMapper

KObjectMapper is a simple, intuitive, and effective open-source object-to-object mapping library for C# and .NET.

It supports two mapping modes — **Implicit** (extension methods, zero configuration) and **Explicit** (mapper instance) — and a rich **profile-based configuration** system for production-grade mapping pipelines.

**_Pre-release notice: KObjectMapper is currently published as an alpha package (`0.0.0-alpha-1`). The API is still evolving and may include breaking changes between alpha versions. Do not use in production workloads yet._**

## Installation

```bash
dotnet add package KObjectMapper --version 0.0.0-alpha-1
```

> For newer alpha drops, increment the suffix (for example: `0.0.0-alpha-2`, `0.0.0-alpha-3`).

---

## Quick Start

### Implicit mapping (extension methods)

Implicit mapping uses extension methods and requires no mapper instance or configuration.

```csharp
using KObjectMapper.Extensions;

var customer = new Customer { Id = 25, FirstName = "Will", LastName = "Smith" };
CustomerDto dto = new();

customer.MapTo(dto);       // customer → dto
dto.MapFrom(customer);     // equivalent reverse direction
```

### Explicit mapping (mapper instance)

```csharp
using KObjectMapper;

var mapper = Mapper.Create();

// Map into an existing instance
mapper.Map<Customer, CustomerDto>(customer, dto);

// Map to a new instance
CustomerDto dto = mapper.Map<Customer, CustomerDto>(customer);

// Map collections
IEnumerable<CustomerDto> dtos = mapper.Map<Customer, CustomerDto>(customers);
```

---

## Profile-Based Configuration

Profiles give you explicit, reusable, and versionable mapping rules per source/target pair.

### Defining a profile

```csharp
using KObjectMapper.Configuration;

public class CustomerProfile : MappingProfile
{
    protected override void Configure()
    {
        CreateMap<Customer, CustomerDto>()
            .ForMember(src => src.FullName, tgt => tgt.Name)   // rename
            .Ignore(tgt => tgt.InternalCode);                  // skip member
    }
}
```

### Registering profiles

```csharp
using KObjectMapper.DependencyInjection;

// Register individual profiles
builder.Services.AddKObjectMapper(options =>
{
    options.AddProfile<CustomerProfile>();
});

// Or scan an assembly for all profiles
builder.Services.AddKObjectMapper(options =>
{
    options.AddProfilesFromAssembly(typeof(CustomerProfile).Assembly);
});
```

Inject and use `IObjectMapper` anywhere:

```csharp
using KObjectMapper.Abstractions;

public class CustomersController(IObjectMapper mapper)
{
    public CustomerDto Get(Customer customer)
        => mapper.Map<Customer, CustomerDto>(customer);
}
```

---

## Null-Handling Policy

Control how null source values are handled — globally or per map.

```csharp
// Global policy
builder.Services.AddKObjectMapper(options =>
{
    options.WithNullPolicy(NullMappingPolicy.Ignore);
});

// Per-map policy with a null substitute
CreateMap<Order, OrderDto>()
    .WithNullPolicy(NullMappingPolicy.Ignore)
    .SubstituteNullWith(tgt => tgt.Status, "Unknown");
```

| Policy | Behaviour |
|--------|-----------|
| `Propagate` (default) | Null source value is written to the target |
| `Ignore` | Null source values are skipped; target retains its value |

---

## Strict Mode and Startup Validation

Enable strict mode to fail fast when no type map is registered for a requested pair.

```csharp
builder.Services.AddKObjectMapper(options =>
{
    options.EnableStrictMode();
    options.AddProfile<CustomerProfile>();
});
```

With strict mode on, calling `mapper.Map<A, B>(...)` for an unregistered pair throws `InvalidOperationException` at the call site rather than silently falling back to reflection-based mapping.

Startup validation also catches structural problems — such as target types with no accessible parameterless constructor — and surfaces them as a structured `MappingProfileValidationException` with an `Errors` collection before the application starts serving traffic.

---

## Type Converters

Register custom converters for complex domain transformations that go beyond `Convert.ChangeType`.

### Built-in converters

`TypeConverters` provides ready-made converters for common patterns:

```csharp
using KObjectMapper.Converters;

// string → int, long, double, decimal, bool, Guid, DateTime, DateTimeOffset
// string → enum (with optional case-insensitive parsing)
// int    → enum
TypeConverters.StringToInt32
TypeConverters.StringToGuid
TypeConverters.StringToDateTimeOffset
TypeConverters.StringToEnum<MyEnum>()
TypeConverters.Int32ToEnum<MyEnum>()
```

### Custom converters

Implement `ITypeConverter<TSource, TTarget>`:

```csharp
using KObjectMapper.Abstractions;

public class MoneyConverter : ITypeConverter<decimal, string>
{
    public string Convert(decimal source) => source.ToString("C2");
}
```

Register globally or per map:

```csharp
// Global — applies to all maps
options.AddConverter<decimal, string>(new MoneyConverter());

// Per map — takes precedence over global
CreateMap<Invoice, InvoiceDto>()
    .AddConverter<decimal, string>(new MoneyConverter());
```

---

## Enum Conversion Safety

Use `EnumConverter` for safe, validated enum conversions that surface failures explicitly instead of silently corrupting data.

```csharp
using KObjectMapper.Converters;

// String → enum (case-insensitive)
var converter = EnumConverter.FromString<Status>(ignoreCase: true);

// Int → enum
var converter = EnumConverter.FromInt32<Status>();

// Wrap as ITypeConverter and register
CreateMap<OrderDto, Order>()
    .AddConverter<string, Status>(
        EnumConverter.FromString<Status>(ignoreCase: true).AsTypeConverter());
```

`AsTypeConverter()` throws `InvalidOperationException` with a descriptive message when the source value cannot be mapped to a valid enum member. Use `EnumConverter` directly when you need the structured `EnumConversionResult<TEnum>` (with `IsSuccess`, `Value`, and `Error` properties) for non-throwing error handling.

---

## Structured Mapping Results

Use the non-throwing `TryMap` API to get a structured result instead of an exception on mapping failure.

```csharp
MappingResult result = mapper.TryMap(source, target);

if (!result.IsSuccess)
{
    foreach (MappingError error in result.Errors)
        Console.WriteLine($"{error.MemberPath}: {error.Reason}");
}
```

Each `MappingError` includes `MemberPath`, `SourceType`, `TargetType`, and `Reason`.

### Observability hooks

Register callbacks for logging and metrics without coupling your mapping code to a specific logging framework:

```csharp
builder.Services.AddKObjectMapper(options =>
{
    options.WithOnMappingError(err => logger.LogError("Mapping failed: {Reason}", err.Reason));
    options.WithOnMappingCompleted(result => metrics.Record(result));
});
```

---

## Collection Mapping Strategies

The collection `Map` overload supports three merge modes via `CollectionMappingOptions`.

```csharp
using KObjectMapper.Collections;

// Replace (default) — replaces target collection with source items
mapper.Map<Source, Target>(sourceList, targetList);

// Append — adds source items after existing target items
var options = new CollectionMappingOptions<Source, Target>()
    .WithMergeMode(CollectionMergeMode.Append);
mapper.Map(sourceList, targetList, options);

// MergeByKey — add/update/remove by key selector
var options = new CollectionMappingOptions<Source, Target>()
    .WithMergeMode(CollectionMergeMode.MergeByKey)
    .WithKeySelector(src => src.Id, tgt => tgt.Id);
mapper.Map(sourceList, targetList, options);
```

| Mode | Behaviour |
|------|-----------|
| `Replace` | Target collection is replaced by source items (default) |
| `Append` | Source items are appended after existing target items |
| `MergeByKey` | Matched items are updated; unmatched target items are removed; new source items are added |

`MergeByKey` throws `InvalidOperationException` when key selectors are not configured.

---

## Queryable Projection

Project `IQueryable<TSource>` to `IQueryable<TTarget>` using EF Core translation-friendly `Expression<Func<TSource, TTarget>>` expressions built via `Expression.MemberInit`.

```csharp
using KObjectMapper.Abstractions;

// Project a queryable (e.g. EF Core DbSet)
IQueryable<CustomerDto> query = mapper.ProjectTo<Customer, CustomerDto>(dbContext.Customers);

// Compose with LINQ
var results = await query.Where(d => d.IsActive).ToListAsync();

// Retrieve the raw expression for manual use
Expression<Func<Customer, CustomerDto>> expr =
    mapper.GetProjectionExpression<Customer, CustomerDto>();
```

Expressions are cached per type pair. A `ProjectionException` (with `SourceType` and `TargetType` context) is thrown when no mappable properties exist or the source is null.

---

## Nested Graph and Circular Reference Handling

Map deep object graphs safely with configurable reference preservation and depth limits.

```csharp
using KObjectMapper.Configuration;

var options = new GraphMappingOptions()
    .WithReferencePreservation()   // reuse already-mapped target instances
    .WithMaxDepth(32);             // throw InvalidOperationException beyond this depth

mapper.Map(source, target, options);
```

- **Circular reference detection** — objects already in the visited set are not re-entered, preventing `StackOverflowException`.
- **Reference preservation** — when the same source instance appears multiple times in the graph, the same target instance is reused.
- **MaxDepth** — defaults to 64; throws `InvalidOperationException` when exceeded.

---

## Async Mapping and Cancellation

Use `IAsyncObjectMapper` for async pipelines with `CancellationToken` support.

```csharp
using KObjectMapper.Abstractions;

IAsyncObjectMapper asyncMapper = serviceProvider.GetRequiredService<IAsyncObjectMapper>();

// Async map (throws OperationCanceledException if token is already cancelled)
await asyncMapper.MapAsync(source, target, cancellationToken);

// Non-throwing async variant
MappingResult result = await asyncMapper.TryMapAsync(source, target, cancellationToken);
```

### Custom async converters

Implement `IAsyncTypeConverter<TSource, TTarget>` for expensive async transformations:

```csharp
public class PriceLookupConverter : IAsyncTypeConverter<ProductId, ProductDto>
{
    public async Task<ProductDto> ConvertAsync(ProductId source, CancellationToken ct)
    {
        var price = await _priceService.GetAsync(source.Id, ct);
        return new ProductDto { Price = price };
    }
}

// Register
builder.Services.AddKObjectMapper(options =>
{
    options.AddAsyncConverter<ProductId, ProductDto>(new PriceLookupConverter(_priceService));
});
```

`IAsyncObjectMapper` is registered as a singleton in the DI container automatically when `AddKObjectMapper` is called.

---

## Source Generator

KObjectMapper includes an optional Roslyn source generator that produces static mapper classes at compile time, eliminating reflection overhead on hot paths.

### Setup

Add an `<Analyzer>` reference to `KObjectMapper.SourceGenerator` in your project file:

```xml
<ItemGroup>
  <Analyzer Include="path/to/KObjectMapper.SourceGenerator.dll" />
</ItemGroup>
```

### Enabling source generation

Enable globally via DI registration:

```csharp
builder.Services.AddKObjectMapper(options =>
{
    options.EnableSourceGeneration();
    options.AddProfile<CustomerProfile>();
});
```

Enable per map inside a profile:

```csharp
CreateMap<Customer, CustomerDto>().UseSourceGeneration();
```

When generation is unavailable for a type pair, the runtime mapper falls back to the reflection pipeline automatically.

### Constraints

- Only public, non-static, non-indexer properties with matching names are mapped.
- Source and target property types must be directly assignable.
- Unsupported members emit `KOM001` or `KOM002` warnings at build time and are excluded from the generated mapper.

### Troubleshooting

To view the generated files on disk, see [docs/specs/GeneratorDebugging.md](docs/specs/GeneratorDebugging.md).

| Diagnostic | Meaning |
|------------|---------|
| `KOM001` | A source property has no matching property on the target type and will not be mapped. |
| `KOM002` | A property exists on both types but the source type is not assignable to the target type. |

---

## Dependency Injection — Full Example

```csharp
using KObjectMapper.DependencyInjection;
using KObjectMapper.Configuration;
using KObjectMapper.Converters;

builder.Services.AddKObjectMapper(options =>
{
    options.EnableStrictMode();
    options.SetGlobalNullPolicy(NullMappingPolicy.Ignore);
    options.AddConverter<string, Status>(
        EnumConverter.FromString<Status>(ignoreCase: true).AsTypeConverter());
    options.EnableSourceGeneration();
    options.ConfigureGraph(g => g.WithReferencePreservation().WithMaxDepth(32));
    options.WithOnMappingError(err => logger.LogError("Mapping failed: {Reason}", err.Reason));
    options.AddProfilesFromAssembly(typeof(CustomerProfile).Assembly);
});
```

---

## Namespace Reference

| Namespace | Contents |
|-----------|----------|
| `KObjectMapper` | `Mapper`, `AsyncMapper` |
| `KObjectMapper.Abstractions` | `IObjectMapper`, `IAsyncObjectMapper`, `ITypeConverter<,>`, `IAsyncTypeConverter<,>`, `IQueryableMapper` |
| `KObjectMapper.Collections` | `CollectionMergeMode`, `CollectionMappingOptions<,>` |
| `KObjectMapper.Configuration` | `MappingProfile`, `MappingProfileOptions`, `MappingTypeMapConfiguration`, `NullMappingPolicy`, `GraphMappingOptions`, `MappingProfileValidationException` |
| `KObjectMapper.Converters` | `TypeConverters`, `EnumConverter` |
| `KObjectMapper.DependencyInjection` | `ServiceCollectionExtensions` (`AddKObjectMapper`) |
| `KObjectMapper.Extensions` | Implicit mapping extension methods |
| `KObjectMapper.Projections` | `ProjectionException` |

---

Please see the [contributing guide](CONTRIBUTING.md) for project status and contribution policy.
