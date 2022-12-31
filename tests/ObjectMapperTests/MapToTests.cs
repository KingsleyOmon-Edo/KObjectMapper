using FluentAssertions;
using ObjectMapper;
using ObjectMapperTests.Helpers;

namespace ObjectMapperTests;

public class MapToTests
{
    //  ToDo: By TDD implement guard clause for null source object for MapTo().
    //  ToDo: By TDD implement guard clause for null target object for MapTo().
    
    private readonly CommonAsserts _commonAsserts;

    public MapToTests()
    {
        _commonAsserts = CommonAsserts.Create();
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