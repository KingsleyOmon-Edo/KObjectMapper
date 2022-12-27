using DataStructureTests.Helpers.Statics;
using FluentAssertions;
using Xunit;

namespace GenericsDemoTests
{
    public class StaticsTests
    {
        [Fact]
        public void Closed_generics_have_unique_static_data()
        {
            //  Arrange / Act
            ++Bob<int>.Count;
            ++Bob<int>.Count;
            ++Bob<int>.Count;
            ++Bob<int>.Count;

            ++Bob<string>.Count;
            ++Bob<string>.Count;

            //  Assert
            var intCount = Bob<int>.Count;
            var stringCount = Bob<string>.Count;

            intCount.Should().NotBe(stringCount);
        }
    }
}
