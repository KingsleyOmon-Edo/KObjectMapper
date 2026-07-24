using KObjectMapper;
using KObjectMapper.UnitTests.Helpers;
using Shouldly;

namespace KObjectMapper.UnitTests;

public class EdgeCaseMappingTests
{
    [Fact]
    public void Map_IntoExistingCollection_ReusesTargetCollectionAndUpdatesItsItems()
    {
        Mapper mapper = Mapper.Create();

        List<CustomerDto> source =
        [
            new CustomerDto
            {
                Id = 10,
                FirstName = "James",
                PhoneNumber = "5555550009"
            }
        ];

        List<Customer> target =
        [
            new Customer
            {
                Id = 1,
                FirstName = "Before",
                LastName = "StillHere",
                PhoneNumber = "0000000000"
            }
        ];

        IEnumerable<Customer> mapped = mapper.Map<CustomerDto, Customer>(source, target);

        mapped.ShouldBeSameAs(target);
        target.Count.ShouldBe(1);
        target[0].Id.ShouldBe(10);
        target[0].FirstName.ShouldBe("James");
        target[0].PhoneNumber.ShouldBe("5555550009");
        target[0].LastName.ShouldBe("StillHere");
    }

    [Fact]
    public void Map_ReadOnlyProperty_DoesNotOverwriteNonWritableProperty()
    {
        Mapper mapper = Mapper.Create();

        ReadOnlySource source = new()
        {
            Name = "Updated",
            Age = 42
        };

        ReadOnlyTarget target = new();

        mapper.Map(source, target);

        target.Name.ShouldBe("Before");
        target.Age.ShouldBe(42);
    }

    [Fact]
    public void Map_SourcePropertyIsNull_SetsTargetPropertyToNull()
    {
        Mapper mapper = Mapper.Create();

        Customer source = new()
        {
            Id = 1,
            FirstName = null,
            LastName = "Omon",
            PhoneNumber = "5555550009"
        };

        CustomerDto target = new()
        {
            Id = 1,
            FirstName = "Before",
            PhoneNumber = "5555550009"
        };

        mapper.Map(source, target);

        target.FirstName.ShouldBeNull();
        target.PhoneNumber.ShouldBe("5555550009");
    }

    [Fact]
    public void Map_TargetPropertyIsNull_OverwritesItWithSourceValue()
    {
        Mapper mapper = Mapper.Create();

        Customer source = new()
        {
            Id = 1,
            FirstName = "James",
            LastName = "Omon",
            PhoneNumber = "5555550009"
        };

        CustomerDto target = new()
        {
            Id = 1,
            FirstName = null,
            PhoneNumber = null
        };

        mapper.Map(source, target);

        target.FirstName.ShouldBe("James");
        target.PhoneNumber.ShouldBe("5555550009");
    }

    [Fact]
    public void Map_NestedObjectProperty_MapsNestedValuesWithoutSharingReferences()
    {
        Mapper mapper = Mapper.Create();

        NestedCustomerSource source = new()
        {
            Name = "Source",
            Address = new Address
            {
                Street = "1 Main Street",
                City = "London"
            }
        };

        NestedCustomerTarget target = new()
        {
            Name = "Before",
            Address = new Address
            {
                Street = "Old Street",
                City = "Paris"
            }
        };

        mapper.Map(source, target);

        target.Name.ShouldBe("Source");
        target.Address.Street.ShouldBe("1 Main Street");
        target.Address.City.ShouldBe("London");
        target.Address.ShouldNotBeSameAs(source.Address);
    }

    [Fact]
    public void Map_CompatibleConvertiblePropertyTypes_ConvertsValues()
    {
        Mapper mapper = Mapper.Create();

        ConvertibleSource source = new()
        {
            Quantity = 42
        };

        ConvertibleTarget target = new();

        mapper.Map(source, target);

        target.Quantity.ShouldBe(42L);
    }

    [Fact]
    public void Map_CollectionTargetIsNotIList_ReturnsNewMappedCollection()
    {
        Mapper mapper = Mapper.Create();

        List<CustomerDto> source =
        [
            new CustomerDto
            {
                Id = 10,
                FirstName = "James",
                PhoneNumber = "5555550009"
            }
        ];

        IEnumerable<Customer> target = new Customer[]
        {
            new Customer
            {
                Id = 1,
                FirstName = "Before",
                LastName = "StillHere",
                PhoneNumber = "0000000000"
            }
        };

        IEnumerable<Customer> mapped = mapper.Map<CustomerDto, Customer>(source, target);
        List<Customer> mappedList = mapped.ToList();

        mapped.ShouldNotBeSameAs(target);
        mappedList.Count.ShouldBe(1);
        mappedList[0].Id.ShouldBe(10);
        mappedList[0].FirstName.ShouldBe("James");
        mappedList[0].PhoneNumber.ShouldBe("5555550009");
    }

    [Fact]
    public void Map_CollectionTargetIsReadOnlyList_ReturnsNewMappedCollectionWithoutMutatingOriginal()
    {
        Mapper mapper = Mapper.Create();

        List<CustomerDto> source =
        [
            new CustomerDto
            {
                Id = 10,
                FirstName = "James",
                PhoneNumber = "5555550009"
            }
        ];

        List<Customer> backingList =
        [
            new Customer
            {
                Id = 1,
                FirstName = "Before",
                LastName = "StillHere",
                PhoneNumber = "0000000000"
            }
        ];

        IReadOnlyList<Customer> readOnlyTarget = backingList.AsReadOnly();

        IEnumerable<Customer> mapped = mapper.Map<CustomerDto, Customer>(source, readOnlyTarget);
        List<Customer> mappedList = mapped.ToList();

        mapped.ShouldNotBeSameAs(readOnlyTarget);
        backingList[0].Id.ShouldBe(1);
        backingList[0].FirstName.ShouldBe("Before");
        mappedList.Count.ShouldBe(1);
        mappedList[0].Id.ShouldBe(10);
        mappedList[0].FirstName.ShouldBe("James");
        mappedList[0].PhoneNumber.ShouldBe("5555550009");
    }

}
