// Copyright (c) KObjectMapper contributors. All rights reserved.
using KObjectMapper.Extensions;
using KObjectMapperTests.Helpers;
using Shouldly;

namespace KObjectMapperTests;

public class CommonAsserts
{
    private CommonAsserts()
    {
    }

    public static CommonAsserts Create() => new();

    internal static void AssertSimilarProducts(Product sourceProduct, Product targetProduct)
    {
        targetProduct.ShouldNotBeNull();
        targetProduct.ShouldBeOfType<Product>();
        targetProduct.ShouldBeAssignableTo<Product>();

        targetProduct.Id.ShouldBe(sourceProduct.Id);
        targetProduct.Description.ShouldBe(sourceProduct.Description);
        targetProduct.Quantity.ShouldBe(sourceProduct.Quantity);
        targetProduct.Price.ShouldBe(sourceProduct.Price);
    }

    internal static void AssertSimilarCustomers(Customer sourceCustomer, Customer targetCustomer)
    {
        targetCustomer.ShouldNotBeNull();
        targetCustomer.ShouldBeOfType<Customer>();
        targetCustomer.ShouldBeEquivalentTo(sourceCustomer);
        targetCustomer.Id.ShouldBe(sourceCustomer.Id);
        targetCustomer.FirstName.ShouldBe(sourceCustomer.FirstName);
        targetCustomer.LastName.ShouldBe(sourceCustomer.LastName);
        targetCustomer.PhoneNumber.ShouldBe(sourceCustomer.PhoneNumber);
    }

    public void AssertCustomerIsCorrectlyMappedFromEmployee(Customer customer, Employee employee)
    {
        customer.ShouldNotBeNull();
        customer.Id.ShouldNotBe(employee.EmployeeId);
        customer.FirstName.ShouldBe(employee.FirstName);
        customer.LastName.ShouldBe(employee.LastName);
    }

    public void AssertCustomerDtoIsCorrectlyMappedFromCustomer(CustomerDto customerDto, Customer customer)
    {
        customerDto.ShouldNotBeNull();
        customerDto.Id.ShouldBe(customer.Id);
        customerDto.FirstName.ShouldBe(customer.FirstName);
    }

    public void AssertCustomerIsCorrectlyMappedFromCustomerDto(Customer customer, CustomerDto customerDto)
    {
        customer.ShouldNotBeNull();
        customer.Id.ShouldBe(customerDto.Id);
        customer.FirstName.ShouldBe(customerDto.FirstName);
    }

    public void AssertEmployeeIsCorrectlyMappedFromCustomer(Employee employee, Customer customer)
    {
        employee.ShouldNotBeNull();
        employee.EmployeeId.ShouldNotBe(customer.Id);
        employee.FirstName.ShouldBe(customer.FirstName);
        employee.LastName.ShouldBe(customer.LastName);
        employee.Salary.ShouldBe(100_000.00M);
    }

    public void AssertCustomerCorrectlyMapsToCustomerDto(Customer customer, CustomerDto customerDto)
    {
        AssertCustomerDtoIsCorrectlyMappedFromCustomer(customerDto, customer);
    }

    public void AssertCustomerDtoCorrectlyMapsToCustomer(CustomerDto customerDto, Customer customer)
    {
        AssertCustomerIsCorrectlyMappedFromCustomerDto(customer, customerDto);
    }

    public void AssertThatEmployeeCorrectlyMapsToCustomer(Employee employee, Customer customer)
    {
        AssertCustomerIsCorrectlyMappedFromEmployee(customer, employee);
    }

    public void AssertMapperObjectCorrectlyMapsCustomerToCustomerDto(Customer customer, CustomerDto customerDto)
    {
        AssertCustomerDtoIsCorrectlyMappedFromCustomer(customerDto, customer);
    }

    public void AssertMapperObjectCorrectlyMapsCustomerDtoToCustomer(CustomerDto customerDto, Customer customer)
    {
        AssertCustomerIsCorrectlyMappedFromCustomerDto(customer, customerDto);
    }

    public void AssertCustomerCorrectlyMapsToEmployee(Customer customer, Employee employee)
    {
        AssertEmployeeIsCorrectlyMappedFromCustomer(employee, customer);
    }

    public void AssertEmployeeCorrectlyMapsToCustomer(Employee employee, Customer customer)
    {
        AssertCustomerIsCorrectlyMappedFromEmployee(customer, employee);
    }

    public void AssertMapperObjectCorrectlyMapsCustomerToEmployee(Customer customer, Employee employee)
    {
        AssertEmployeeIsCorrectlyMappedFromCustomer(employee, customer);
    }

    public void AssertMapperObjectCorrectlyMapsEmployeeToCustomer(Employee employee, Customer customer)
    {
        AssertCustomerIsCorrectlyMappedFromEmployee(customer, employee);
    }

    public void AssertCustomerDtoDataCorrectlyMapsFromCustomerData(List<CustomerDto> customerDtos, List<Customer> customers)
    {
        customerDtos.ShouldNotBeNull();
        customerDtos.Count.ShouldBe(customers.Count);
        customerDtos[1].FirstName.ShouldBe(customers[1].FirstName);
        customerDtos[1].PhoneNumber.ShouldBe(customers[1].PhoneNumber);
    }

    public void AssertCustomerDataIsCorrectlyMappedFromCustomerDtoData(List<Customer> customers, List<CustomerDto> customerDtos)
    {
        customers = customers.MapFrom<CustomerDto, Customer>(customerDtos).ToList();
        customers.Count.ShouldBe(customerDtos.Count);
        customers[0].Id.ShouldBe(customerDtos[0].Id);
        customers[0].FirstName.ShouldBe(customerDtos[0].FirstName);
    }

    public void AssertCustomerDataIsCorrectlyMappedFromEmployeeData(List<Customer> customers, List<Employee> employees)
    {
        customers.ShouldNotBeNull();
        customers[1].FirstName.ShouldBe(employees[1].FirstName);
        customers[1].LastName.ShouldBe(employees[1].LastName);
    }

    public void AssertEmployeeDataIsCorrectlyMappedFromCustomerData(List<Employee> employees, List<Customer> customers)
    {
        employees.ShouldNotBeNull();
        employees[1].FirstName.ShouldBe(customers[1].FirstName);
        employees[1].LastName.ShouldBe(customers[1].LastName);
    }
}
