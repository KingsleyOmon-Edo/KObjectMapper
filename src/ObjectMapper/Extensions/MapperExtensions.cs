namespace ObjectMapper.Extensions
{
    using Helpers;

    public static class MapperExtensions
    {
        public static void MapFrom(this object source, object target)
        {
            Checker.NullCheckAll(source, target);

            var mapper = new Mapper();
            mapper.MapFrom(source, target);
        }

        public static void MapTo<TTarget>(this object source, TTarget target)
        {
            //var mapper = Mapper.Create();
            //mapper.Map(source, target);
            //  Mapping.

            var svc = MappingService.Create();
            svc.ApplyDiffs(source, target);
        }
    }
}