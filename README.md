# KObjectMapper

KObjectMapper is a simple, intuitive, and effective open-source object-to-object mapping library for C# and .NET.

It can be used in two modes: **Implicit** and **Explicit**.

**_Pre-release notice: KObjectMapper is currently published as a preview package. The API is still evolving and may include breaking changes between preview versions. Do not use in production workloads yet._**

## Installation

```bash
dotnet add package KObjectMapper --prerelease
```

## Usage scenarios

### Implicit mapping (extension methods)

Implicit mapping uses extension methods and does not require creating a mapper instance.

```csharp
using KObjectMapper.Extensions;
```

#### Map to an existing target

```csharp
var customer = new Customer { Id = 25, FirstName = "Will", LastName = "Smith", PhoneNumber = "5555234432" };
CustomerDto customerDto = new();

customer.MapTo(customerDto);
```

#### Reverse direction with `MapFrom`

```csharp
var customer = new Customer { Id = 25, FirstName = "Will", LastName = "Smith", PhoneNumber = "5555234432" };
CustomerDto customerDto = new();

customerDto.MapFrom(customer);
```

You can also keep `MapTo` and swap source/target objects:

```csharp
customer.MapTo(customerDto);
customerDto.MapTo(customer);
```

### Explicit mapping (mapper instance)

Explicit mapping uses a `Mapper` instance directly.

```csharp
using KObjectMapper;
```

#### 1. Create mapper instance

```csharp
var mapper = Mapper.Create();
```

#### 2. Map to an existing target

```csharp
var customer = new Customer { Id = 25, FirstName = "Will", LastName = "Smith", PhoneNumber = "5555234432" };
CustomerDto customerDto = new();

mapper.Map<Customer, CustomerDto>(customer, customerDto);
```

#### 3. Map to a new destination instance

```csharp
CustomerDto customerDto = mapper.Map<Customer, CustomerDto>(customer);
```

#### 4. Map collections

Map source collection into an existing target collection:

```csharp
IEnumerable<Customer> customers = GetCustomers();
List<CustomerDto> customerDtos = new();

IEnumerable<CustomerDto> mapped = mapper.Map<Customer, CustomerDto>(customers, customerDtos);
```

Map source collection to a newly created destination collection:

```csharp
IEnumerable<Customer> customers = GetCustomers();
IEnumerable<CustomerDto> customerDtos = mapper.Map<Customer, CustomerDto>(customers);
```

### Dependency Injection

If you use ASP.NET Core, register KObjectMapper with the provided service extension.

```csharp
using KObjectMapper;
using KObjectMapper.Abstractions;

builder.Services.AddKObjectMapper();
```

Then inject `IObjectMapper` where needed:

```csharp
private readonly IObjectMapper _mapper;

public CustomersController(IObjectMapper mapper)
{
    _mapper = mapper;
}
```

Example usage:

```csharp
var customer = new Customer { Id = 25, FirstName = "Will", LastName = "Smith", PhoneNumber = "5555234432" };
CustomerDto customerDto = new();

_mapper.Map<Customer, CustomerDto>(customer, customerDto);
```

Please see the contributing guide for project status and contribution policy.

