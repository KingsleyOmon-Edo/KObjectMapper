using KObjectMapper.Extensions;
using KObjectMapper;
using Shouldly;
using System.Collections;
using System.Collections.Generic;
using KObjectMapper.UnitTests.Helpers;


namespace KObjectMapper.UnitTests.CharacterizationTests;

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
        customerDto.ShouldNotBeNull();

        _commonAsserts.AssertCustomerDtoIsCorrectlyMappedFromCustomer(customerDto, customer);
    }

    [Fact]
    public void Mapping_from_an_existing_IEnumerableOfT_to_new_dynamically_instantiated_one_should_succeed()
    {
        var sut = Mapper.Create();
        IEnumerable<Customer> customers = ObjectMother.SampleCustomerData;
        IEnumerable<CustomerDto> customerDtos = sut.Map<Customer, CustomerDto>(customers);

        customerDtos.ShouldNotBeNull();
        
        _commonAsserts.AssertCustomerDtoDataCorrectlyMapsFromCustomerData(customerDtos.ToList(), customers.ToList());
    }
}
