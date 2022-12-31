using System.Diagnostics.CodeAnalysis;

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
            source.ApplyDiffs<T>(target);
        }

        public void Map(object source, object target)
        {
            source = source ?? throw new ArgumentNullException(nameof(source));
            target = target ?? throw new ArgumentNullException(nameof(target));

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

        public void MapTo<T>([DisallowNull] T source, T target)
        {
            source = source ?? throw new ArgumentNullException(nameof(source));
            target = target ?? throw new ArgumentNullException(nameof(target));
            source.ApplyDiffs(target);
        }

        public void MapTo(object source, object target)
        {
            target.ApplyDiffs(source);
        }
    }
}
