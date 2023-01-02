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

            source.ApplyDiffsTo(target);
        }

        public void MapEx(object source, object target)
        {
            Checker.NullChecks(source, target);

            source.ApplyDiffsTo(target);
        }

        public void MapFrom<TCommon>(TCommon source, TCommon target)
        {
            Checker.NullChecks(source, target);

            target.ApplyDiffsTo(source);
        }

        public void MapFrom(object source, object target)
        {
            Checker.NullChecks(source, target);

            target.ApplyDiffsTo(source);
        }

        public void MapTo<TCommon>(TCommon source, TCommon target)
        {
            Checker.NullChecks(source, target);

            source.ApplyDiffsTo(target);
        }

        public void MapTo(object source, object target)
        {
            Checker.NullChecks(source, target);

            target.ApplyDiffsTo(source);
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