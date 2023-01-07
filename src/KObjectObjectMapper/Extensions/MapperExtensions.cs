// ReSharper disable All

namespace KObjectObjectMapper.Extensions
{
    using Helpers;

    public static class MapperExtensions
    {
        public static object MapTo(this object source, object target)
        {
            Checker.NullCheckAll(source, target);

            var mappingService = MappingService.Create();
            mappingService.ApplyDiffs(source, target);

            return target;
        }

        public static object MapFrom(this object target, object source)
        {
            Checker.NullCheckAll(source, target);

            var mappingService = MappingService.Create();
            mappingService.ApplyDiffs(source, target);

            return target;
        }
        
        //  Delegate to the KObjectObjectMapper
        public static TTarget MapTo<TTarget>(this object source, TTarget target)
        {
            // Null check both
            Checker.CoalescedNullCheck<object>(source);
            Checker.CoalescedNullCheck<TTarget>(target);

            //  Type check TTarget only
            Checker.TypeCheck<TTarget>(target);

            var mappingService = MappingService.Create();
            mappingService.ApplyDiffs(source, target);

            return target;
        }
   

        public static object MapFrom<TSource>(this object target, TSource source)
        {
            //  Null check both
            Checker.CoalescedNullCheck<TSource>(source);
            Checker.CoalescedNullCheck<object>(target);

            //  Type check only the source object
            Checker.TypeCheck<TSource>(source);

            var mappingService = MappingService.Create();
            mappingService.ApplyDiffs(source, target);

            return target;
        }

        public static IEnumerable<TTarget> MapFrom<TSource, TTarget>(this IEnumerable<TTarget> target,
            IEnumerable<TSource> source)
            where TTarget : new()
        {
            Checker.NullCheckAll<TSource>(source.ToArray());
            Checker.NullCheckAll<TTarget>(target.ToArray());

            var resultCollection = new List<TTarget>();
            var mappingService = MappingService.Create();
            foreach (var sourceElement in source)
            {
                var targetElement = new TTarget();
          
                mappingService.ApplyDiffs(sourceElement, targetElement);

                resultCollection.Add(targetElement);
            }

            return resultCollection;
        }

        public static IEnumerable<TTarget> MapTo<TSource, TTarget>(this IEnumerable<TSource> source,
            IEnumerable<TTarget> target)
            where TTarget : new()
        {
            Checker.NullCheckAll<TSource>(source.ToArray());
            Checker.NullCheckAll<TTarget>(target.ToArray());

            var resultCollection = new List<TTarget>();
            var mappingService = MappingService.Create();

            foreach (var sourceElement in source)
            {
                var targetElement = new TTarget();
                
                mappingService.ApplyDiffs(sourceElement, targetElement);

                resultCollection.Add(targetElement);
            }

            return resultCollection;
        }
    }
}