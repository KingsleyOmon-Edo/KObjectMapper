

namespace ObjectMapperTests
{
    using FluentAssertions;
    using FluentAssertions.Formatting;
    using Microsoft.VisualStudio.TestPlatform.Common.DataCollection;
    using ObjectMapper;
    using ObjectMapperTests.Helpers;
    using System.Net.WebSockets;
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
            AssertSimilarCustomers(firstCustomer, secondCustomer);
        }

        [Fact]
        public void Mapping_with_the_MapToOfT_overload_should_succeed()
        {
            var sut = new Mapper();
            var sourceProduct = ObjectMother.SampleProduct;
            var targetProduct = new Product
            {
                Id = 11,
                Description = "Good lawn mower",
                Price = 200.00M,
                Quantity = 2
            };


            sut.MapTo<Product>(sourceProduct, targetProduct);
            AssertSimilarProducts(sourceProduct, targetProduct);
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
            AssertSimilarCustomers(sourceCustomer, targetCustomer);
        }

        [Fact]
        public void Mapping_via_Map_with_type_inference_should_succeed()
        {
            var sut = new Mapper();
            var sourceCustomer = ObjectMother.SampleCustomer;
            var targetCustomer = new Customer
            {
                Id = 55,
                FirstName = "Remko",
                LastName = "Zingi",
                PhoneNumber = "7075554434"
            };

            //  Act
            sut.Map(sourceCustomer, targetCustomer);


            //  Assert            
            AssertSimilarCustomers(sourceCustomer, targetCustomer);
        }

        private void AssertSimilarProducts(Product sourceProduct, Product targetProduct)
        {
            targetProduct.Should().NotBeNull();
            targetProduct.Should().BeOfType<Product>();
            targetProduct.Should().BeAssignableTo<Product>();

            targetProduct.Id.Should().Be(sourceProduct.Id);
            targetProduct.Description.Should().Be(sourceProduct.Description);
            targetProduct.Quantity.Should().Be(sourceProduct.Quantity);
            targetProduct.Price.Should().Be(sourceProduct.Price);
        }
        private void AssertSimilarCustomers(Customer sourceCustomer, Customer targetCustomer)
        {
            targetCustomer.Should().NotBeNull();
            targetCustomer.Should().BeOfType<Customer>();
            targetCustomer.Should().BeEquivalentTo<Customer>(sourceCustomer);
            targetCustomer.Id.Should().Be(sourceCustomer.Id);
            targetCustomer.FirstName.Should().Be(sourceCustomer.FirstName);
            targetCustomer.LastName.Should().Be(sourceCustomer.LastName);
            targetCustomer.PhoneNumber.Should().Be(sourceCustomer.PhoneNumber);
        }

        [Fact]
        public void Mapping_with_MapTo_via_type_inference_should_succeed()
        {
            var sut = new Mapper();
            var sourceProduct = ObjectMother.SampleProduct;
            var targetProduct = new Product
            {
                Id = 122,
                Description = "Tenis racket",
                Price = 25.00M,
                Quantity = 5
            };

            sut.MapTo(sourceProduct, targetProduct);

            AssertSimilarProducts(sourceProduct, targetProduct);
        }
    }
}
