using KObjectMapper.Extensions;
using KObjectMapperTests.Abstractions;
using KObjectMapperTests.Helpers;

namespace KObjectMapperTests.ImplicitMapping
{
    public class MapToOfTExtensionTests : IImplicitMappingTests
    {
        private readonly CommonAsserts _commonAsserts;

        public MapToOfTExtensionTests() => _commonAsserts = CommonAsserts.Create();

        [Fact]
        public void Implicit_forward_mapping_via_extensions_from_a_Customer_entity_to_a_customer_Dto_should_succeed()
        {
            var customer = ObjectMother.SampleCustomer;
            var customerDto = ObjectMother.SampleCustomerDto;

            customer.MapTo<CustomerDto>(customerDto);

            _commonAsserts.AssertCustomerCorrectlyMapsToCustomerDto(customer, customerDto);
        }

        [Fact]
        public void
            Implicit_reverse_mapping_via_extension_methods_from_a_customerDto_back_to_a_Customer_entity_should_succeed()
        {
            var customerDto = ObjectMother.SampleCustomerDto;
            var customer = ObjectMother.SampleCustomer;

            customerDto.MapTo<Customer>(customer);

            _commonAsserts.AssertCustomerDtoCorrectlyMapsToCustomer(customerDto, customer);
        }

        [Fact]
        public void Implicit_forward_mapping_via_extension_methods_of_any_two_dissimilar_types_should_succeed()
        {
            var customer = ObjectMother.SampleCustomer;
            var employee = ObjectMother.SampleEmployee;

            customer.MapTo<Employee>(employee);

            _commonAsserts.AssertCustomerCorrectlyMapsToEmployee(customer, employee);
        }

        [Fact]
        public void Implicit_reverse_mapping_via_extension_methods_of_any_two_dissimilar_types_should_succeed()
        {
            var employee = ObjectMother.SampleEmployee;
            var customer = ObjectMother.SampleCustomer;

            employee.MapTo<Customer>(customer);

            _commonAsserts.AssertThatEmployeeCorrectlyMapsToCustomer(employee, customer);
        }

        [Fact]
        public void
            Passing_a_null_source_object_in_implicit_mapping_via_extension_method_should_throw_ArgumentNullException()
        {
            Customer customer = null;
            var customerDto = ObjectMother.SampleCustomerDto;

            Assert.Throws<ArgumentNullException>(() => { customer.MapTo<CustomerDto>(customerDto); });
        }

        [Fact]
        public void
            Passing_a_null_target_object_in_implicit_mapping_via_extension_method_should_throw_ArgumentNullException()
        {
            var customer = ObjectMother.SampleCustomer;
            CustomerDto customerDto = null;

            Assert.Throws<ArgumentNullException>(() => { customer.MapTo<CustomerDto>(customerDto); });
        }
    }
}