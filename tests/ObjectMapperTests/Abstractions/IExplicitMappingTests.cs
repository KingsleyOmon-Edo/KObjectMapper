namespace ObjectMapperTests.Abstractions
{
    public interface IExplicitMappingTests
    {
        void
            Explicit_forward_mapping_via_mapper_instance_from_Customer_entity_to_a_CustomerDto_should_succeed();

        void
            Explicit_reverse_mapping_via_mapper_instance_from_a_CustomerDto_back_to_a_customer_entity_should_succeed();

        void
            Explicit_forward_mapping_via_mapper_instance_of_any_two_dissimilar_types_via_mapper_instance_should_succeed();

        void Explicit_reverse_mapping_via_mapper_instance_of_any_two_dissimilar_types_should_succeed();

        void
            Passing_a_null_source_object_in_explicit_mapping_via_a_mapper_instance_should_throw_ArgumentNullException();

        public void
            Passing_a_null_target_object_in_explicit_mapping_via_mapper_instance_throws_ArgumentNullException();
    }
}