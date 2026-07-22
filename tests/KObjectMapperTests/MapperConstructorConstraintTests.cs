using System.Reflection;
using KObjectMapper;
using Shouldly;

namespace KObjectMapperTests;

public class MapperConstructorConstraintTests
{
    [Fact]
    public void Map_SingleObjectCreationOverload_DoesNotDeclareNewConstraintOnTargetType()
    {
        MethodInfo method = typeof(Mapper)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Single(candidate =>
                candidate.Name == nameof(Mapper.Map) &&
                candidate.IsGenericMethodDefinition &&
                candidate.GetGenericArguments().Length == 2 &&
                candidate.GetParameters().Length == 1 &&
                candidate.GetParameters()[0].ParameterType.IsGenericParameter);

        GenericParameterAttributes targetAttributes = method.GetGenericArguments()[1].GenericParameterAttributes;

        targetAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint).ShouldBeFalse();
    }

    [Fact]
    public void Map_CollectionCreationOverload_DoesNotDeclareNewConstraintOnTargetType()
    {
        MethodInfo method = typeof(Mapper)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Single(candidate =>
                candidate.Name == nameof(Mapper.Map) &&
                candidate.IsGenericMethodDefinition &&
                candidate.GetGenericArguments().Length == 2 &&
                candidate.GetParameters().Length == 1 &&
                candidate.GetParameters()[0].ParameterType.IsGenericType &&
                candidate.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        GenericParameterAttributes targetAttributes = method.GetGenericArguments()[1].GenericParameterAttributes;

        targetAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint).ShouldBeFalse();
    }
}
