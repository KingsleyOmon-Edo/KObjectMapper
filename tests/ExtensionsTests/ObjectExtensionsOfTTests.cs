namespace ExtensionsTests
{
    using Extensions;
    using ExtensionsTests.Helpers;
    using FluentAssertions;
    using System.Reflection;

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
            var result = ObjectMother.SampleCustomer.ApplyDiffs(ObjectMother.SampleCustomer);

            Assert.IsAssignableFrom<Customer>(result);
            Assert.NotNull(result);
            Assert.Equivalent(ObjectMother.SampleCustomer, result);
            Assert.IsType<Customer>(result);

            //  
            result.Should().NotBeNull();
            result.Should().BeOfType<Customer>();
            result.Should().BeAssignableTo<Customer>();
            result.Id.Should().Be(ObjectMother.SampleCustomer.Id);
            result.FirstName.Should().Be(ObjectMother.SampleCustomer.FirstName);
            result.LastName.Should().Be(ObjectMother.SampleCustomer.LastName);
            result.PhoneNumber.Should().Be(ObjectMother.SampleCustomer.PhoneNumber);
        }

        [Fact]
        public void Should_correctly_identify_objects_with_different_property_values()
        {
            var customerOne = ObjectMother.SampleCustomer;
            var customerTwo = new Customer
            {
                Id = 1_000,
                FirstName = "Jane",
                LastName = "Ono",
                PhoneNumber = "6655550009",
            };

            var result = customerOne.GetPropertyDiffs<Customer>(customerTwo);
            var expectedItemCount = 3;

            result.Should().NotBeNull();
            result.Count.Should().Be(expectedItemCount);
        }

        [Fact]
        public void Calling_GetPropertyDiff_on_a_null_source_ArgumentNullException()
        {
            Product sourceProuduct = null;
            Product targetProduct = ObjectMother.SampleProduct;

            Assert.Throws<ArgumentNullException>(() =>
            {
                var operationResult = sourceProuduct.GetPropertyDiffs<Product>(targetProduct);
                operationResult.Should().BeOfType(typeof(List<PropertyInfo>));
            });
        }

        [Fact]
        public void Calling_GetPropertyDiff_with_a_null_target_object_throws_ArgumentNullException()
        {
            Product sourceProduct = ObjectMother.SampleProduct;
            Product targetProduct = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                var operationResult = sourceProduct.GetPropertyDiffs<Product>(targetProduct);
                operationResult.Should().BeOfType(typeof(List<PropertyInfo>));
            });
        }


        [Fact]
        public void Two_objects_with_no_corresponding_properties_different_in_value_should_return_no_items()
        {
            //  Arrange
            var sourceProduct = ObjectMother.SampleProduct;
            var targetProduct = ObjectMother.SampleProduct;

            //  Act
            var operationResult = sourceProduct.GetPropertyDiffs<Product>(targetProduct);
            var actualItemCount = operationResult.Count;
            var expectedItemCount = 0;

            //  Assert
            operationResult.Should().NotBeNull();
            operationResult.Should().BeOfType(typeof(List<PropertyInfo>));
            actualItemCount.Should().Be(expectedItemCount);
        }

        [Fact]
        public void Two_objects_with_all_corresponding_properties_of_the_same_values_should_return_all_items()
        {
            //  Arrange
            var sourceCustomer = ObjectMother.SampleCustomer;
            var targetCustomer = new Customer
            {
                Id = 2_000,
                FirstName = "Jane",
                LastName = "Doe",
                PhoneNumber = "6666888897"
            };

            //  Act
            var operationResult = sourceCustomer.GetPropertyDiffs<Customer>(targetCustomer);
            var expectedItemCount = typeof(Customer).GetProperties().Length;
            var actualItemCount = operationResult.Count;

            //  Assert
            operationResult.Should().NotBeNull();
            operationResult.Should().BeOfType(typeof(List<PropertyInfo>));
            actualItemCount.Should().Be(expectedItemCount);
        }

    
        [Fact]
        public void Passing_a_null_source_property_to_predicate_throws_ArgumentNullExceptioin()
        {
            var sourceProduct = ObjectMother.SampleProduct;
            PropertyInfo sourceDescription = null;
                      
            var targetProduct = new Product
            {
                Id = 10,
                Description = "New-A nice bag for your stuff",
                Price = 200.00M,
                Quantity = 20
            };
            var targetDescription = targetProduct.GetType().GetProperties()[1];

            Assert.Throws<ArgumentNullException>(() =>
            {
                var operationResult = ObjectExtensions.ArePropValuesDifferent<Product>(sourceProduct, sourceDescription, targetProduct, targetDescription);
            });
        }

        [Fact]
        public void Passing_a_null_target_property_throws_ArgumentNullException()
        {
            var sourceCustomer = ObjectMother.SampleCustomer;
            var sourceLastNameProp = sourceCustomer.GetType().GetProperties()[2];
            var targetCustomer = new Customer
            {
                Id = 200,
                FirstName = "June",
                LastName = "Bug",
                PhoneNumber = "6660002284"
            };

            PropertyInfo? targetLastNameProp = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                var operationResult = ObjectExtensions.ArePropValuesDifferent<Customer>(sourceCustomer,
                                                                                        sourceLastNameProp,
                                                                                        targetCustomer,
                                                                                        targetLastNameProp); ;
            });


        }

        [Fact]
        public void Passing_a_two_properties_with_different_names_should_throw_ArgumentException()
        {
            var sourceProduct = ObjectMother.SampleProduct;
            var sourceDescription = sourceProduct.GetType().GetProperties()[1];

            var targetProduct = new Product
            {
                Id = 22,
                Description = "A funky t-shirt",
                Price = 25.00M,
                Quantity = 2
            };

            var targetPrice = targetProduct.GetType().GetProperties()[2];

            Assert.Throws<ArgumentException>(() =>
            {
                var operationResult = ObjectExtensions.ArePropValuesDifferent<Product>(sourceProduct,
                                                                                       sourceDescription,
                                                                                       targetProduct,
                                                                                       targetPrice);
            });
        }

        
        [Fact]
        public void Passing_two_properties_of_dissimilar_types_should_throws_ArgumentException()
        {
            dynamic edward = new { FirstName = "Edward", LastName = "Simon", Salary = 100_000M };
            var edwardsSalary = edward.GetType().GetProperties()[2];

            dynamic sean = new { FirstName = "Sean", LastName = "Ratchet", Salary = "100000.00" };
            var seansSalary = sean.GetType().GetProperties()[2];

            Assert.Throws<ArgumentException>(() =>
            {
                var operationResult = ObjectExtensions.ArePropValuesDifferent<dynamic>(sean, seansSalary, edward, edwardsSalary);

            });

        }

        [Fact]
        public void Passing_a_null_source_object_to_predicate_throws_ArgumentNullException()
        {
            Product sourceProduct = null;          
            var sourceProductDesc = sourceProduct?.GetType().GetProperties()[1];

            var targetProduct = ObjectMother.SampleProduct;
            var targetProductDesc = targetProduct.GetType().GetProperties()[1];

            Assert.Throws<ArgumentNullException>(() =>
            {
                var operationResult = ObjectExtensions.ArePropValuesDifferent<Product>(sourceProduct,
                                                                                       sourceProductDesc,
                                                                                       targetProduct,
                                                                                       targetProductDesc);
            });
        }

        [Fact]
        public void Passing_a_null_target_object_to_predicate_throws_ArgumentNullException()
        {
            Product sourceProduct = ObjectMother.SampleProduct;
            var sourceProductDesc = sourceProduct?.GetType().GetProperties()[1];

            Product targetProduct = null;    

            var targetProductDesc = targetProduct?.GetType().GetProperties()[1];

            Assert.Throws<ArgumentNullException>(() =>
            {
                var operationResult = ObjectExtensions.ArePropValuesDifferent<Product>(sourceProduct,
                                                                                       sourceProductDesc,
                                                                                       targetProduct,
                                                                                       targetProductDesc);
            });
        }

    }

}
