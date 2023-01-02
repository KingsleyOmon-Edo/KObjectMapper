namespace ObjectMapper
{
    using Extensions;
    using Helpers;

    public class Mapper
    {
        private Mapper()
        {
        }

        public static Mapper Create() => new();

        public void Map<TCommon>(TCommon source, TCommon target)
        {
            source = Checker.NullChecks(source, target);

            MappingService.Create().ApplyDiffs(source, target);
        }

        public void MapEx(object source, object target)
        {
            Checker.NullChecks(source, target);

            MappingService.Create().ApplyDiffs(source, target);
        }

        public void MapFrom<TCommon>(TCommon source, TCommon target)
        {
            Checker.NullChecks(source, target);

            MappingService.Create().ApplyDiffs(target, source);
        }

        public void MapFrom(object source, object target)
        {
            Checker.NullChecks(source, target);

            MappingService.Create().ApplyDiffs(target, source);
        }

        public void MapTo<TCommon>(TCommon source, TCommon target)
        {
            Checker.NullChecks(source, target);

            MappingService.Create().ApplyDiffs(source, target);
        }

        public void Map(object source, object target)
        {
            Checker.NullChecks(source, target);

            MappingService.Create().ApplyDiffs(target, source);
        }

        public IEnumerable<TCommon> MapFrom<TCommon>(IEnumerable<TCommon> source)
            where TCommon : new()
        {
            var resultCollection = new List<TCommon>();

            foreach (var sourceElem in source)
            {
                var targetElem = new TCommon();
                sourceElem.MapTo(targetElem);

                resultCollection.Add(targetElem);
            }

            return resultCollection;
        }
    }
}