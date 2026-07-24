# KObjectMapper — Production Guide

This guide covers configuration patterns, DI registration, failure modes, edge cases, performance guidance, and migration notes for production use of KObjectMapper.

---

## Configuration Patterns

### Profiles

Profiles are the primary unit of mapping configuration. Define one profile per bounded context or aggregate root.

```csharp
public class OrderProfile : MappingProfile
{
    protected override void Configure()
    {
        CreateMap<Order, OrderDto>()
            .ForMember(src => src.CustomerName, tgt => tgt.Name)
            .Ignore(tgt => tgt.InternalCode)
            .WithNullPolicy(NullMappingPolicy.Ignore)
            .SubstituteNullWith(tgt => tgt.Status, "Unknown");
    }
}
```

Register profiles individually or by assembly scan:

```csharp
options.AddProfile<OrderProfile>();
// or
options.AddProfilesFromAssembly(typeof(OrderProfile).Assembly);
```

### Null Policy

| Policy | Behaviour |
|--------|-----------|
| `Propagate` (default) | Null source value is written to the target member |
| `Ignore` | Null source values are skipped; target retains its value |

Set globally or per map:

```csharp
options.SetGlobalNullPolicy(NullMappingPolicy.Ignore);

// Per map
CreateMap<Order, OrderDto>().WithNullPolicy(NullMappingPolicy.Ignore);
```

### Strict Mode

Enable strict mode to throw `InvalidOperationException` at the call site when no map is registered for a type pair, rather than silently falling back to reflection.

```csharp
options.EnableStrictMode();
```

Combine with startup validation to catch structural problems (missing constructors, invalid converters) before the application serves traffic.

### Type Converters

Register converters globally or per map. Per-map converters take precedence over global ones.

```csharp
// Global
options.AddConverter<decimal, string>(new MoneyConverter());

// Per map
CreateMap<Invoice, InvoiceDto>()
    .AddConverter<decimal, string>(new MoneyConverter());
```

Use the built-in `TypeConverters` for common primitive conversions and `EnumConverter` for safe enum mapping.

### Source Generation

Enable source generation to eliminate reflection overhead on hot paths:

```csharp
options.EnableSourceGeneration();

// Or per map
CreateMap<Customer, CustomerDto>().UseSourceGeneration();
```

When generation is unavailable for a type pair, the runtime falls back to reflection automatically.

### Graph Options

Configure nested object graph behaviour globally:

```csharp
options.ConfigureGraph(g =>
    g.WithReferencePreservation()
     .WithMaxDepth(32));
```

Or pass `GraphMappingOptions` directly to a `Map` call for one-off overrides.

### Async Converters

Register async converters for expensive I/O-bound transformations:

```csharp
options.AddAsyncConverter<ProductId, ProductDto>(new PriceLookupConverter(_priceService));
```

Use `IAsyncObjectMapper` (injected via DI) to execute async pipelines with `CancellationToken` support.

### Sensitive Data Policy

| Policy | Behaviour |
|--------|-----------|
| `ExcludeMarked` (default) | Only `[Sensitive]`-annotated members are excluded |
| `DefaultDeny` | All members excluded unless explicitly allowed via `AllowMember` |

```csharp
options.SetSensitivePolicy(SensitiveMappingPolicy.DefaultDeny);

// In profile — opt-in allowlist
CreateMap<User, UserDto>()
    .AllowMember(tgt => tgt.Username)
    .AllowMember(tgt => tgt.Email);
```

---

## DI Registration — Full Example

```csharp
using KObjectMapper.DependencyInjection;
using KObjectMapper.Configuration;
using KObjectMapper.Converters;

builder.Services.AddKObjectMapper(options =>
{
    // Fail fast on unmapped type pairs
    options.EnableStrictMode();

    // Global null handling
    options.SetGlobalNullPolicy(NullMappingPolicy.Ignore);

    // Sensitive data protection
    options.SetSensitivePolicy(SensitiveMappingPolicy.ExcludeMarked);

    // Custom type converter
    options.AddConverter<string, Status>(
        EnumConverter.FromString<Status>(ignoreCase: true).AsTypeConverter());

    // Async converter
    options.AddAsyncConverter<ProductId, ProductDto>(new PriceLookupConverter(_priceService));

    // Compile-time source generation
    options.EnableSourceGeneration();

    // Graph mapping options
    options.ConfigureGraph(g => g.WithReferencePreservation().WithMaxDepth(32));

    // Observability
    options.WithOnMappingError(err => logger.LogError("Mapping failed: {Reason}", err.Reason));
    options.WithOnMappingCompleted(result => metrics.Record(result));

    // Register all profiles in the assembly
    options.AddProfilesFromAssembly(typeof(CustomerProfile).Assembly);
});
```

### Injecting mapper interfaces

`AddKObjectMapper` registers all three mapper interfaces as singletons:

```csharp
// Synchronous object mapping
public class OrderService(IObjectMapper mapper)
{
    public OrderDto ToDto(Order order) => mapper.Map<Order, OrderDto>(order);
}

// Async mapping pipeline
public class ProductService(IAsyncObjectMapper asyncMapper)
{
    public Task<ProductDto> ToDtoAsync(ProductId id, CancellationToken ct)
        => asyncMapper.MapAsync<ProductId, ProductDto>(id, ct);
}

// EF Core queryable projection
public class CustomerQueryService(IQueryableMapper queryMapper)
{
    public IQueryable<CustomerDto> GetAll(IQueryable<Customer> source)
        => queryMapper.ProjectTo<Customer, CustomerDto>(source);
}
```

---

## Failure Modes

| Scenario | Cause | Resolution |
|----------|-------|------------|
| `InvalidOperationException` at map call site | Strict mode on, no map registered for the type pair | Register a profile or disable strict mode |
| `MappingProfileValidationException` at startup | Target type has no accessible parameterless constructor | Add a public parameterless constructor to the target type |
| `InvalidOperationException` during `MergeByKey` | Key selectors not configured on `CollectionMappingOptions` | Call `.WithKeySelector(...)` before mapping |
| `ProjectionException` from `ProjectTo` | No mappable properties between source and target | Ensure at least one property name matches or add a `ForMember` mapping |
| `InvalidOperationException` during graph mapping | `MaxDepth` exceeded | Increase `WithMaxDepth(n)` or flatten the object graph |
| `OperationCanceledException` from `MapAsync` | `CancellationToken` already cancelled before call | Check `ct.IsCancellationRequested` before calling `MapAsync` |
| `InvalidOperationException` from `EnumConverter.AsTypeConverter()` | Source value is not a valid enum member | Validate input before mapping or use `EnumConverter` directly for non-throwing result |

---

## Edge Cases

### Null source

- `NullMappingPolicy.Propagate` (default): null is written to the target member.
- `NullMappingPolicy.Ignore`: target member retains its current value.
- Use `SubstituteNullWith` to provide a fallback value per member.

### Empty collections

Mapping an empty `IEnumerable<TSource>` produces an empty `IEnumerable<TTarget>`. No exception is thrown. `MergeByKey` with an empty source removes all existing target items.

### Circular references

Enable `WithReferencePreservation()` in `GraphMappingOptions`. Objects already in the visited set are not re-entered, preventing `StackOverflowException`. Without reference preservation, circular graphs will hit `MaxDepth` and throw `InvalidOperationException`.

### Sensitive data

- `[Sensitive]` on a **property**: that property is excluded.
- `[Sensitive]` on a **class**: all properties of that class are excluded.
- `SensitiveMappingPolicy.DefaultDeny`: all members excluded unless explicitly allowed via `AllowMember`.

### Cancellation

`MapAsync` throws `OperationCanceledException` immediately if the token is already cancelled at the point of the call. Long-running async converters should forward the token to their own async operations.

---

## Performance Guidance

### Use source generation for hot paths

Source-generated mappers eliminate reflection and expression compilation overhead. Enable globally with `options.EnableSourceGeneration()` or per map with `.UseSourceGeneration()`. The runtime falls back to reflection automatically for unsupported type pairs.

### Thread safety

The mapper instance registered via `AddKObjectMapper` is a singleton and is thread-safe. Profile configuration is immutable after startup validation completes. `GraphMappingOptions` instances passed per-call are not shared and are safe to create per-request.

### Collection strategy selection

| Strategy | Best for |
|----------|----------|
| `Replace` (default) | Full replacement; simplest and fastest |
| `Append` | Accumulating results across multiple mapping calls |
| `MergeByKey` | Synchronising a target collection with a source by identity key |

Avoid `MergeByKey` on very large collections without indexed key lookups, as the default implementation performs linear scans.

### Queryable projection

`ProjectTo` builds an `Expression<Func<TSource, TTarget>>` that is translated to SQL by EF Core. Expressions are cached per type pair after the first call. Prefer `ProjectTo` over in-memory `Map` for database queries to avoid loading unnecessary columns.

---

## Migration Notes

### Reflection-only to source-generated mapping

1. Add an `<Analyzer>` reference to `KObjectMapper.SourceGenerator.dll` in your project file.
2. Call `options.EnableSourceGeneration()` in your `AddKObjectMapper` registration.
3. Build the project and review `KOM001` / `KOM002` diagnostics for members that cannot be source-generated.
4. For unsupported members, add explicit `ForMember` or `AddConverter` configuration in the relevant profile.
5. The runtime falls back to reflection for any type pair the generator cannot handle — no manual intervention required for those pairs.

### API contract changes between alpha versions

**Pre-release notice:** KObjectMapper is published as `0.0.0-alpha-*`. The API is still evolving and breaking changes may occur between alpha drops.

| Change area | Notes |
|-------------|-------|
| `IObjectMapper` | Core `Map` overloads are stable within an alpha series but may gain new overloads between series |
| `MappingProfile.Configure()` | `CreateMap` fluent API is stable; new fluent methods may be added without breaking existing calls |
| `CollectionMappingOptions<,>` | `WithMergeMode` / `WithKeySelector` API is stable |
| `GraphMappingOptions` | `WithReferencePreservation` / `WithMaxDepth` API is stable |
| `SensitiveMappingPolicy` | Introduced in `0.0.0-alpha-1`; `SetSensitivePolicy` / `AllowMember` API may be refined |
| Namespace changes | Check the CHANGELOG for any namespace moves between alpha versions |

When upgrading between alpha versions:
1. Read the [CHANGELOG](../CHANGELOG.md) for breaking changes.
2. Rebuild and resolve any `KOM001` / `KOM002` source generator diagnostics.
3. Run startup validation (`EnableStrictMode`) in a staging environment before deploying to catch unmapped type pairs introduced by model changes.
