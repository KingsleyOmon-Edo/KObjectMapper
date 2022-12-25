

namespace ObjectMapperTests
{
    using FluentAssertions;
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
            var firstCustomer = ObjectMother.ArbitraryCustomer;
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
    }
}
