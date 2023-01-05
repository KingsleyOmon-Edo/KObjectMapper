namespace ObjectMapperTests
{
    using System.Collections;
    using Abstractions;
    using FluentAssertions;
    using Helpers;
    using ObjectMapper.Extensions;

    public class MapIEnumerableOfTTests : IImplicitMappingTests
    {
        [Fact]
        public void Implicit_forward_mapping_via_extensions_from_a_Customer_entity_to_a_customer_Dto_should_succeed()
        {
            var customers = new List<Customer>
            {
                new() { Id = 0, FirstName = "Jane", LastName = "Doe", PhoneNumber = "6555555555" },
                new() { Id = 1, FirstName = "Ray", LastName = "Ono", PhoneNumber = "983665541" }
            };

            //var customerDtos = new List<CustomerDto>();

            //customerDtos = customerDtos.MapFrom<Customer, CustomerDto>(customers).ToList();
            List<CustomerDto> customerDtos = new();
            customerDtos = customerDtos.MapFrom<Customer, CustomerDto>(customers).ToList();

            customerDtos.Should().NotBeNull();
            customerDtos.Count.Should().Be(customers.Count);
        }

        [Fact]
        public void
            Implicit_reverse_mapping_via_extension_methods_from_a_customerDto_back_to_a_Customer_entity_should_succeed()
        {
        }

        public void Implicit_forward_mapping_via_extension_methods_of_any_two_dissimilar_types_should_succeed()
        {
            throw new NotImplementedException();
        }

        public void Implicit_reverse_mapping_via_extension_methods_of_any_two_dissimilar_types_should_succeed()
        {
            throw new NotImplementedException();
        }

        public void
            Passing_a_null_source_object_in_implicit_mapping_via_extension_method_should_throw_ArgumentNullException()
        {
            throw new NotImplementedException();
        }

        public void
            Passing_a_null_target_object_in_implicit_mapping_via_extension_method_should_throw_ArgumentNullException()
        {
            throw new NotImplementedException();
        }

        private bool CheckCollection<T>(T parameter) => parameter is IEnumerable;
    }
}