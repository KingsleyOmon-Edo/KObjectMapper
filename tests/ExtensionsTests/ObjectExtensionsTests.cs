using Extensions;
using ExtensionsTests.Helpers;
using FluentAssertions;

namespace ExtensionsTests
{
    public class ObjectExtensionsTests
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

            destinationTodo = (Todo)sourceTodo.ApplyDiffs(destinationTodo);

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
                Secret = "adshs!3234"
            };

            Action applyAction = () => { sourceTodo.ApplyDiffs(targetTodo); };

            Assert.Throws<ArgumentNullException>(applyAction);
        }

        [Fact]
        public void Passing_a_null_target_object_should_fail()
        {
            var sourceTodo = new Todo
            {
                Id = 10,
                Name = "Go shopping",
                IsComplete = false,
                Secret = "sahdlsdh23-23-2"
            };

            Todo targetTodo = null;

            Action applyAction = () => { sourceTodo.ApplyDiffs(targetTodo); };

            Assert.Throws<ArgumentNullException>(applyAction);
        }

        [Fact]
        public void Passing_objects_of_dissimilar_types_should_fail()
        {
            Action applyAction = () => { ObjectMother.ArbitraryProduct.SendUpdatesTo(ObjectMother.ArbitraryCustomer); };

            Assert.Throws<ArgumentException>(applyAction);
        }

        [Fact]
        public void Passing_objects_with_identical_property_values_should_cause_no_side_effects()
        {
            var result = (Customer)ObjectMother.ArbitraryCustomer.ApplyDiffs(ObjectMother.ArbitraryCustomer);

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
    }
}