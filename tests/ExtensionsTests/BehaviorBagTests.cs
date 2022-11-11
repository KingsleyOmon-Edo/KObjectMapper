namespace TodoApiUnitTests
{
    using ExtensionsTests.Helpers;
    using FluentAssertions;
    using TodoApi.Helpers;

    public class BehaviorBagTests
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

            destinationTodo = (Todo)BehaviorBag.ApplyUpdatesTo(sourceTodo, destinationTodo);

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
                Secret = "21302830f)83se##@1!3234"
            };

            Action actionMethod = () => { BehaviorBag.ApplyUpdatesTo(sourceTodo, targetTodo); };
            if (actionMethod == null) throw new ArgumentNullException(nameof(actionMethod));

            Assert.Throws<ArgumentNullException>(actionMethod);
        }

        [Fact]
        public void Passing_a_null_target_object_should_fail()
        {
            var sourceTodo = new Todo
            {
                Id = 10,
                Name = "Go shopping",
                IsComplete = false,
                Secret = "@$###DSd2138023dhf&322"
            };

            Todo targetTodo = null;

            Action applyAction = () => { BehaviorBag.ApplyUpdatesTo(sourceTodo, targetTodo); };

            Assert.Throws<ArgumentNullException>(applyAction);
        }

        [Fact]
        public void Passing_objects_of_dissimilar_types_should_fail()
        {
            Action actionMethod = () =>
            {
                BehaviorBag.ApplyUpdatesTo(ObjectMother.ArbitraryProduct, ObjectMother.ArbitraryCustomer);
            };

            Assert.Throws<ArgumentException>(actionMethod);
        }

        [Fact]
        public void Passing_objects_with_identical_property_values_should_cause_no_side_effects()
        {
            var result =
                (Customer)BehaviorBag.ApplyUpdatesTo(ObjectMother.ArbitraryCustomer, ObjectMother.ArbitraryCustomer);

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