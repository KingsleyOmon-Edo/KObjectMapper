namespace ObjectMapperTests
{
    public class TaskList
    {
        //  TEST SCENARIOS
        //  -----------------
        //  Implicit 
        //  - Implicit forward map
        //  - Implicit reverse map
        //  - Implicit similar map
        //  - Implicit dissimilar

        //  Explicit
        //  - Explicit forward map
        //  - Explicit reverse map
        //  - Explicit similar map
        //  - Explicit dissimilar map

        //  --------------------------

        //  Todo: Before tacking details of immutable types, given that 
        //  classes and Dto are working correctly, try to put together
        //  a test API that consumes that increment (slice of funtionality)
        //  Fix and debug any issues arising to confirm that classes and Dtos
        //  are working ok

        //  ToDo: REFACTOR: Push some of ObjectExtension methods into a service class and consume by object composition.

        //  DISSIMILAR TYPES - SINGLE OBJECTS
        //  ---------------------------------
        //  "MapTO" a specific type from any arbitrary object.
        //  ToDo: public void MapTo<TTarget>(this object source, TTarget target);

        //  "MapTO" a specific type from any arbitrary object.
        //  ToDo: public void MapTo<TTarget>(this object source, TTarget target);

        //  "MapTO" any arbitrary object from a specific source type
        //  ToDo: public void MapTo<TSource>(this TSource source, object target);

        //  "MapFrom" specific source type to a specific "TARGET" type
        //  ToDo: public void MapTo<TSource, TTarget(this TSource source, TTarget target);

        //  "MapFROM" any arbitrary object to a specified target type
        //  ToDo: public TSource MapFrom<TSource>(this object source, TSource source);

        //  "MapFrom" a specific source type to specific target type
        //  ToDo: public void MapFrom<TSource>(this TSource source, object target);

        //  Specific to specific. Which may or may not be the same.
        //  ToDo: public void MapFrom<TSource, TTarget>(TSource source, TTarget target);


        //  COMMON TYPES - SINGLE OBJECTS
        //  ------------------------------
        //  "MapTO" a common specific type for source and target
        // ToDo: public void MapTo<TCommon>(this TCommon source, TCommon target);

        //  "MapFrom" common specific types for source and target
        //  ToDo: public void MapFrom(this TCommon source, TCommon target);


        //  COMMON TYPES - COLLECTIONS
        //  --------------------------
        //  "MapTo" a common collection type
        //  ToDo: public void MapTo<TCommon>(this IEnumerable<TCommon> source, IEnumerable<TCommon>(target));

        //  "MapFrom" common specific type from source to target.
        //  ToDo: public void MapFrom<TCommon>(this IEnumerable<TCommon> source, IEnumerable<TCommon> target);

        //  DISSIMILAR TYPES - COLLECTIONS
        //  -------------------------------
        //  Specific to specific
        //  Todo: public void MapTo<TSource, TTarget>(this IEnumerable<TSource> source, IEnumerable<TTarget> target);

        //  "MapTo" from any type to a specific collection type 
        //  ToDo: public void MapTo(this IEnumerable<object> source, IEnumerable<TTarget> target);

        //  "MapTo" arbitrary from a specific type
        //  ToDo: public void MapTo(this IEnumerable<TSource> source, IEnumerable<object> target);

        //  =====================================================
        //  ToDo: Validate types of source objects
        //  ToDo: Validate types of target objects
        //  ToDo: Validate both types when different types are supplied for each Map<TSource, TTarget>(TSource source, TTarget target)
        //  ToDo: Validate the type ignorance still works.
        //  ToDo: POC: Setup a sample, install the nuget and test. Fix any issues.

        //  ===================================================
        //  ToDo: MapToEnIEnumerable: By TDD implement guard clauses for source collection.
        //  Todo: MapToEnIEnumerable: By TDd implement guard clauses for target collection.
        //  ToDo: MapToEnIEnumerable: By TDD validate that source and target objects are IEnumerable<T> derived instances.

        //- Test guard clauses
    }
}