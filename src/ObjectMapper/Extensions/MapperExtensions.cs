namespace ObjectMapper.Extensions
{
    using Helpers;

    public static class MapperExtensions
    {
        //public static void MapFrom<TCommon>(this TCommon source, TCommon target)
        //{
        //    source = source ?? throw new ArgumentNullException(nameof(source));
        //    target = target ?? throw new ArgumentNullException(nameof(target));

        //    var mapper = Mapper.Create();
        //    mapper.Map(target, source);
        //}

        //public static void MapTo<TCommon>(this TCommon source, TCommon target)
        //{
        //    Checker.NullCheckAll<TCommon>();

        //    var mapper = Mapper.Create();
        //    mapper.Map(source, target);
        //}

        //  ====================================

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