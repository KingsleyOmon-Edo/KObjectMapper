using FluentAssertions;
using ObjectMapper;
using ObjectMapperTests.Helpers;

namespace ObjectMapperTests;

public class MapFromTests
{
    private readonly CommonAsserts _commonAsserts = new();

    [Fact]
    public void Non_generic_MapFrom_should_be_available_as_extension_method_on_any_object_without_a_need_for_a_mapper_instance()
    {
        var testCustomer = ObjectMother.SampleCustomer;
        var testEmployee = ObjectMother.SampleEmployee;

        testCustomer.MapFrom(testEmployee);

        testCustomer.Should().NotBeNull();
        testCustomer.Id.Should().NotBe(testEmployee.EmployeeId);
        testCustomer.FirstName.Should().Be(testEmployee.FirstName);
        testCustomer.LastName.Should().Be(testEmployee.LastName);
    }

    [Fact]
    public void Non_generic_MapFrom_should_correctly_map_compatible_props_of_two_dissimilar_objects()
    {
        var sut = Mapper.Create();
        var sampleEmployee = ObjectMother.SampleEmployee;
        var sampleCustomer = ObjectMother.SampleCustomer;

        sut.MapFrom(sampleCustomer, sampleEmployee);

        sampleEmployee.Should().NotBeNull();
        sampleEmployee.EmployeeId.Should().NotBe(sampleCustomer.Id);
        sampleEmployee.FirstName.Should().Be(sampleCustomer.FirstName);
        sampleEmployee.LastName.Should().Be(sampleCustomer.LastName);
        sampleEmployee.Salary.Should().Be(100_000.00M);
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

        targetCustomer.MapFrom(sourceCustomer);

        _commonAsserts.AssertSimilarCustomers(sourceCustomer, targetCustomer);
    }
    
}