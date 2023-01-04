namespace ObjectMapperTests
{
    using Abstractions;
    using Helpers;
    using ObjectMapper;
    using ObjectMapper.Abstractions;

    public class MapTests : IExplicitMappingTests
    {
        private readonly CommonAsserts _commonAsserts = CommonAsserts.Create();

        [Fact]
        public void
            Explicit_forward_mapping_via_mapper_instance_from_Customer_entity_to_a_CustomerDto_should_succeed()
        {
            //  Arrange
            var mapper = new Mapper();
            var customer = ObjectMother.SampleCustomer;
            var customerDto = ObjectMother.SampleCustomerDto;

            //  Act
            mapper.MapFrom(customer, customerDto);

            //  Assert
            _commonAsserts.AssertCustomerDtoIsCorrectlyMappedFromCustomer(customerDto, customer);
        }

        [Fact]
        public void
            Explicit_reverse_mapping_via_mapper_instance_from_a_CustomerDto_back_to_a_customer_entity_should_succeed()
        {
            IMapper mapper = new Mapper();

            var customerDto = ObjectMother.SampleCustomerDto;
            var customer = ObjectMother.SampleCustomer;

            mapper.MapFrom(customerDto, customer);

            //  Assert
            _commonAsserts.AssertCustomerIsCorrectlyMappedFromCustomerDto(customer, customerDto);
        }

        [Fact]
        public void
            Explicit_forward_mapping_via_mapper_instance_of_any_two_dissimilar_types_via_mapper_instance_should_succeed()
        {
            var mapper = new Mapper();

            var customer = ObjectMother.SampleCustomer;
            var employee = ObjectMother.SampleEmployee;

            mapper.MapFrom(employee, customer);

            _commonAsserts.AssertCustomerIsCorrectlyMappedFromEmployee(customer, employee);
        }

        [Fact]
        public void Explicit_reverse_mapping_via_mapper_instance_of_any_two_dissimilar_types_should_succeed()
        {
            var sut = new Mapper();

            var employee = ObjectMother.SampleEmployee;
            var customer = ObjectMother.SampleCustomer;

            sut.MapFrom(customer, employee);

            _commonAsserts.AssertEmployeeIsCorrectlyMappedFromCustomer(employee, customer);
        }

        [Fact]
        public void
            Passing_a_null_source_object_in_explicit_mapping_via_a_mapper_instance_should_throw_ArgumentNullException()
        {
            var mapper = new Mapper();

            Customer customer = null;
            var customerDto = ObjectMother.SampleCustomerDto;

            Assert.Throws<ArgumentNullException>(() => { mapper.MapFrom(customer, customerDto); });
        }

        [Fact]
        public void
            Passing_a_null_target_object_in_explicit_mapping_via_mapper_instance_throws_ArgumentNullException_when_using_a_mapper_instance()
        {
            var sut = new Mapper();

            var customer = ObjectMother.SampleCustomer;
            CustomerDto customerDto = null;

            Assert.Throws<ArgumentNullException>(() => { sut.MapFrom(customer, customerDto); });
        }
    }
}