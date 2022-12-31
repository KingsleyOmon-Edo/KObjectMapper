using FluentAssertions;
using ObjectMapper;
using ObjectMapperTests.Helpers;

namespace ObjectMapperTests;

public class MapToIEnumerableOfTTests
{
    //  ToDo: MapToEnIEnumerable: By TDD implement guard clauses for source collection.
    //  Todo: MapToEnIEnumerable: By TDd implement guard clauses for target collection.
    //  ToDo: MapToEnIEnumerable: By TDD validate that source and target objects are IEnumerable<T> derived instances.


    [Fact]
    public void Mapping_from_one_IEnumerableOfT_to_another_should_succeed()
    {
        var sut = Mapper.Create();
        var sourceCollection = new List<Product>()
        {
            new Product { Id = 0, Description = "Tennis Racket", Price = 25.00M, Quantity = 1 },
            new Product { Id = 1, Description = "Golf Ball", Price = 10.00M, Quantity = 4 },
            new Product { Id = 2, Description = "Laptop Computer", Price = 1_000.00M, Quantity = 2 }
        };

        var targetCollection = new List<Product>();

        targetCollection = sut.MapFrom<Product>(sourceCollection).ToList();

        targetCollection.Count.Should().BeGreaterThan(0);
        targetCollection.Count.Should().Be(sourceCollection.Count);
        targetCollection[0].Id.Should().Be(sourceCollection[0].Id);
        targetCollection.Should().BeOfType<List<Product>>();

    }
}