namespace ObjectMapper
{
    using System;

    public class Mapper
    {
        public Mapper()
        {
        }

        public void Map<T>(T source, T target)
        {
            source.ApplyDiffs<T>(target);
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
