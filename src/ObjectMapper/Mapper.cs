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

        //public void MapFrom<TSource, TTarget>(TSource source, TTarget target)
        //{
        //    Checker.CoalescedNullCheck(source);
        //    Checker.CoalescedNullCheck(target);

        //    Checker.TypeCheck(source);
        //    Checker.TypeCheck(target);

        //    _mappingService.ApplyDiffs(source, target);
        //}

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

        public IEnumerable<TTarget> Map<TSource, TTarget>(IEnumerable<TSource> source, IEnumerable<TTarget> target)
            where TTarget : new()
            where TSource : new()
        {
            source = source ?? throw new ArgumentNullException(nameof(source));
            target = target ?? throw new ArgumentNullException(nameof(target));

            var resultCollection = new List<TTarget>();

            foreach (var sourceElement in source)
            {
                var targetElem = new TTarget();
                _mappingService.ApplyDiffs(sourceElement, targetElem);

                resultCollection.Add(targetElem);
            }

            return resultCollection;
        }

        public static Mapper Create() => new();
    }
}