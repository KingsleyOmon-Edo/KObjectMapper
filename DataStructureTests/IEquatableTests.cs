using DataStructureTests.Helpers.SelfReferencing;
using FluentAssertions;
using Xunit;

namespace DataStructureTests
{
    public class IEquatableTests
    {
        [Fact]
        public void Two_baloons_with_identical_property_values_are_equal()
        {
            //  Arrange
            var firstBaloon = new Baloon { Color = "Red", CC = "ff0000" };
            var secondBaloon = new Baloon { Color = "Red", CC = "ff0000" };

            //  Act
            var result = firstBaloon.Equals(secondBaloon);

            //  Assert
            Assert.True(result);
            result.Should().Be(true);
        }

        [Fact]
        public void One_null_baloon_is_not_equal_to_a_valid_baloon_instance()
        {
            Baloon firstBaloon = new() { Color = "Blue", CC = "0000ff" };
            Baloon secondBaloon = null;

            var result = firstBaloon?.Equals(secondBaloon);

            Assert.False(result);
            result.Should().Be(false);
        }
    }
}