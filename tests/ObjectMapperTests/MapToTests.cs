using FluentAssertions;
using ObjectMapper;
using ObjectMapperTests.Helpers;

namespace ObjectMapperTests;

public class MapToTests
{
    private readonly CommonAsserts _commonAsserts;

    public MapToTests()
    {
        _commonAsserts = new CommonAsserts();
    }

    [Fact]
    public void Mapping_between_two_objects_of_the_same_type_should_succeed()
    {
        //  Arrange
        var sut = Mapper.Create();
        var firstCustomer = ObjectMother.SampleCustomer;
        var secondCustomer = new Customer
        {
            Id = 5,
            FirstName = "Jane",
            LastName = "Doe",
            PhoneNumber = "5666655598"
        };

        //  Act
        sut.Map<Customer>(firstCustomer, secondCustomer);

        //  Assert
        _commonAsserts.AssertSimilarCustomers(firstCustomer, secondCustomer);
    }

    [Fact]
    public void Mapping_with_the_MapToOfT_overload_should_succeed()
    {
        var sut = Mapper.Create();
        var sourceProduct = ObjectMother.SampleProduct;
        var targetProduct = new Product
        {
            Id = 11,
            Description = "Good lawn mower",
            Price = 200.00M,
            Quantity = 2
        };


        sut.MapTo<Product>(sourceProduct, targetProduct);
        _commonAsserts.AssertSimilarProducts(sourceProduct, targetProduct);
    }
    

    [Fact]
    public void Mapping_with_the_MapFromOfT_overload_should_succeed()
    {
        //  Arrange
        var sut = Mapper.Create();
        var sourceCustomer = ObjectMother.SampleCustomer;
        var targetCustomer = new Customer
        {
            Id = 100,
            FirstName = "Fred",
            LastName = "Mark",
            PhoneNumber = "9305558973"
        };

        //  Act         
        sut.MapFrom<Customer>(sourceCustomer, targetCustomer);

        //  Assert
        _commonAsserts.AssertSimilarCustomers(sourceCustomer, targetCustomer);
    }

    [Fact]
    public void Mapping_with_MapTo_via_type_inference_should_succeed()
    {
        var sut = Mapper.Create();
        var sourceProduct = ObjectMother.SampleProduct;
        var targetProduct = new Product
        {
            Id = 122,
            Description = "Tenis racket",
            Price = 25.00M,
            Quantity = 5
        };

        sut.MapTo(sourceProduct, targetProduct);

        _commonAsserts.AssertSimilarProducts(sourceProduct, targetProduct);
    }


    [Fact]
    public void Mapping_with_MapFromOfT_via_type_inference_should_succeed()
    {
        var sut = Mapper.Create();
        var sourceCustomer = ObjectMother.SampleCustomer;
        var targetCustomer = new Customer
        {
            Id = 2_000,
            FirstName = "Epicurus",
            LastName = "Osemede",
            PhoneNumber = "4027771115"
        };

        sut.MapFrom(sourceCustomer, targetCustomer);

        _commonAsserts.AssertSimilarCustomers(sourceCustomer, targetCustomer);
    }


    [Fact]
    public void Invoking_MapToOfT_on_an_object_without_a_mapper_instance_succeeds()
    {
        var sourceProduct = ObjectMother.SampleProduct;
        var targetProduct = new Product
        {
            Id = 38,
            Description = "Nice book",
            Price = 30.00M,
            Quantity = 1
        };

        sourceProduct.MapTo<Product>(targetProduct);

        _commonAsserts.AssertSimilarProducts(sourceProduct, targetProduct);
    }


    [Fact]
    public void MapToOfT_should_work_when_target_is_a_default_instance()
    {
        var sourceCustomer = ObjectMother.SampleCustomer;
        var targetCustomer = new Customer();

        sourceCustomer.MapTo<Customer>(targetCustomer);

        _commonAsserts.AssertSimilarCustomers(sourceCustomer, targetCustomer);
    }


    [Fact]
    public void Invoking_MapOfT_should_work_when_source_object_is_a_new_default_instance()
    {
        var sourceProduct = new Product();
        var targetProduct = ObjectMother.SampleProduct;

        sourceProduct.MapTo<Product>(targetProduct);

        _commonAsserts.AssertSimilarProducts(sourceProduct, targetProduct);
        _commonAsserts.AssertSimilarProducts(targetProduct, sourceProduct);
    }

    [Fact]
    public void Calling_MapFromOfT_on_a_satisfactory_object_without_mapper_should_succeed()
    {
        var sourceCustomer = ObjectMother.SampleCustomer;
        var targetCustomer = new Customer
        {
            Id = 500,
            FirstName = "Sean",
            LastName = "Daniels",
            PhoneNumber = "1234567890",
        };

        targetCustomer.MapFrom<Customer>(sourceCustomer);

        _commonAsserts.AssertSimilarCustomers(targetCustomer, sourceCustomer);
        _commonAsserts.AssertSimilarCustomers(sourceCustomer, targetCustomer);

    }

    [Fact]
    public void Invoking_MapFromOfT_on_a_default_object_instance_should_succeed()
    {
        var sourceProduct = ObjectMother.SampleProduct;
        var targetProduct = new Product();

        targetProduct.MapFrom<Product>(sourceProduct);

        _commonAsserts.AssertSimilarProducts(sourceProduct, targetProduct);
        _commonAsserts.AssertSimilarProducts(targetProduct, sourceProduct);
    }

    [Fact]
    public void Type_inference_should_work_for_MapToOfT()
    {
        var sourceProduct = ObjectMother.SampleProduct;
        var targetProduct = new Product
        {
            Id = 21,
            Description = "Headphones for everyone",
            Price = 85.00M,
            Quantity = 1
        };

        sourceProduct.MapTo(targetProduct);

        _commonAsserts.AssertSimilarProducts(sourceProduct, targetProduct);
    }

    [Fact]
    public void Non_generic_MapTo_should_correctly_map_compatible_props_of_two_objects_even_of_different_types()
    {
        var sut = Mapper.Create();
        var testCustomer = ObjectMother.SampleCustomer;
        var testEmployee = ObjectMother.SampleEmployee;

        sut.MapTo(testEmployee, testCustomer);

        testCustomer.Should().NotBeNull();
        testCustomer.FirstName.Should().Be(testEmployee.FirstName);
        testCustomer.LastName.Should().Be(testEmployee.LastName);
        testCustomer.Id.Should().NotBe(testEmployee.EmployeeId);
    }

    [Fact]
    public void Non_generic_MapTo_should_be_available_as_an_extension_on_any_object_with_no_need_for_mapper_instance()
    {
        var sampleCustomer = ObjectMother.SampleCustomer;
        var sampleEmployee = ObjectMother.SampleEmployee;

        sampleCustomer.MapTo(sampleEmployee);

        sampleEmployee.Should().NotBeNull();
        sampleEmployee.EmployeeId.Should().NotBe(sampleCustomer.Id);
        sampleEmployee.FirstName.Should().Be(sampleCustomer.FirstName);
        sampleEmployee.LastName.Should().Be(sampleCustomer.LastName);
        sampleEmployee.Salary.Should().Be(100_000.00M);

    }
    
}