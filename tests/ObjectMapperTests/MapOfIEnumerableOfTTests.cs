using System.Runtime.InteropServices;
using FluentAssertions;
using KObjectObjectMapper;
using ObjectMapperTests.Abstractions;
using ObjectMapperTests.Helpers;

namespace ObjectMapperTests;

public class MapOfIEnumerableOfTTests : IExplicitMappingTests
{
    private readonly CommonAsserts _commonAsserts;

    public MapOfIEnumerableOfTTests()
    {
        _commonAsserts = CommonAsserts.Create();
    }

    [Fact]
    public void Explicit_forward_mapping_via_mapper_instance_from_Customer_entity_to_a_CustomerDto_should_succeed()
    {
        var mapper = KObjectObjectMapper.ObjectMapper.Create();

        List<Customer> customers = ObjectMother.SampleCustomerData;
        List<CustomerDto> customerDtos = new();

        customerDtos = mapper.Map<Customer, CustomerDto>(customers, customerDtos).ToList();
   
        _commonAsserts.AssertCustomerDtoDataCorrectlyMapsFromCustomerData(customerDtos, customers);
    }

    [Fact]
    public void
        Explicit_reverse_mapping_via_mapper_instance_from_a_CustomerDto_back_to_a_customer_entity_should_succeed()
    {
        var mapper = KObjectObjectMapper.ObjectMapper.Create();
        List<CustomerDto> customerDtos = ObjectMother.SampleCustomerDtoData;
        List<Customer> customers = new();

        customers = mapper.Map<CustomerDto, Customer>(customerDtos, customers).ToList();

        _commonAsserts.AssertCustomerDataIsCorrectlyMappedFromCustomerDtoData(customers, customerDtos);
    }

    [Fact]
    public void
        Explicit_forward_mapping_via_mapper_instance_of_any_two_dissimilar_types_via_mapper_instance_should_succeed()
    {
        var mapper = KObjectObjectMapper.ObjectMapper.Create();

        List<Customer> customers = ObjectMother.SampleCustomerData;
        List<Employee> employees = new();

        employees = mapper.Map<Customer, Employee>(customers, employees).ToList();

        _commonAsserts.AssertEmployeeDataIsCorrectlyMappedFromCustomerData(employees, customers);
    }

    [Fact]
    public void Explicit_reverse_mapping_via_mapper_instance_of_any_two_dissimilar_types_should_succeed()
    {
        var mapper = KObjectObjectMapper.ObjectMapper.Create();
        
        List<Employee> employees = ObjectMother.SampleEmployeeData;
        List<Customer> customers = new();

        customers = mapper.Map<Employee, Customer>(employees, customers).ToList();

        _commonAsserts.AssertCustomerDataIsCorrectlyMappedFromEmployeeData(customers, employees);
    }

    [Fact]
    public void
        Passing_a_null_source_object_in_explicit_mapping_via_a_mapper_instance_should_throw_ArgumentNullException()
    {
        var mapper = KObjectObjectMapper.ObjectMapper.Create();
        
        List<Customer> customers = null;
        List<CustomerDto> customerDtos = ObjectMother.SampleCustomerDtoData;

        Assert.Throws<ArgumentNullException>(() =>
        {
            customerDtos = mapper.Map<Customer, CustomerDto>(customers, customerDtos).ToList();
        });
    }

    [Fact]
    public void
        Passing_a_null_target_object_in_explicit_mapping_via_mapper_instance_throws_ArgumentNullException()
    {
        var mapper = KObjectObjectMapper.ObjectMapper.Create();
        
        List<Customer> customers = ObjectMother.SampleCustomerData;
        List<CustomerDto> customerDtos = null;

        Action mapperInvocation = () =>
        {
            customerDtos = mapper.Map<Customer, CustomerDto>(customers, customerDtos).ToList();
        };
        mapperInvocation.Should()
            .Throw<ArgumentNullException>()
            .WithParameterName("target");

    }
}