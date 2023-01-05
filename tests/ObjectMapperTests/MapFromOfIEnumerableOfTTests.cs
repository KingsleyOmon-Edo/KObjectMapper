using System.Diagnostics.CodeAnalysis;

namespace ObjectMapperTests
{
    using System.Collections;
    using Abstractions;
    using FluentAssertions;
    using Helpers;
    using ObjectMapper.Extensions;

    public class MapFromIEnumerableOfTTests : IImplicitMappingTests
    {
        private readonly CommonAsserts _commonAsserts;

        public MapFromIEnumerableOfTTests()
        {
            _commonAsserts = CommonAsserts.Create();
        }

        [Fact]
        public void Implicit_forward_mapping_via_extensions_from_a_Customer_entity_to_a_customer_Dto_should_succeed()
        {
            var customers = ObjectMother.SampleCustomerData;
            List<CustomerDto> customerDtos = new();
            customerDtos = customerDtos.MapFrom<Customer, CustomerDto>(customers).ToList();

            _commonAsserts.AssertCustomerDtoDataCorrectlyMapsFromCustomerData(customerDtos, customers);
        }

        [Fact]
        public void
            Implicit_reverse_mapping_via_extension_methods_from_a_customerDto_back_to_a_Customer_entity_should_succeed()
        {
            List<CustomerDto> customerDtos = ObjectMother.SampleCustomerDtoData;
            List<Customer> customers = new();

            customers = customers.MapFrom<CustomerDto, Customer>(customerDtos).ToList();

            _commonAsserts.AssertCustomerDataIsCorrectlyMappedFromCustomerDtoData(customers, customerDtos);
        }

        [Fact]
        public void Implicit_forward_mapping_via_extension_methods_of_any_two_dissimilar_types_should_succeed()
        {
            List<Employee> employees = ObjectMother.SampleEmployeeData;
            List<Customer> customers = new();

            customers = customers.MapFrom<Employee, Customer>(employees).ToList();

            _commonAsserts.AssertCustomerDataIsCorrectlyMappedFromEmployeeData(customers, employees);
        }

        [Fact]
        public void Implicit_reverse_mapping_via_extension_methods_of_any_two_dissimilar_types_should_succeed()
        {
            List<Customer> customers = ObjectMother.SampleCustomerData;
            List<Employee> employees = new();

            employees = employees.MapFrom<Customer, Employee>(customers).ToList();

            _commonAsserts.AssertEmployeeDataIsCorrectlyMappedFromCustomerData(employees, customers);
        }

        [Fact]
        public void
            Passing_a_null_source_object_in_implicit_mapping_via_extension_method_should_throw_ArgumentNullException()
        {
            List<Customer> customers = null;
            List<Employee> employees = ObjectMother.SampleEmployeeData;

            Assert.Throws<ArgumentNullException>(() =>
            {
                employees = employees.MapFrom<Customer, Employee>(customers).ToList();
            });
        }

        [Fact]
        public void
            Passing_a_null_target_object_in_implicit_mapping_via_extension_method_should_throw_ArgumentNullException()
        {
            List<Customer> customers = ObjectMother.SampleCustomerData;
            List<Employee> employees = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                employees = employees.MapFrom<Customer, Employee>(customers).ToList();
            });
        }
    }
}