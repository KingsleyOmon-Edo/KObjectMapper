# KObjectMapper

KObjectMapper is a simple, intuitive, yet effective, Open Source Object To object mapping library written in C# .NET.

It is easy to set up, and may be used in two modes - **Implicit** and **Explicit** 

Implicit mode leverages C# extension methods, and performs object mappings transparently without the need for a mapping object instance.

**_Please note that this project is in its early stages and I am not accepting contributions at this time. Also, the API is still in flux, so please do not use in production. Thanks_**

## Installation

```
dotnet add package KObjectMapper --version 0.0.0-0.0.1.20230116084130
```

## Usage scenarios

### Implicit mapping

***Implicit mapping***, also called ***Transparent mapping***, is intended to take all the ceremony and set up out of using an Object to Object mapper.

The methods utilize **MapTo** and **MapFrom** semantics to make the directionality of the mapping obvious. For example, to map a "Customer" object to a "CustomerDto" one could use the _MapTo()_ method as shown below:

<span style="font-size:1.15em">

```
var customer = new Customer { Id = 25, FirstName = "Will", LastName = "Smith", PhoneNumber = "5555234432" };
CustomerDto customerDto = new();
customer.MapTo(customerDto);
```
</span>

Conversely, the _MapFrom()_ method may be used to achieve the same objective in the opposite direction. Hence, the preceeding code may be written as:

```
var customer = new Customer { Id = 25, FirstName = "Will", LastName = "Smith", PhoneNumber = "5555234432" };
CustomerDto customerDto = new();
customerDto.MapFrom(customer);
```

You may have noticed you could use one method to achieve both use cases. So, you may decide to stick with one method, and swap the source and target objects back and forht as necessary, when needed.
Hence mapping a "Customer" to a "CustomerDto" could be done this way:

```
customer.MapTo(customerDto);
```

Mapping back from a CustomerDto to Customer would be:

```
customerDto.MapTo(customer);
```

That is all there is folks! No need to manually instantiate or inject a Mapper via DI.

#### Explicit or conventional mapping

For completeness, I provided what I call "Explicit Mapping". Or what some may be used to as conventional mapping.

This is the usual convention employed by Object To Object Mappers. Using either direct manual instantiation, or using a Dependency Injection container.

Here is how to accomplish the above using manual instantiation of the KObjectMapper's Mapper class, with the provided factory method.

###### 1. Instantiation by hand

- Create a Mapper instance with the Create() factory method.

```
var mapper = Mapper.Create();
```

- Setup the objects that need mapping.

```
var customer = new Customer { Id = 25, FirstName = "Will", LastName = "Smith", PhoneNumber = "5555234432" };
CustomerDto customerDto = new();
```

- Call the mapper's Map<TSource, TTarget>() with the right signature, to effect the mapping.

```
mapper.Map<Customer, CustomerDto>(customer, customerDto);
```

###### 2. Instantiation using the ASP.NET Dependency Inversion container

- Ensure the KObjectMapper Nuget package has been installed in your project as described above.
- In Program.cs register the IObjectMapper interface as the "abstraction", and the concrete Mapper class as the "implementation". as shown below:

```
builder.Services.AddScoped<IObjectMapper, Mapper>();
```

- In the controller class, define a private readonly member variable of type IObjectMapper called \_mapper or anything you prefer, like so:

- Declare a private readonly member variable like so:

```
private readonly IObjectMapper _mapper;
```

- Initialize the variable from the constructor, like so:

```
CustomersController(IObjectMapper mapper)
{
    _mapper = mapper;
}
```

- Now, whenever an IObjectMapper instance is required within the class, in harmony with the Hollywood principle, the DI container will auto-instantiate, and make one available for you.

- You may then proceed to use the mapper as usual, like so:

```
var customer = new Customer { Id = 25, FirstName = "Will", LastName = "Smith", PhoneNumber = "5555234432" };
CustomerDto customerDto = new();
_mapper.Map<Customer, CustomerDto>(customer, customerDto);
```
**_Please note that this project is in its early stages and I am not accepting contributions at this time. Also, the API is still in flux, so please do not use in production. Thanks_**

Please see our contributing page for details.

