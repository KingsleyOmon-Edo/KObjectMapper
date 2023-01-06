# Object Mapper

ObjectMapper is an Open Source simple, intuitive, yet effective object-to-object mapper library written in C# .NET. It is relatively easy to set up and use, especially in implicit mapping mode, described below, which completely obviates the need to new up a Mapper instance.

For now, the project is in the very early stages of development and I am not accepting contributions for now.

Currently, the project supports .NET 6 the current LTS version, out of the box. Plans are underway to add support to previous versions of the .NET framework and runtime in the upcoming days.

## Features

The core of an Object mapper is to copy properties from one object to another. Ditto that for collections. The basics of this feature are already in place and will be augmented as the days go by.

But here is a brief description of what has been implemented so far: summary of the features:

- Object to Object mapping
- Collection mapping

#### Object to object mapping

ObjectMapper can map between any two "mutable" objects regardless of type. By default, it can successfully map public writable properties. Support for immutable objects is in the works.

#### Collection mapping

ObjetMapper can map an IEnumerable<T> or any collection derived from it. The engineering of this feature is in progress and I hope to have a useful and more complete implementation at release. But for now, out of the box, it can map any collection of mutable objects that meets the above constraints.

## Installation

```
    dotnet add package ObjectMapper
```

## Usage scenarios

### Implicit, or transparent mapping

"Implicit mapping", also called Transparent mapping, is intended to take all the ceremony and set up, usually needed to use an Object to Object Mapper.

ObjectMapper accomplishes this by attaching mapping behaviors to all objects in scope, after the installation of the ObjectMapper Nuget package. To perform mapping, you just invoke the applicable map method on the current object as the invocation subject. This is also called the "source" object. The mapping is effectuated via a mediating mapping service over to the receiving object, called the "target".

The operations utilize "MapTo" and "MapFrom" semantics to make the directionality of the mapping obvious. For example, to map a "Customer" object to a "CustomerDto" one could use either the MapTo() or MapFrom() methods, as described below:

```
var customer = new Customer { Id = 25, FirstName = "Will", LastName = "Smith", PhoneNumber = "5555234432" };

CustomerDto customerDto = new();

customer.MapTo(customerDto);

```

Conversely, the MapFrom() method could be used to achieve the same objective in the opposite direction. Hence the above, could be re-written as:

```
var customer = new Customer { Id = 25, FirstName = "Will", LastName = "Smith", PhoneNumber = "5555234432" };

CustomerDto customerDto = new();

customerDto.MapFrom(customer);

```

Furthermore, the observant may have noticed that you could use one method to achieve both use cases. So, you may decide to stick with one method, and swap the source and target objects as necessary, when needed.

Hence mapping a "Customer" to a "CustomerDto" would be:

```
    customer.MapTo(customerDto);
```

Mapping from CustomerDto to Customer would be:

```
    customerDto.MapTo(customer);
```

That's all there is to it folks. No need to manually instantiate or inject a Mapper via DI.

### Explicit or conventional mapping

For completeness, I provided what I call "Explicit Mapping". Or what some may call conventional mapping.

This is the usual mode conventionally employed in the utilization of Object to Object Mappers. Using either direct manual instantiation, or using a Dependency Injection container.

Here is how to accomplish the above using manual instantiation of the ObjectMapper's Mapper class, with the provided factory method.

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

- Call the mapper's Map<TSource, TTareget>() with the right signature, to effect the mapping.

  ```
  mapper.Map<Customer, CustomerDto>(customer, customerDto);

  ```

###### 2. Instantiation using the ASP.NET Dependency Inversion container.

- Ensure the ObjectMapper Nuget package has been installed in your project as described above.
- In Program.cs register the IObjectMapper interface as the "abstraction", and the concrete Mapper class as the "implementation". as shown below:

```
    builder.Services.AddScoped<IObjectMapper, Mapper>();
```

- In the controller class, define a private readonly member variable of type IObjectMapper called \_mapper or anything you prefer, like so:

- Declare a private readonly member variable like so:

```
    private readonly IObjectMapper _mapper;
```

- 5. Initialize the variable from the constructor, like so:

```
    CustomersController(IObjectMapper mapper)
    {
        _mapper = mapper;
    }
```

- 6. Now, whenever an IObjectMapper instance is required within the class, in harmony with the Hollywood principle, the DI container will auto-instantiate, and make one available for you.

- 7. You may then proceed to use the mapper as usual, like so:

```
     var customer = new Customer { Id = 25, FirstName = "Will", LastName = "Smith", PhoneNumber = "5555234432" };

    CustomerDto customerDto = new();

    _mapper.Map<Customer, CustomerDto>(customer, customerDto);
```
