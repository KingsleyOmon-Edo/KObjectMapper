

namespace ObjectMapperTests
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TestPlatform.Common.DataCollection;
    using ObjectMapper;
    using ObjectMapperTests.Helpers;
    using Xunit;
    public class MapperTests
    {
        [Fact]
        public void Crate_a_valid_mapper_class()
        {
            var sut = new Mapper();

            sut.Should().NotBeNull();
        }

        [Fact]
        public void Mappping_between_two_objects_of_the_same_type_succeeds()
        {
            //  Arrange
            var sut = new Mapper();
            var firstCustomer = ObjectMother.SampleCustomer;
            var secondCustomer = new Customer
            {
                Id = 5,
                FirstName = "Jane",
                LastName = "Doe",
                PhoneNumber = "5666655598"
            };

            //  Act
            sut.Map<Customer>(firstCustomer, secondCustomer);

            //  Assert
            secondCustomer.Should().NotBeNull();
            secondCustomer.Id.Should().Be(firstCustomer.Id);
            secondCustomer.FirstName.Should().Be(firstCustomer.FirstName);
            secondCustomer.LastName.Should().Be(firstCustomer.LastName);
            secondCustomer.PhoneNumber.Should().Be(firstCustomer.PhoneNumber);

            secondCustomer.Should().BeEquivalentTo(firstCustomer);
            secondCustomer.Should().BeOfType<Customer>();
        }

        [Fact]
        public void Mapping_with_the_MapToOfT_overload_should_succeed()
        {
            var sut = new Mapper();
            var sourceProduct = ObjectMother.SampleProduct;
            var targetProduct = new Product
            {
                Id = 0,
                Description = "A nice bag for your stuff",
                Price = 100.00M,
                Quantity = 10
            };

            
            sut.MapTo<Product>(sourceProduct, targetProduct);

            targetProduct.Should().NotBeNull();
            targetProduct.Should().BeOfType<Product>();
            targetProduct.Should().BeAssignableTo<Product>();

            targetProduct.Id.Should().Be(sourceProduct.Id);
            targetProduct.Description.Should().Be(sourceProduct.Description);
            targetProduct.Quantity.Should().Be(sourceProduct.Quantity);
            targetProduct.Price.Should().Be(sourceProduct.Price);
        }

        [Fact]
        public void Mapping_with_the_MapFromOfT_overload_should_sceed()
        {
            //  Arrange
            var sut = new Mapper();
            var sourceCustomer = ObjectMother.SampleCustomer;
            var targetCustomer = new Customer
            {
                Id = 100,
                FirstName = "Fred",
                LastName = "Mark",
                PhoneNumber = "9305558973"
            };

            //  Act         
            sut.MapFrom<Customer>(sourceCustomer, targetCustomer);

            //  Assert
            targetCustomer.Should().NotBeNull();
            targetCustomer.Should().BeOfType<Customer>();

            targetCustomer.Id.Should().Be(sourceCustomer.Id);
            targetCustomer.FirstName.Should().Be(sourceCustomer.FirstName);
            targetCustomer.LastName.Should().Be(sourceCustomer.LastName);
            targetCustomer.PhoneNumber.Should().Be(sourceCustomer.PhoneNumber);
        }
    }
}
