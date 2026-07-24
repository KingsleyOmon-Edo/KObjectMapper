using KObjectMapper.Extensions;
using KObjectMapper.UnitTests.Helpers;

namespace KObjectMapper.UnitTests.ImplicitMapping;

public class MapToIEnumerableOfTTests
{
    private readonly CommonAsserts _commonAsserts;

    public MapToIEnumerableOfTTests() => _commonAsserts = CommonAsserts.Create();

    [Fact]
    public void Implicit_collection_mapping_via_extension_methods_without_explicit_generic_arguments_should_succeed()
    {
        List<Customer> customers = ObjectMother.SampleCustomerData;
        List<CustomerDto> customerDtos = new();

        IEnumerable<CustomerDto> mappedCustomerDtos = customers.MapTo(customerDtos);
        customerDtos = mappedCustomerDtos.ToList();

        _commonAsserts.AssertCustomerDtoDataCorrectlyMapsFromCustomerData(customerDtos, customers);
    }
}
