namespace ObjectMapperTests.Abstractions
{
    public interface ITypedMappingTest
    {
        void
            Passing_a_null_target_object_in_explicit_mapping_via_mapper_instance_throws_ArgumentNullException_when_using_a_mapper_instance();

        void Passing_a_source_object_incompatible_with_the_source_type_parameter_should_fail();
    }
}