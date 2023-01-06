# Object Mapper

ObjectMapper is a simple and intuitive, yet effective Open Source Object to object mapper library written in C# .NET.

**Please note that this project is in its early stages and so we are not accepting contributions at this time. Besides the code though functional is not suitable for production use. Tnaks you. **

ObjectMapper is relatively easy to set up and may be used in two modes - _Implicit_ and _Explicit_ The implicit mode for example, leverages extension methods, and performs mapping transparently without the need for a mapping object.

## Installation

```
    dotnet add package ObjectMapper
```

## Usage scenarios

#### Implicit, or transparent mapping

**_Implicit mapping_**, also called Transparent mapping, is intended to take all the ceremony and set up out using an OO mapper.

The operations utilize **MapTo** and **MapFrom** semantics to make the directionality of the mapping obvious. For example, to map a "Customer" object to a "CustomerDto" one could use the MapTo() method as shown below:

<span style="font-size:1.15em">

```
var customer = new Customer { Id = 25, FirstName = "Will", LastName = "Smith", PhoneNumber = "5555234432" };

CustomerDto customerDto = new();

customer.MapTo(customerDto);

```

</span>

Conversely, the _MapFrom()_ method may be used to achieve the same objective in the opposite direction. Hence, peceeding code may be written as:

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

#### Explicit or conventional mapping

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

###### 2. Instantiation using the ASP.NET Dependency Inversion container

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

As this project is in the early stages, we are not taking any contributions are this time.

Please see our contributing page for details.

```

```
