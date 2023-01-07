namespace KObjectObjectMapper.Abstractions
{
    public interface IObjectMapper
    {
        void MapTo(object source, object target);
        void MapTo<TSource, TTarget>(TSource source, TTarget target);
        void MapFrom(object source, object target);
        void MapFrom<TSource, TTarget>(TSource source, TTarget target);
        void Map(object source, object target);
        void Map<TSource, TTarget>(TSource source, TTarget target);

        IEnumerable<TTarget> Map<TSource, TTarget>(IEnumerable<TSource> source, IEnumerable<TTarget> target)
            where TTarget : new()
            where TSource : new();
    }
}