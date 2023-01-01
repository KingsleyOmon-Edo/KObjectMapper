namespace ObjectMapperTests;

public class TaskList
{
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

}