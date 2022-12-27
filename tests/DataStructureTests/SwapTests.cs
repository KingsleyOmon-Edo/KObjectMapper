using Extensions.Raw;
using Xunit;

namespace GenericsDemoTests
{
    using FluentAssertions;

    public class SwapTests
    {
        [Fact]
        public void Swap_correctly_swaps_two_integer_values()
        {
            var a = 10;
            var b = 20;

            BehaviorBag.Swap<int>(ref a, ref b);

            Assert.Equal(20, a);
            a.Should().Be(20);

            Assert.Equal(10, b);
            b.Should().Be(10);
        }

        [Theory]
        [InlineData(10, 20, 20, 10)]
        [InlineData(2, 4, 4, 2)]
        [InlineData(true, false, false, true)]
        [InlineData("left", "right", "right", "left")]
        public void Swap_correctly_swaps_any_two_arbitrary_values<T>(T a, T b, T swappedA, T swappedB)
        {
            BehaviorBag.Swap<T>(ref a, ref b);

            Assert.Equal<T>(a, swappedA);
            Assert.Equal<T>(b, swappedB);        
        }
        
    }
}
