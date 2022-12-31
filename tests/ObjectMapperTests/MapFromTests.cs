using FluentAssertions;
using ObjectMapper;
using ObjectMapperTests.Helpers;

namespace ObjectMapperTests;

public class MapFromTests
{
    private readonly CommonAsserts _commonAsserts = CommonAsserts.Create();

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
        sut.MapFrom(sourceCustomer, targetCustomer);

        //  Assert
        _commonAsserts.AssertSimilarCustomers(sourceCustomer, targetCustomer);
    }
    
    [Fact]
    public void Passing_a_null_source_object_to_non_generic_MapFrom_throws_ArgumentNullException()
    {
        var sut = Mapper.Create();

        Product sourceProduct = null;
        var targetProduct = ObjectMother.SampleProduct;

        Assert.Throws<ArgumentNullException>(() => { sut.MapFrom(sourceProduct, targetProduct as object); });
    }

    [Fact]
    public void Passing_a_null_target_object_to_non_generic_MapFrom_throws_ArgumentNullException()
    {
        var sut = Mapper.Create();

        var sourceCustomer = ObjectMother.SampleCustomer;
        Customer? targetCustomer = null;

        Assert.Throws<ArgumentNullException>(() => { sut.MapFrom(sourceCustomer, targetCustomer as object); });
    }
}