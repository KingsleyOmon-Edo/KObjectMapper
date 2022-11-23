using System;
using DataStructures.Generics;
using DataStructureTests.Helpers;
using FluentAssertions;
using Xunit;

namespace GenericsDemoTests
{
    public class VariantStackTests
    {
        [Fact]
        public void Covariance_SubType_stack_is_assignable_IPoppable_of_baseType()
        {
            DataStructures.Generics.Stack<Rectangle> rectStack = new DataStructures.Generics.Stack<Rectangle>();
            rectStack.Push(new Rectangle());
            rectStack.Push(new Rectangle());

            IPoppable<Shape> shapes = rectStack;

            Shape result = shapes.Pop();

            //  Asserts
            result.Should().NotBeNull();
            shapes.Count.Should().Be(1);

        }

        [Fact]
        public void Contravariance_stack_of_baseType_is_assignable_to_IPushable_of_subtype()
        {
            //Helpers.Stack<Shape> shapes = new Helpers.Stack<Shape>();
            IPushable<Shape> shapes = new DataStructures.Generics.Stack<Shape>();

            shapes.Push(new Shape());
            shapes.Push(new Shape());

            IPushable<Square> squareStack = shapes;
            squareStack.Push(new Square());

            squareStack.Should().NotBeNull();   
            squareStack.Count.Should().Be(3);
          
        }
    }
}
