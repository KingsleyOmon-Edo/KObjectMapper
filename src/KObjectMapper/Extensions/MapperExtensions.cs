// ReSharper disable All

namespace KObjectMapper.Extensions
{
    using Helpers;

    public static class MapperExtensions
    {
        /// <summary>
        /// MapTo() reads property values from the object on which
        /// it is invoked, and writes them to the target object that is passed in as
        /// argument.
        /// 
        /// After the KObjectMapper nuget package is installed, this extension method
        /// becomes available on all objects within the project.
        /// </summary>
        /// <param name="source">The originating object from which property values are read</param>
        /// <param name="target">The receiving object to which the property values are written.</param>
        /// <returns>Returns the mapped target object instance</returns>
        public static object MapTo(this object source, object target)
        {
            Checker.NullCheckAll(source, target);

            var mappingService = MappingService.Create();
            mappingService.ApplyDiffs(source, target);

            return target;
        }

        /// <summary>
        /// MapFrom() reads property values from the object instance passed to it
        /// as an argument, and writes them to the current object on which it
        /// was invoked.
        ///
        /// This extension method becomes available to all objects after the nuget
        /// package is installed.
        /// </summary>
        /// <param name="target">The invocation subject. The one to which properties will be written</param>
        /// <param name="source">The object from which properties are read.</param>
        /// <returns>The already mapped target object.</returns>
        public static object MapFrom(this object target, object source)
        {
            Checker.NullCheckAll(source, target);

            var mappingService = MappingService.Create();
            mappingService.ApplyDiffs(source, target);

            return target;
        }

        /// <summary>
        /// This the generic overload of the MapTo() method. It reads property values from
        /// the invocation subject, i.e. the object it is invoked upon, and writes them to
        /// the object that is passed in as a parameter.
        /// </summary>
        /// <param name="source">The object from which property data is read.</param>
        /// <param name="target">The object to which property data is written.</param>
        /// <typeparam name="TTarget">The runtime type of the target object</typeparam>
        /// <returns></returns>
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

        /// <summary>
        /// This is the generic overload of the MapFrom() method. It reads property values from
        /// the object that is passed to it as a parameter, and writes them to the
        /// object upon which it was invoked. 
        /// </summary>
        /// <param name="target">The receiver object, to which property values are to be wrritten</param>
        /// <param name="source">The object whose property values are to be read</param>
        /// <typeparam name="TSource">The runtime type of source object</typeparam>
        /// <returns>Returns the already mapped target object instance</returns>
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
        
        /// <summary>
        /// This is an extension method available on IEnumerable<T> or derived instances.
        ///
        /// It maps an inpout IEnumerable<T> to a corresponding output IEnumerable<T>,
        /// mapping each source/target element pair accordingly, where their properties
        /// are identical, public and mutable.
        /// </summary>
        /// <param name="target">The receiver collection object to which mapped objects
        /// are added.</param>
        /// <param name="source">The originating IEnumerable<T> collection object whose elements have their
        /// property values read and written to target.</param>
        /// <typeparam name="TSource">The runtime type of the source object</typeparam>
        /// <typeparam name="TTarget">The runtime type of the target object</typeparam>
        /// <returns>The mapped collection.</returns>
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

        /// <summary>
        /// This method maps property values of objects in an input collection, called the source,
        /// to those of objects in an output collection here referred to as the target, using generics.
        /// </summary>
        /// <param name="source">The originating collection object whose elements have their property
        /// values read</param>
        /// <param name="target">The receiving collection object whose elements have their property values
        /// written to</param>
        /// <typeparam name="TSource">The type of the IEnumerable<T> input collection</typeparam>
        /// <typeparam name="TTarget">The runtime type expecetd by the IEnumerable<T> output collection</typeparam>
        /// <returns>The mapped resulting IEnumerable<T></returns>
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