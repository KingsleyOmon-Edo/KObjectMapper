namespace ObjectMapperTests
{
    using FluentAssertions;
    using Helpers;

    public class CommonAsserts
    {
        private CommonAsserts()
        {
        }

        public static CommonAsserts Create() => new();


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

        public void AssertEmployeeIsCorrectlyMappedFromCustomer(Customer customer, Employee employee)
        {
            employee.Should().NotBeNull();
            employee.EmployeeId.Should().NotBe(customer.Id);
            employee.FirstName.Should().Be(customer.FirstName);
            employee.LastName.Should().Be(customer.LastName);
            employee.Salary.Should().Be(100_000.00M);
        }
    }
}