﻿namespace ObjectMapperTests
{
    using FluentAssertions;
    using ObjectMapper;
    using ObjectMapperTests.Helpers;
    using System.Net.WebSockets;
    using Xunit;
    public class MapTests
    {
        private readonly CommonAsserts _commonAsserts;

        public MapTests()
        {
            _commonAsserts = CommonAsserts.Create();
        }

        [Fact]
        public void Crate_a_valid_mapper_class()
        {
            var sut = Mapper.Create();

            sut.Should().NotBeNull();
        }

        [Fact]
        public void Mapping_between_two_objects_of_the_same_type_succeeds()
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
            _commonAsserts.AssertSimilarCustomers(firstCustomer, secondCustomer);
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
            _commonAsserts.AssertSimilarCustomers(sourceCustomer, targetCustomer);
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
        public void Mapping_between_two_objects_of_the_same_type_should_succeed()
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
            _commonAsserts.AssertSimilarCustomers(firstCustomer, secondCustomer);
        }

        [Fact]
        public void Passing_a_null_source_object_should_throw_an_ArgumentNullException()
        {
            var sut = Mapper.Create();

            Product sourceProduct = null;
            Product targetProduct = ObjectMother.SampleProduct;

            Assert.Throws<ArgumentNullException>(() =>
               {
                   sut.Map(sourceProduct as object, targetProduct as object);
               });

        }

        
    }
}
