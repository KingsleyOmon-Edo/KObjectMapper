namespace ObjectMapper
{
    using Extensions;
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

        public void MapFrom<T>(T source, T target)
        {
            target.ApplyDiffs<T>(source);
        }

        public void MapTo<T>(T source, T target)
        {
            source.ApplyDiffs(target);
        }
    }
}
