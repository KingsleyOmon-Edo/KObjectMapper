namespace ObjectMapper
{
    using System;

    public class Mapper
    {
        private Mapper()
        {
        }

        public static Mapper Create()
        {
            return new Mapper();
        }

        public void Map<T>(T source, T target)
        {
            source = source ?? throw new ArgumentNullException(nameof(source));
            target = target ?? throw new ArgumentNullException(nameof(target));

            source.ApplyDiffsTo<T>(target);
        }


        public IEnumerable<T> MapFrom<T>(IEnumerable<T> source)
            where T: new()
        {
            var resultCollection = new List<T>();

            foreach (var sourceElem in source)
            {
                var targetElem = new T();
                sourceElem.MapTo(targetElem);

                resultCollection.Add(targetElem);
            }

            return resultCollection;

        }


        public void Map(object source, object target)
        {
            source = source ?? throw new ArgumentNullException(nameof(source));
            target = target ?? throw new ArgumentNullException(nameof(target));

            source.ApplyDiffsTo(target);
        }

        public void MapFrom<T>(T source, T target)
        {
            target.ApplyDiffs<T>(source);
        }

        public void MapFrom(object source, object target)
        {
            source = source ?? throw new ArgumentNullException(nameof(source));
            target = target ?? throw new ArgumentNullException(nameof(target));
            
            target.ApplyDiffsTo(source);
        }

        public void MapTo<T>([DisallowNull] T source, T target)
        {
            source = source ?? throw new ArgumentNullException(nameof(source));
            target = target ?? throw new ArgumentNullException(nameof(target));
            
            source.ApplyDiffsTo(target);
        }

        public void MapTo(object source, object target)
        {
            target.ApplyDiffs(source);
        }
    }
}
