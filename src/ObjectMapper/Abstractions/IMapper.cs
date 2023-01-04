namespace ObjectMapper.Abstractions
{
    public interface IMapper
    {
        void MapTo(object source, object target);
        void MapFrom(object source, object target);
        void Map(object source, object target);
    }
}