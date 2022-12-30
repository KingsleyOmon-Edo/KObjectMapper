using FluentAssertions;
using ObjectMapperTests.Helpers;

namespace ObjectMapperTests;

public class CommonAsserts
{
    private CommonAsserts()
    {
    }

    public static CommonAsserts Create()
    {
        return new CommonAsserts();
    }

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
        targetCustomer.Should().BeEquivalentTo<Customer>(sourceCustomer);
        targetCustomer.Id.Should().Be(sourceCustomer.Id);
        targetCustomer.FirstName.Should().Be(sourceCustomer.FirstName);
        targetCustomer.LastName.Should().Be(sourceCustomer.LastName);
        targetCustomer.PhoneNumber.Should().Be(sourceCustomer.PhoneNumber);
    }
}