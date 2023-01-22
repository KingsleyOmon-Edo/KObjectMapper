using KObjectMapper.Extensions;
using KObjectMapperTests.Helpers;
using FluentAssertions;
using KObjectMapper;

namespace KObjectMapperTests.CharacterizationTests;

public class NewTypeTests
{
    private readonly CommonAsserts _commonAsserts;

    public NewTypeTests()
    {
        _commonAsserts = CommonAsserts.Create();
    }
    
    [Fact]
    public void Mapping_from_an_existing_type_to_a_new_one_should_succeed()
    {
        var sut = Mapper.Create();
        Customer customer = ObjectMother.SampleCustomer;
      
        CustomerDto customerDto = sut.Map<Customer, CustomerDto>(customer);
        customerDto.Should().NotBeNull();
        
        _commonAsserts.AssertCustomerDtoIsCorrectlyMappedFromCustomer(customerDto, customer);
    }
}