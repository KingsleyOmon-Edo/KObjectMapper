namespace ObjectMapperTests
{
    using Abstractions;
    using Helpers;
    using Microsoft.CSharp.RuntimeBinder;
    using ObjectMapper;

    public class MapOfTTests : IExplicitMappingTests
    {
        private readonly CommonAsserts _commonAsserts;


        public MapOfTTests() => _commonAsserts = CommonAsserts.Create();


        [Fact]
        public void Explicit_forward_mapping_via_mapper_instance_from_Customer_entity_to_a_CustomerDto_should_succeed()
        {
            var customer = ObjectMother.SampleCustomer;
            var customerDto = ObjectMother.SampleCustomerDto;
            var mapper = Mapper.Create();

            mapper.Map<Customer, CustomerDto>(customer, customerDto);

            _commonAsserts.AssertCustomerCorrectlyMapsToCustomerDto(customer, customerDto);
        }

        [Fact]
        public void
            Explicit_reverse_mapping_via_mapper_instance_from_a_CustomerDto_back_to_a_customer_entity_should_succeed()
        {
            var mapper = Mapper.Create();
            var customerDto = ObjectMother.SampleCustomerDto;
            var customer = ObjectMother.SampleCustomer;

            mapper.Map<CustomerDto, Customer>(customerDto, customer);

            _commonAsserts.AssertCustomerDtoCorrectlyMapsToCustomer(customerDto, customer);
        }

        [Fact]
        public void
            Explicit_forward_mapping_via_mapper_instance_of_any_two_dissimilar_types_via_mapper_instance_should_succeed()
        {
            var customer = ObjectMother.SampleCustomer;
            var employee = ObjectMother.SampleEmployee;
            var mapper = Mapper.Create();

            mapper.Map<Customer, Employee>(customer, employee);

            _commonAsserts.AssertCustomerCorrectlyMapsToEmployee(customer, employee);
        }

        [Fact]
        public void Explicit_reverse_mapping_via_mapper_instance_of_any_two_dissimilar_types_should_succeed()
        {
            var employee = ObjectMother.SampleEmployee;
            var customer = ObjectMother.SampleCustomer;
            var mapper = Mapper.Create();

            mapper.Map<Employee, Customer>(employee, customer);

            _commonAsserts.AssertEmployeeCorrectlyMapsToCustomer(employee, customer);
        }

        [Fact]
        public void
            Passing_a_null_source_object_in_explicit_mapping_via_a_mapper_instance_should_throw_ArgumentNullException()
        {
            Customer customer = null;
            var customerDto = ObjectMother.SampleCustomerDto;
            var mapper = Mapper.Create();

            Assert.Throws<ArgumentNullException>(() => { mapper.Map<Customer?, CustomerDto>(customer, customerDto); });
        }

        [Fact]
        public void
            Passing_a_null_target_object_in_explicit_mapping_via_mapper_instance_throws_ArgumentNullException_when_using_a_mapper_instance()
        {
            var customer = ObjectMother.SampleCustomer;
            CustomerDto customerDto = null;
            var mapper = Mapper.Create();

            Assert.Throws<ArgumentNullException>(() => { mapper.Map<Customer, CustomerDto?>(customer, customerDto); });
        }

        [Fact]
        public void Passing_a_source_object_incompatible_with_the_source_type_parameter_should_fail()
        {
            dynamic pseudoCustomer = ObjectMother.SampleEmployee;
            var customerDto = new CustomerDto();

            var mapper = Mapper.Create();

            Assert.Throws<RuntimeBinderException>(() =>
            {
                mapper.Map<Customer, CustomerDto>(pseudoCustomer, customerDto);
            });
        }

        [Fact]
        public void Pass_a_target_object_incompatible_with_the_target_type_parameter_should_fail()
        {
            var customer = ObjectMother.SampleCustomer;
            dynamic pseudoCustomerDto = ObjectMother.SampleProduct;
            var mapper = Mapper.Create();

            Assert.Throws<RuntimeBinderException>(() =>
            {
                mapper.Map<Customer, CustomerDto>(customer, pseudoCustomerDto);
            });
        }
    }
}