using ObjectMapper;
using ObjectMapperTests.Helpers;

namespace ObjectMapperTests;

public class MapToOfTTests
{
    private readonly CommonAsserts _commonAsserts;

    public MapToOfTTests()
    {
        _commonAsserts = CommonAsserts.Create();
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
    public void Invoking_MapToOfT_should_work_when_source_object_is_a_new_default_instance()
    {
        var sourceProduct = new Product();
        var targetProduct = ObjectMother.SampleProduct;

        sourceProduct.MapTo<Product>(targetProduct);

        _commonAsserts.AssertSimilarProducts(sourceProduct, targetProduct);
        _commonAsserts.AssertSimilarProducts(targetProduct, sourceProduct);
    }
}