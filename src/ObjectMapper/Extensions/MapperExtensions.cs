namespace ObjectMapper.Extensions
{
    using Helpers;

    public static class MapperExtensions
    {
        public static void MapTo(this object source, object target)
        {
            Checker.NullCheckAll(source, target);

            var mapper = new Mapper();
            mapper.Map(source, target);
        }

        public static void MapFrom(this object source, object target)
        {
            Checker.NullCheckAll(source, target);

            var mapper = new Mapper();
            mapper.MapFrom(source, target);
        }
    }
}