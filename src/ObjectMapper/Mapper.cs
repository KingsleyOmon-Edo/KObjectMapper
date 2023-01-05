namespace ObjectMapper
{
    using Abstractions;
    using Helpers;

    public class Mapper : IMapper
    {
        private readonly MappingService _mappingService = MappingService.Create();

        public Mapper()
        {
        }

        public void MapTo(object source, object target)
        {
            Checker.NullChecks(source, target);

            Map(source, target);
        }

        public void MapTo<TSource, TTarget>(TSource source, TTarget target)
        {
            Checker.CoalescedNullCheck(source);
            Checker.CoalescedNullCheck(target);

            Checker.TypeCheck(source);
            Checker.TypeCheck(target);

            _mappingService.ApplyDiffs(source, target);
        }

        public void MapFrom(object source, object target)
        {
            Checker.NullChecks(source, target);

            Map(target, source);
        }

        public void MapFrom<TSource, TTarget>(TSource source, TTarget target)
        {
            Checker.CoalescedNullCheck(source);
            Checker.CoalescedNullCheck(target);

            Checker.TypeCheck(source);
            Checker.TypeCheck(target);

            _mappingService.ApplyDiffs(source, target);
        }

        public void Map(object source, object target)
        {
            Checker.NullChecks(source, target);

            _mappingService.ApplyDiffs(source, target);
        }

        public void Map<TSource, TTarget>(TSource source, TTarget target)
        {
            //  Null checks
            Checker.CoalescedNullCheck(source);
            Checker.CoalescedNullCheck(target);

            //  Type checks
            Checker.TypeCheck(source);
            Checker.TypeCheck(target);

            //  Mapping.
            _mappingService.ApplyDiffs(source, target);
        }

        //public void MapTo<TCommon>(TCommon source, TCommon target)
        //{
        //    Checker.NullChecks(source, target);

        //    MappingService.Create().ApplyDiffs(source, target);
        //}

        //public IEnumerable<TCommon> MapFrom<TCommon>(IEnumerable<TCommon> source)
        //    where TCommon : new()
        //{
        //    var resultCollection = new List<TCommon>();

        //    foreach (var sourceElem in source)
        //    {
        //        var targetElem = new TCommon();
        //        sourceElem.MapTo(targetElem);

        //        resultCollection.Add(targetElem);
        //    }

        //    return resultCollection;
        //}

        public static Mapper Create() => new();
    }
}