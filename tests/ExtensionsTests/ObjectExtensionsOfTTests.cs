using Extensions;
using ExtensionsTests.Helpers;
using FluentAssertions;

namespace ExtensionsTests
{
    public class ObjectExtensionsOfTTests
    {
        [Fact]
        public void Should_correctly_apply_changes_from_source_to_destination_objects()
        {
            var sourceTodo = new Todo()
            {
                Id = 0,
                Name = "Vacuum",
                IsComplete = true,
            };

            var destinationTodo = new Todo()
            {
                Id = 1,
                Name = "Go for a walk",
                IsComplete = true
            };

            destinationTodo = sourceTodo.ApplyDiffs(destinationTodo);

            Assert.True(destinationTodo.Id == 0);
            Assert.True(destinationTodo.Name == "Vacuum");
        }

        [Fact]
        public void Passing_a_null_source_object_should_fail()
        {
            Todo sourceTodo = null;

            var targetTodo = new Todo
            {
                Id = 5,
                Name = "Study",
                IsComplete = false,
                Secret = "23244@33429e)9@1!3234"
            };

            Action testCode = () =>
            {
                _ = sourceTodo.ApplyDiffs<Todo>(targetTodo);
            };

            Assert.Throws<ArgumentNullException>(testCode);
        }

        [Fact]
        public void Passing_a_null_target_object_should_fail()
        {
            var sourceTodo = new Todo
            {
                Id = 10,
                Name = "Go shopping",
                IsComplete = false,
                Secret = "432153324634(&^$#23-23-2"
            };

            Todo targetTodo = null;

            Action applyAction = () =>
            {
                _ = sourceTodo.ApplyDiffs<Todo>(targetTodo);
            };

            Assert.Throws<ArgumentNullException>(applyAction);
        }

        [Fact]
        public void Passing_objects_with_identical_property_values_should_cause_no_side_effects()
        {
            var result = ObjectMother.ArbitraryCustomer.ApplyDiffs(ObjectMother.ArbitraryCustomer);

            Assert.IsAssignableFrom<Customer>(result);
            Assert.NotNull(result);
            Assert.Equivalent(ObjectMother.ArbitraryCustomer, result);
            Assert.IsType<Customer>(result);

            //  
            result.Should().NotBeNull();
            result.Should().BeOfType<Customer>();
            result.Should().BeAssignableTo<Customer>();
            result.Id.Should().Be(ObjectMother.ArbitraryCustomer.Id);
            result.FirstName.Should().Be(ObjectMother.ArbitraryCustomer.FirstName);
            result.LastName.Should().Be(ObjectMother.ArbitraryCustomer.LastName);
            result.PhoneNumber.Should().Be(ObjectMother.ArbitraryCustomer.PhoneNumber);
        }

        [Fact]
        public void Should_correctly_identify_objects_with_different_property_values()
        {
            var customerOne = ObjectMother.ArbitraryCustomer; 
            var customerTwo = new Customer
            {
                Id = 1_000,
                FirstName = "FNqqqq",
                LastName = "Ono",
                PhoneNumber = "5555550009",
            };

            //Id = 0,
            //FirstName = "James",
            //LastName = "Ono",
            //PhoneNumber = "5555550009"

            var result = customerOne.GetPropertyDiffs<Customer>(customerTwo);
            var actualItemCount = 2;

            result.Should().NotBeNull();
            result.Count.Should().Be(actualItemCount);
        }

        //  TODO: Objects with some property different
        //  TODO: Objects with all properties fifferent
        //  TODO: Objects with no properties different.
    }
}
