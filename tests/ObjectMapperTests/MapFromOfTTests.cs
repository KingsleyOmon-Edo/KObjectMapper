using FluentAssertions;
using ObjectMapper;
using ObjectMapper.Extensions;
using ObjectMapperTests.Helpers;

namespace ObjectMapperTests;

public class MapFromOfTTests
{
    //  ToDo: Validate types of source objects
    //  ToDo: Validate types of target objects
    //  ToDo: Validate both types when different types are supplied for each Map<TSource, TTarget>(TSource source, TTarget target)
    //  ToDo: Validate the type ignorance still works.
    //  ToDo: POC: Setup a sample, install the nuget and test. Fix any issues.

    private readonly CommonAsserts _commonAsserts;

    public MapFromOfTTests()
    {
        _commonAsserts = CommonAsserts.Create();
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

        sut.MapFrom<Customer>(sourceCustomer, targetCustomer);

        _commonAsserts.AssertSimilarCustomers(sourceCustomer, targetCustomer);
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
            PhoneNumber = "1234567890"
        };

        targetCustomer.MapFrom<Customer>(sourceCustomer);

        _commonAsserts.AssertSimilarCustomers(targetCustomer, sourceCustomer);
        _commonAsserts.AssertSimilarCustomers(sourceCustomer, targetCustomer);
    }

    [Fact]
    public void Invoking_MapFromOfT_on_a_default_object_instance_should_work()
    {
        var sourceProduct = ObjectMother.SampleProduct;
        var targetProduct = new Product();

        targetProduct.MapFrom<Product>(sourceProduct);

        _commonAsserts.AssertSimilarProducts(sourceProduct, targetProduct);
        _commonAsserts.AssertSimilarProducts(targetProduct, sourceProduct);
    }

    [Fact]
    public void Type_inference_should_work_for_MapFromOfT()
    {
        var sourceCustomer = ObjectMother.SampleCustomer;
        var targetCustomer = new Customer();

        //  Call MapFrom<T>() by type inference.
        targetCustomer.MapFrom(sourceCustomer);

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

}