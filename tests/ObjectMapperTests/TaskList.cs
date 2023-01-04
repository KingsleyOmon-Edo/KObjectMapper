namespace ObjectMapperTests
{
    public class TaskList
    {
        //  ToDo: Refactoring - Explicit dependencies principle

        //  INVOCATION TARGETS
        //  ---------------------
        //  "MapFrom" specific source type to a specific "TARGET" type
        //  ToDo: public void MapTo<TSource, TTarget(TSource source, TTarget target);

        //  Specific to specific. Which may or may not be the same.
        //  ToDo: public void MapFrom<TSource, TTarget>(TSource source, TTarget target);


        //  ALIAS METHODS
        //  ---------------------------------
        //  "MapTO" a specific type from any arbitrary object.
        //  ToDo: public void MapTo<TTarget>(this object source, TTarget target);

        //  "MapFROM" any arbitrary object to a specified target type
        //  ToDo: public TSource MapFrom<TSource>(this object source, TSource source);


        //  COLLECTION MAPPING
        //  -----------------------
        //  Specific to specific
        //  Todo: public void MapTo<TTarget>(this IEnumerable<TSource> source, IEnumerable<TTarget> target);
        //  Todo: public void MapFrom<TSource>(this IEnumerable<TSource> source, IEnumerable<TTarget> target);


        //  --------------------------
        //  Todo: Before tacking details of immutable types, given that 
        //  classes and Dto are working correctly, try to put together
        //  a test API that consumes that increment (slice of funtionality)
        //  Fix and debug any issues arising to confirm that classes and Dtos
        //  are working ok

        //  ANALYSIS
        //  --------
        //  A method built for tow dissimilar types will also work for the case
        //  where both types are the same but with different property states
        //  The assumption could be made that dissimilar types are a superset of
        //  similar types, as they subsume similar types. Hence,

        //          MapTo<TSource, TTarget>(TSource source, TTarget target) 

        //  will also return the correct computations for when 
        //  TSource and TTarget are the same. ie.

        //          TSource == TTarget

        //  So one could focus and building and testing just the dissimilar case.


        //  ===================================================
        //  ToDo: MapToEnIEnumerable: By TDD implement guard clauses for source collection.
        //  Todo: MapToEnIEnumerable: By TDd implement guard clauses for target collection.
        //  ToDo: MapToEnIEnumerable: By TDD validate that source and target objects are IEnumerable<T> derived instances.

        //- Test guard clauses
    }
}