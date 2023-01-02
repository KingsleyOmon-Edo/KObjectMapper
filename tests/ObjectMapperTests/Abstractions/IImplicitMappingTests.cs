namespace ObjectMapperTests.Abstractions
{
    public interface IImplicitMappingTests
    {
        void Implicit_forward_mapping_via_extensions_from_a_Customer_entity_to_a_customer_Dto_should_succeed();

        void
            Implicit_reverse_mapping_via_extension_methods_from_a_customerDto_back_to_a_Customer_entity_should_succeed();

        void
            Implicit_forward_mapping_via_extension_methods_of_any_two_dissimilar_types_should_succeed();

        void Implicit_reverse_mapping_via_extension_methods_of_any_two_dissimilar_types_should_succeed();

        void
            Passing_a_null_source_object_in_implicit_mapping_via_extension_method_should_throw_ArgumentNullException();

        void
            Passing_a_null_target_object_in_implicit_mapping_via_extension_method_should_throw_ArgumentNullException();
    }
}