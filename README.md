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
using KObjectMapper.Configuration;

// Global policy
builder.Services.AddKObjectMapper(options =>
{
    options.WithNullPolicy(NullMappingPolicy.Ignore); // skip null source props
});

// Per-map policy with a null substitute
CreateMap<Order, OrderDto>()
    .WithNullPolicy(NullMappingPolicy.Ignore)
    .SubstituteNullWith(tgt => tgt.Status, "Unknown");
```

Available policies:

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

## Dependency Injection — Full Example

```csharp
using KObjectMapper.DependencyInjection;
using KObjectMapper.Configuration;
using KObjectMapper.Converters;

builder.Services.AddKObjectMapper(options =>
{
    options.EnableStrictMode();
    options.WithNullPolicy(NullMappingPolicy.Ignore);
    options.AddConverter<string, Status>(
        EnumConverter.FromString<Status>(ignoreCase: true).AsTypeConverter());
    options.AddProfilesFromAssembly(typeof(CustomerProfile).Assembly);
});
```

---

## Namespace Reference

| Namespace | Contents |
|-----------|----------|
| `KObjectMapper` | `Mapper` — main entry point |
| `KObjectMapper.Abstractions` | `IObjectMapper`, `ITypeConverter<,>`, `IEnumConverter<,>`, `EnumConversionResult<>` |
| `KObjectMapper.Configuration` | `MappingProfile`, `MappingProfileOptions`, `MappingTypeMapConfiguration`, `NullMappingPolicy`, `MappingProfileValidationException` |
| `KObjectMapper.Converters` | `TypeConverters`, `EnumConverter` |
| `KObjectMapper.DependencyInjection` | `ServiceCollectionExtensions` (`AddKObjectMapper`) |
| `KObjectMapper.Extensions` | Implicit mapping extension methods |

---

Please see the [contributing guide](CONTRIBUTING.md) for project status and contribution policy.

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

### Enabling Source Generation

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

### Constraints

- Only public, non-static, non-indexer properties with matching names are mapped.
- Source and target property types must be directly assignable.
- Unsupported members emit `KOM001` or `KOM002` warnings at build time and are excluded from the generated mapper.

### Troubleshooting

To view the generated files on disk, see [docs/specs/GeneratorDebugging.md](docs/specs/GeneratorDebugging.md).

| Diagnostic | Meaning |
|------------|---------|
| `KOM001` | A source property has no matching property on the target type and will not be mapped. The diagnostic message includes the source property name. |
| `KOM002` | A property exists on both types but the source type is not assignable to the target type. The diagnostic message includes the member name, source type, and target type. |
