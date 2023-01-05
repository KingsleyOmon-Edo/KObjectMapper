// ReSharper disable All

namespace ObjectMapper.Extensions
{
    using Helpers;

    public static class MapperExtensions
    {
        public static void MapTo(this object source, object target)
        {
            Checker.NullCheckAll(source, target);

            var mapper = new Mapper();
            mapper.MapTo(source, target);
        }

        public static void MapFrom(this object source, object target)
        {
            Checker.NullCheckAll(source, target);

            var mapper = new Mapper();
            mapper.MapFrom(source, target);
        }

        public static void MapTo<TTarget>(this object source, TTarget target)
        {
            //var mapper = Mapper.Create();
            //mapper.MapTo(source, target);
            ////mapper.Map(source, target);


            // Null check both
            Checker.CoalescedNullCheck<object>(source);
            Checker.CoalescedNullCheck<TTarget>(target);

            //  Type check TTarget only
            Checker.TypeCheck<TTarget>(target);

            var svc = MappingService.Create();
            svc.ApplyDiffs(source, target);
        }

        public static void MapFrom<TSource>(this object target, TSource source)
        {
            //  Null check both
            Checker.CoalescedNullCheck<TSource>(source);
            Checker.CoalescedNullCheck<object>(target);

            //  Type check only the source object
            Checker.TypeCheck<TSource>(source);

            var svc = MappingService.Create();
            svc.ApplyDiffs(source, target);
        }

        public static IEnumerable<TTarget> MapFrom<TSource, TTarget>(this IEnumerable<TTarget> target,
            IEnumerable<TSource> source)
            where TTarget : new()
        {
            var resultCollection = new List<TTarget>();

            foreach (var sourceElem in source)
            {
                var targetElem = new TTarget();
                sourceElem.MapTo(targetElem);

                resultCollection.Add(targetElem);
            }

            return resultCollection;
        }
    }
}