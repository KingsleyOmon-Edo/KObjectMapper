namespace ObjectMapper.Abstractions
{
    public interface IMapper
    {
        void Map(object source, object target);
        void Map<TSource, TTarget>(TSource source, TTarget target);
        void MapFrom(object source, object target);
        void MapFrom<TSource, TTarget>(TSource source, TTarget target);
        void MapTo(object source, object target);
        void MapTo<TSource, TTarget>(TSource source, TTarget target);
    }
}