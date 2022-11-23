using System;
using System.Collections.Generic;
using Extensions.Raw;
using FluentAssertions;
using Xunit;

namespace GenericsDemoTests
{
    public class MaxTests
    {
        [Fact]
        public void Max_should_return_the_greater_of_two_numbers()
        {
            var a = 1;
            var b = 0;
            var greater = a;

            var result = BehaviorBag.Max(a, b);
            result.Should().Be(greater);
        }

        [Theory]
        [MemberData(nameof(CreateMaxTestData))]
        public void Max_should_return_the_greater_of_any_two_values<T>(T a, T b, T greater) where T : IComparable<T>
        {
            var result = BehaviorBag.Max(a, b);
            result.Should().Be(greater);
        }

       public static IEnumerable<object[]> CreateMaxTestData()
        {
            yield return new object[] { 1, 2, 2 };
            yield return new object[] { 10, 2, 10 };
            yield return new object[] { int.MinValue, int.MaxValue, int.MaxValue };
            yield return new object[] { decimal.MaxValue, decimal.MinValue, decimal.MaxValue };
        }
    }
}