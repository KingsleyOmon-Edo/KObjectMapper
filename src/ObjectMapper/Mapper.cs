using System.Collections;
using System.ComponentModel.DataAnnotations;

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
            source.ApplyDiffs<T>(target);
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
            source.ApplyDiffs(target);
        }

        public void MapFrom<T>(T source, T target)
        {
            target.ApplyDiffs<T>(source);
        }

        public void MapFrom(object source, object target)
        {
            target.ApplyDiffs(source);
        }

        public void MapTo<T>(T source, T target)
        {
            source.ApplyDiffs(target);
        }

        public void MapTo(object source, object target)
        {
            target.ApplyDiffs(source);
        }
    }
}
