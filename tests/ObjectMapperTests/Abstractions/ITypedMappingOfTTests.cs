namespace ObjectMapperTests.Abstractions
{
    public interface ITypedMappingOfTTests
    {
        void Passing_a_source_object_incompatible_with_the_source_type_parameter_should_fail();
        void Pass_a_target_object_incompatible_with_the_target_type_parameter_should_fail();
    }
}