

namespace ExtensionsTests
{
    using Extensions;
    using FluentAssertions;
    using System;
    public class StringExtensionsTests
    {
        [Fact]
        public void A_string_containing_a_series_empty_spaces_is_non_empty()
        {
            var sut = "   ";
            var result = sut.IsEmpty();

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("", true)]
        [InlineData(null, true)]
        [InlineData((string)null, true)]
        [InlineData("    ", false)]
        public void Testing_valid_empty_strings(string sut, bool expectedResult)
        {
            var actualResult = sut.IsEmpty();

            actualResult.Should().Be(expectedResult);
        }
    
        [Fact]
        public void A_string_initialized_with_String_dot_empty_should_pass()
        {
            var sut = String.Empty;
            var result = sut.IsEmpty();

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(" ", true)]
        [InlineData("      ", true)]
        [InlineData("dssd;dkkdd", true)]
        [InlineData("     ", true)]
        [InlineData(null, false)]
        public void Testing_valid_non_empty_strings(string sut, bool expectedResult)
        {
            var actualResult = sut.IsNotEmpty();

            actualResult.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("sdsdfds", true)]
        [InlineData("234234323", true)]
        [InlineData(" ", true)]
        [InlineData("   ", true)]
        public void Any_string_that_has_something_is_non_empty_and_valid(string sut, bool expectedResult)
        {
            var actualResult = sut.IsSomething();

            actualResult.Should().Be(expectedResult);
        }
    }
}
