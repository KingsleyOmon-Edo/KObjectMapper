# Object Mapper

ObjectMapper is an Open Source simple, intuitive, yet effective object-to-object mapper library written in C# .NET. It is relatively easy to set up.  Usage is just as easy with its "Implicit" mode that allows its use, without a mapping object. Obviating the need to instantiate one

The project is in very early stages of development and as such, I am not accepting contributions for now.

The library supports .NET 6.0, the current LTS version of the .NET SDK out of the. Plans are underway to target other other versions of .NET.

## Features

Basic testable features are already in place. Some of these include.

- Object to Object mapping
- Collection mapping

#### Object to object mapping

ObjectMapper can map between any two "mutable" objects regardless of type. It currently supports writig to  public properties of mutable objects. Support for immutable objects is in the works.

#### Collection mapping

ObjetMapper can map an IEnumerable<T> or any collection derived from it. The engineering of this feature is in progress The plane is to have usedul collection mapping at release.
.

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
