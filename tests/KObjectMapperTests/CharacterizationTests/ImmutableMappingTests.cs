using System.Reflection;
using KObjectMapper;
using KObjectMapperTests.Helpers;
using Shouldly;

namespace KObjectMapperTests.CharacterizationTests;

public class ImmutableMappingTests
{
    [Fact]
    public void Mapping_from_immutable_source_to_new_immutable_target_should_succeed()
    {
        Mapper sut = Mapper.Create();
        ImmutableCustomerSource source = new(10, "James", "5555550009");

        MethodInfo mapMethod = typeof(Mapper)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Single(method =>
                method.Name == nameof(Mapper.Map) &&
                method.IsGenericMethodDefinition &&
                method.GetGenericArguments().Length == 2 &&
                method.GetParameters().Length == 1 &&
                method.GetParameters()[0].ParameterType.IsGenericParameter);

        object? mappedObject = mapMethod.MakeGenericMethod(typeof(ImmutableCustomerSource), typeof(ImmutableCustomerDestination))
            .Invoke(sut, [source]);

        mappedObject.ShouldNotBeNull();

        ImmutableCustomerDestination mapped = mappedObject.ShouldBeOfType<ImmutableCustomerDestination>();
        mapped.Id.ShouldBe(source.Id);
        mapped.FirstName.ShouldBe(source.FirstName);
        mapped.PhoneNumber.ShouldBe(source.PhoneNumber);
    }

    [Fact]
    public void Mapping_collection_of_immutable_source_to_new_immutable_target_should_succeed()
    {
        Mapper sut = Mapper.Create();
        List<ImmutableCustomerSource> sources =
        [
            new ImmutableCustomerSource(10, "James", "5555550009"),
            new ImmutableCustomerSource(11, "Anne", "5555550010")
        ];

        MethodInfo mapMethod = typeof(Mapper)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Single(method =>
                method.Name == nameof(Mapper.Map) &&
                method.IsGenericMethodDefinition &&
                method.GetGenericArguments().Length == 2 &&
                method.GetParameters().Length == 1 &&
                method.GetParameters()[0].ParameterType.IsGenericType &&
                method.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        object? mappedObject = mapMethod.MakeGenericMethod(typeof(ImmutableCustomerSource), typeof(ImmutableCustomerDestination))
            .Invoke(sut, [sources]);

        mappedObject.ShouldNotBeNull();

        IEnumerable<ImmutableCustomerDestination> mapped = mappedObject.ShouldBeAssignableTo<IEnumerable<ImmutableCustomerDestination>>()!;
        List<ImmutableCustomerDestination> mappedList = mapped.ToList();

        mappedList.Count.ShouldBe(2);
        mappedList[0].Id.ShouldBe(sources[0].Id);
        mappedList[0].FirstName.ShouldBe(sources[0].FirstName);
        mappedList[0].PhoneNumber.ShouldBe(sources[0].PhoneNumber);
        mappedList[1].Id.ShouldBe(sources[1].Id);
        mappedList[1].FirstName.ShouldBe(sources[1].FirstName);
        mappedList[1].PhoneNumber.ShouldBe(sources[1].PhoneNumber);
    }
}
