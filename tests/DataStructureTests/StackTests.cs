using DataStructureTests.Helpers;
using Xunit;

namespace GenericsDemoTests
{
    using FluentAssertions;
    using System.Collections.Generic;

    public class StackTests
    {
        [Fact]
        public void StackOfT_correctly_pushes_an_integer_value()
        {
            var sut = new Stack<int>();
            sut.Push(1);
            var actual = sut.Pop();

            Assert.Equal(1, actual);
            actual.Should().Be(1);
        }

        [Fact]
        public void StackOfT_correctly_pushes_a_string_value()
        {
            var sut = new Stack<string>();
            var testString = "test";

            sut.Push(testString);

            var actual = sut.Pop();

            Assert.Equal(testString, actual);
            actual.Should().Be(testString);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData("testString")]
        [InlineData(50.50D)]
        [InlineData(25.50)]
        public void StackOfT_correcty_pushes_a_value_of_any_type<T>(T value)
        {
            var sut = new Stack<T>();
            sut.Push(value);
            var actual = sut.Pop();

            Assert.Equal(value, actual);
            value.Should().Be(actual);
           
        }
        
        [Fact]
        public void StackOfT_correctly_pops_the_last_value()
        {
            var sut = new Stack<int>();
            sut.Push(1);
            sut.Push(2);
            sut.Push(3);

            var expected = 3;
            var actual = sut.Pop();

            Assert.Equal(expected, actual);
            actual.Should().Be(expected);
        }

        [Fact]
        public void StackOfT_correctly_returns_a_stack_of_subtypes()
        {
            DataStructures.Generics.Stack<Shape> shapes = new DataStructures.Generics.Stack<Shape>();
   
            shapes.Push(new Rectangle());
            shapes.Push(new Circle());
            shapes.Push(new Square());
            shapes.Push(new Square());
            shapes.Push(new Rectangle());

            DataStructures.Generics.Stack<Square> squaresOnly = shapes.FilterredStack<Square>();
            squaresOnly.Should().NotBeNull();
            squaresOnly.Count.Should().Be(2);
        }       
    }
}
