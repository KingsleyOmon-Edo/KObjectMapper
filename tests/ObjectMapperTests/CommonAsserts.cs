using KObjectObjectMapper.Extensions;

namespace ObjectMapperTests
{
    using FluentAssertions;
    using Helpers;

    public class CommonAsserts
    {
        #region "Constructor and factory"

        private CommonAsserts()
        {
        }

        public static CommonAsserts Create() => new();

        #endregion

        #region "Common asserts"

        internal void AssertSimilarProducts(Product sourceProduct, Product targetProduct)
        {
            targetProduct.Should().NotBeNull();
            targetProduct.Should().BeOfType<Product>();
            targetProduct.Should().BeAssignableTo<Product>();

            targetProduct.Id.Should().Be(sourceProduct.Id);
            targetProduct.Description.Should().Be(sourceProduct.Description);
            targetProduct.Quantity.Should().Be(sourceProduct.Quantity);
            targetProduct.Price.Should().Be(sourceProduct.Price);
        }

        internal void AssertSimilarCustomers(Customer sourceCustomer, Customer targetCustomer)
        {
            targetCustomer.Should().NotBeNull();
            targetCustomer.Should().BeOfType<Customer>();
            targetCustomer.Should().BeEquivalentTo(sourceCustomer);
            targetCustomer.Id.Should().Be(sourceCustomer.Id);
            targetCustomer.FirstName.Should().Be(sourceCustomer.FirstName);
            targetCustomer.LastName.Should().Be(sourceCustomer.LastName);
            targetCustomer.PhoneNumber.Should().Be(sourceCustomer.PhoneNumber);
        }

        #endregion

        #region "Asserts using 'MapFrom' semantics"

        public void AssertCustomerIsCorrectlyMappedFromEmployee(Customer customer, Employee employee)
        {
            customer.Should().NotBeNull();
            customer.Id.Should().NotBe(employee.EmployeeId);
            customer.FirstName.Should().Be(employee.FirstName);
            customer.LastName.Should().Be(employee.LastName);
        }

        public void AssertCustomerDtoIsCorrectlyMappedFromCustomer(CustomerDto customerDto, Customer customer)
        {
            customerDto.Should().NotBeNull();
            customerDto.Id.Should().Be(customer.Id);
            customerDto.FirstName.Should().Be(customer.FirstName);
        }

        public void AssertCustomerIsCorrectlyMappedFromCustomerDto(Customer customer, CustomerDto customerDto)
        {
            customer.Should().NotBeNull();
            customer.Id.Should().Be(customerDto.Id);
            customer.FirstName.Should().Be(customerDto.FirstName);
        }

        #endregion

        #region "Asserts using 'MapTo' semantics"

        public void AssertEmployeeIsCorrectlyMappedFromCustomer(Employee employee, Customer customer)
        {
            employee.Should().NotBeNull();
            employee.EmployeeId.Should().NotBe(customer.Id);
            employee.FirstName.Should().Be(customer.FirstName);
            employee.LastName.Should().Be(customer.LastName);
            employee.Salary.Should().Be(100_000.00M);
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

        #endregion

        public void AssertCustomerDtoDataCorrectlyMapsFromCustomerData(List<CustomerDto> customerDtos, List<Customer> customers)
        {
            customerDtos.Should().NotBeNull();
            customerDtos.Count.Should().Be(customers.Count);
            customerDtos[1].FirstName.Should().Be(customers[1].FirstName);
            customerDtos[1].PhoneNumber.Should().Be(customers[1].PhoneNumber);
        }

        public void AssertCustomerDataIsCorrectlyMappedFromCustomerDtoData(List<Customer> customers, List<CustomerDto> customerDtos)
        {
            customers = customers.MapFrom<CustomerDto, Customer>(customerDtos).ToList();
            customers.Count.Should().Be(customerDtos.Count);
            customers[0].Id.Should().Be(customerDtos[0].Id);
            customers[0].FirstName.Should().Be(customerDtos[0].FirstName);
        }

        public void AssertCustomerDataIsCorrectlyMappedFromEmployeeData(List<Customer> customers, List<Employee> employees)
        {
            customers.Should().NotBeNull();
            customers[1].FirstName.Should().Be(employees[1].FirstName);
            customers[1].LastName.Should().Be(employees[1].LastName);
        }

        public void AssertEmployeeDataIsCorrectlyMappedFromCustomerData(List<Employee> employees, List<Customer> customers)
        {
            employees.Should().NotBeNull();
            employees[1].FirstName.Should().Be(customers[1].FirstName);
            employees[1].LastName.Should().Be(customers[1].LastName);
        }
    }
}