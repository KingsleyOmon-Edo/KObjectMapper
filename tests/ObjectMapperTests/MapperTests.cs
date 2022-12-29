namespace ObjectMapperTests
{
    using FluentAssertions;
    using ObjectMapper;
    using ObjectMapperTests.Helpers;
    using System.Net.WebSockets;
    using Xunit;
    public class MapperTests
    {
        [Fact]
        public void Crate_a_valid_mapper_class()
        {
            var sut = Mapper.Create();

            sut.Should().NotBeNull();
        }

        [Fact]
        public void Mappping_between_two_objects_of_the_same_type_succeeds()
        {
            //  Arrange
            var sut = Mapper.Create();
            var firstCustomer = ObjectMother.SampleCustomer;
            var secondCustomer = new Customer
            {
                Id = 5,
                FirstName = "Jane",
                LastName = "Doe",
                PhoneNumber = "5666655598"
            };

            //  Act
            sut.Map<Customer>(firstCustomer, secondCustomer);

            //  Assert
            AssertSimilarCustomers(firstCustomer, secondCustomer);
        }

        [Fact]
        public void Mapping_with_the_MapToOfT_overload_should_succeed()
        {
            var sut = Mapper.Create();
            var sourceProduct = ObjectMother.SampleProduct;
            var targetProduct = new Product
            {
                Id = 11,
                Description = "Good lawn mower",
                Price = 200.00M,
                Quantity = 2
            };


            sut.MapTo<Product>(sourceProduct, targetProduct);
            AssertSimilarProducts(sourceProduct, targetProduct);
        }

        [Fact]
        public void Mapping_with_the_MapFromOfT_overload_should_sceed()
        {
            //  Arrange
            var sut = Mapper.Create();
            var sourceCustomer = ObjectMother.SampleCustomer;
            var targetCustomer = new Customer
            {
                Id = 100,
                FirstName = "Fred",
                LastName = "Mark",
                PhoneNumber = "9305558973"
            };

            //  Act         
            sut.MapFrom<Customer>(sourceCustomer, targetCustomer);

            //  Assert
            AssertSimilarCustomers(sourceCustomer, targetCustomer);
        }

        [Fact]
        public void Mapping_via_Map_with_type_inference_should_succeed()
        {
            var sut = Mapper.Create();
            var sourceCustomer = ObjectMother.SampleCustomer;
            var targetCustomer = new Customer
            {
                Id = 55,
                FirstName = "Remko",
                LastName = "Zingi",
                PhoneNumber = "7075554434"
            };

            //  Act
            sut.Map(sourceCustomer, targetCustomer);


            //  Assert            
            AssertSimilarCustomers(sourceCustomer, targetCustomer);
        }

        private void AssertSimilarProducts(Product sourceProduct, Product targetProduct)
        {
            targetProduct.Should().NotBeNull();
            targetProduct.Should().BeOfType<Product>();
            targetProduct.Should().BeAssignableTo<Product>();

            targetProduct.Id.Should().Be(sourceProduct.Id);
            targetProduct.Description.Should().Be(sourceProduct.Description);
            targetProduct.Quantity.Should().Be(sourceProduct.Quantity);
            targetProduct.Price.Should().Be(sourceProduct.Price);
        }
        private void AssertSimilarCustomers(Customer sourceCustomer, Customer targetCustomer)
        {
            targetCustomer.Should().NotBeNull();
            targetCustomer.Should().BeOfType<Customer>();
            targetCustomer.Should().BeEquivalentTo<Customer>(sourceCustomer);
            targetCustomer.Id.Should().Be(sourceCustomer.Id);
            targetCustomer.FirstName.Should().Be(sourceCustomer.FirstName);
            targetCustomer.LastName.Should().Be(sourceCustomer.LastName);
            targetCustomer.PhoneNumber.Should().Be(sourceCustomer.PhoneNumber);
        }

        [Fact]
        public void Mapping_with_MapTo_via_type_inference_should_succeed()
        {
            var sut = Mapper.Create();
            var sourceProduct = ObjectMother.SampleProduct;
            var targetProduct = new Product
            {
                Id = 122,
                Description = "Tenis racket",
                Price = 25.00M,
                Quantity = 5
            };

            sut.MapTo(sourceProduct, targetProduct);

            AssertSimilarProducts(sourceProduct, targetProduct);
        }

        [Fact]
        public void Mapping_with_MapFromOfT_via_type_inference_should_succeed()
        {
            var sut = Mapper.Create();
            var sourceCustomer = ObjectMother.SampleCustomer;
            var targetCustomer = new Customer
            {
                Id = 2_000,
                FirstName = "Epicurus",
                LastName = "Osemede",
                PhoneNumber = "4027771115"
            };

            sut.MapFrom(sourceCustomer, targetCustomer);

            AssertSimilarCustomers(sourceCustomer, targetCustomer);
        }

        [Fact]
        public void Invoking_MapToOfT_on_an_object_without_a_mapper_instance_succeeds()
        {
            var sourceProduct = ObjectMother.SampleProduct;
            var targetProduct = new Product
            {
                Id = 38,
                Description = "Nice book",
                Price = 30.00M,
                Quantity = 1
            };

            sourceProduct.MapTo<Product>(targetProduct);

            AssertSimilarProducts(sourceProduct, targetProduct);
        }

        [Fact]
        public void MapToOfT_should_work_when_target_is_a_default_instance()
        {
            var sourceCustomer = ObjectMother.SampleCustomer;
            var targetCustomer = new Customer();

            sourceCustomer.MapTo<Customer>(targetCustomer);

            AssertSimilarCustomers(sourceCustomer, targetCustomer);
        }

        [Fact]
        public void Invoking_MapOfT_should_work_wnen_source_obhject_is_a_new_default_instance()
        {
            var sourceProduct = new Product();
            var targetProduct = ObjectMother.SampleProduct;

            sourceProduct.MapTo<Product>(targetProduct);

            AssertSimilarProducts(sourceProduct, targetProduct);
            AssertSimilarProducts(targetProduct, sourceProduct);
        }

        [Fact]
        public void Calling_MapFromOfT_on_a_satisfactory_object_without_mapper_should_succeed()
        {
            var sourceCustomer = ObjectMother.SampleCustomer;
            var targetCustomer = new Customer
            {
                Id = 500,
                FirstName = "Sean",
                LastName = "Daniels",
                PhoneNumber = "1234567890",
            };

            targetCustomer.MapFrom<Customer>(sourceCustomer);

            AssertSimilarCustomers(targetCustomer, sourceCustomer);
            AssertSimilarCustomers(sourceCustomer, targetCustomer);

        }

        [Fact]
        public void Invoking_MapFromOfT_on_a_default_object_instance_should_work()
        {
            var sourceProduct = ObjectMother.SampleProduct;
            var targetProduct = new Product();

            targetProduct.MapFrom<Product>(sourceProduct);

            AssertSimilarProducts(sourceProduct, targetProduct);
            AssertSimilarProducts(targetProduct, sourceProduct);
        }

        [Fact]
        public void Type_inference_should_work_for_MapToOfT()
        {
            var sourceProduct = ObjectMother.SampleProduct;
            var targetProduct = new Product
            {
                Id = 21,
                Description = "Headphones for everyone",
                Price = 85.00M,
                Quantity = 1
            };

            sourceProduct.MapTo(targetProduct);

            AssertSimilarProducts(sourceProduct, targetProduct);
        }

        [Fact]
        public void Type_inference_should_work_for_MapFromOfT()
        {
            var sourceCustomer = ObjectMother.SampleCustomer;
            var targetCustomer = new Customer();

            targetCustomer.MapFrom(sourceCustomer);

            AssertSimilarCustomers(sourceCustomer, targetCustomer);
        }

        [Fact]
        public void Mapper_should_successfully_map_any_compatible_properties_between_two_objects_of_different_types()
        {
            //  Arrange
            var sut = Mapper.Create();
            var customerJane = ObjectMother.SampleCustomer;
            var employeeSam = new Employee
            {
                EmployeeId = 10,
                FirstName = "Sam",
                LastName = "Williams",
                Salary = 100_000.00M,
                HireDate = new DateTimeOffset(2021, 3, 15, 6, 30, 25, TimeSpan.Zero)
            };

            // Act
            sut.Map(customerJane, employeeSam);

            //  Assert
            employeeSam.Should().NotBeNull();
            employeeSam.EmployeeId.Should().Be(10);
            employeeSam.EmployeeId.Should().NotBe(customerJane.Id);
            employeeSam.FirstName.Should().Be(customerJane.FirstName);
            employeeSam.LastName.Should().Be(customerJane.LastName);
            employeeSam.Salary.Should().Be(100_000.00M);
        }      

        [Fact]
        public void Nongeneric_MapTo_should_corrctly_map_compatible_props_of_two_objects_even_of_different_types()
        {
            var sut = Mapper.Create();
            var testCustomer = ObjectMother.SampleCustomer;
            var testEmployee = ObjectMother.SampleEmployee;

            sut.MapTo(testEmployee, testCustomer);

            testCustomer.Should().NotBeNull();
            testCustomer.FirstName.Should().Be(testEmployee.FirstName);
            testCustomer.LastName.Should().Be(testEmployee.LastName);
            testCustomer.Id.Should().NotBe(testEmployee.EmployeeId);
        }

        [Fact]
        public void Nongeneric_MapFrom_should_correctly_map_compatible_props_of_two_dissimilar_objects()
        {
            var sut = Mapper.Create();
            var sampleEmployee = ObjectMother.SampleEmployee;
            var sampleCustomer = ObjectMother.SampleCustomer;

            sut.MapFrom(sampleCustomer, sampleEmployee);

            sampleEmployee.Should().NotBeNull();
            sampleEmployee.EmployeeId.Should().NotBe(sampleCustomer.Id);
            sampleEmployee.FirstName.Should().Be(sampleCustomer.FirstName);
            sampleEmployee.LastName.Should().Be(sampleCustomer.LastName);
            sampleEmployee.Salary.Should().Be(100_000.00M);
        }

        [Fact]
        public void Nongeneric_MapTo_should_be_available_as_an_extension_on_any_object_with_no_need_for_mapper_instance()
        {
            var sampleCustomer = ObjectMother.SampleCustomer;
            var sampleEmployee = ObjectMother.SampleEmployee;

            sampleCustomer.MapTo(sampleEmployee);

            sampleEmployee.Should().NotBeNull();
            sampleEmployee.EmployeeId.Should().NotBe(sampleCustomer.Id);
            sampleEmployee.FirstName.Should().Be(sampleCustomer.FirstName);
            sampleEmployee.LastName.Should().Be(sampleCustomer.LastName);
            sampleEmployee.Salary.Should().Be(100_000.00M);

        }

        [Fact]
        public void Nongeneric_MapFrom_should_be_available_as_extension_method_on_any_object_without_a_need_for_a_mapper_instance()
        {
            var testCustomer = ObjectMother.SampleCustomer;
            var testEmployee = ObjectMother.SampleEmployee;

            testCustomer.MapFrom(testEmployee);

            testCustomer.Should().NotBeNull();
            testCustomer.Id.Should().NotBe(testEmployee.EmployeeId);
            testCustomer.FirstName.Should().Be(testEmployee.FirstName);
            testCustomer.LastName.Should().Be(testEmployee.LastName);           
        }
    }
}
