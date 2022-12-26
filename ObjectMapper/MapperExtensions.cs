namespace ObjectMapper
{
    public static class MapperExtensions
    {
        public static void MapFrom<T>(this T source, T target)
        {
            var mapper = new Mapper();
            mapper.Map(target, source);
        }

        public static void MapTo<T>(this T source, T target)
        {
            var mapper = new Mapper();
            mapper.Map(source, target);
        }
    }
}
