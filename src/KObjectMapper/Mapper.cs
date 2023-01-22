namespace KObjectMapper
{
    using Abstractions;
    using Helpers;

    /// <summary>
    /// The Mapper class that holds the core mapping algorithms
    /// </summary>
    public class Mapper : IObjectMapper
    {
        private readonly MappingService _mappingService = MappingService.Create();

        public Mapper()
        {
        }

        /// <summary>
        /// Reads identical public, mutable properties from a source object to the target object
        /// </summary>
        /// <param name="source">The object from which identical properties will be read</param>
        /// <param name="target">The object to which identical properties will be written</param>
        public void MapTo(object source, object target)
        {
            Checker.NullChecks(source, target);

            Map(source, target);
        }

        /// <summary>
        /// This generic overload writes compatible public writable properties from the source object to the target object
        /// </summary>
        /// <param name="source">The object from which identical properties are to be read</param>
        /// <param name="target">The object to which identical properties are to be written</param>
        /// <typeparam name="TSource">The runtime type of the source object</typeparam>
        /// <typeparam name="TTarget">The runtime type of the target object</typeparam>
        public void MapTo<TSource, TTarget>(TSource source, TTarget target)
        {
            Checker.CoalescedNullCheck(source);
            Checker.CoalescedNullCheck(target);

            Checker.TypeCheck(source);
            Checker.TypeCheck(target);

            _mappingService.ApplyDiffs(source, target);
        }
        
        
        /// <summary>
        /// Maps or read property data from a specified source object to a specified target object.
        /// </summary>
        /// <param name="source">The object from which property values are to be read</param>
        /// <param name="target">The target object to which property values are to be written</param>
        public void MapFrom(object source, object target)
        {
            Checker.NullChecks(source, target);
            
            Map(target, source);
        }

        /// <summary>
        /// Reads identical, public, mutable properties from a source object to a target object
        /// </summary>
        /// <param name="source">The object from which property values are read</param>
        /// <param name="target">The object to which property values are written</param>
        /// <typeparam name="TSource">The runtime type of the source object</typeparam>
        /// <typeparam name="TTarget">The runtime type of the target object</typeparam>
        public void MapFrom<TSource, TTarget>(TSource source, TTarget target)
        {
            Checker.CoalescedNullCheck(source);
            Checker.CoalescedNullCheck(target);

            Checker.TypeCheck(source);
            Checker.TypeCheck(target);

            _mappingService.ApplyDiffs(source, target);
        }

        /// <summary>
        /// Writes identical, public, mutable properties from a source object to a target object
        /// </summary>
        /// <param name="source">The object from which property values are read</param>
        /// <param name="target">The object to which property values are written</param>
        public void Map(object source, object target)
        {
            Checker.NullChecks(source, target);

            _mappingService.ApplyDiffs(source, target);
        }
        
        /// <summary>
        /// A generic overload that reads identical properties from a source object and writes them to a target object
        /// </summary>
        /// <param name="source">The object from which property values are read</param>
        /// <param name="target">The object to which property values are written</param>
        /// <typeparam name="TSource">The runtime type of the source object</typeparam>
        /// <typeparam name="TTarget">The runtime type of the target object</typeparam>
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

        /// <summary>
        /// The generic overload of Map, that reads property values from a source object and writes them to a target object
        /// </summary>
        /// <param name="source">The object whose property values are read</param>
        /// <param name="target">The object to which property values are written</param>
        /// <typeparam name="TSource">The runtime type of the source object</typeparam>
        /// <typeparam name="TTarget">The runtime type of the target object</typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
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

        public TTarget Map<TSource, TTarget>(TSource source)
        {
            //  Algo
            //  ======
            //  Using reflections => possibly Activator.CreateInstance, 
            //  dynamically create an innstace of the specified destination
            //  Map to the props
            //  Return it.
            TTarget target = Activator.CreateInstance<TTarget>();
            _mappingService.ApplyDiffs(source, target);
            return target;
        }
        
        // Map<IEnumerable<Customer>, IEnumerable<CustomerDto>(customers);
        public IEnumerable<TTarget> Map<TSource, TTarget>(IEnumerable<TSource> sourcesInstances)
            where TSource : new()
        {
            // return Enumerable.Empty<TTarget>();
            List<TTarget> targets = new();
            foreach (var source in sourcesInstances)
            {
                TTarget newTargetInstance = Activator.CreateInstance<TTarget>();
                _mappingService.ApplyDiffs(source, newTargetInstance);

                targets.Add(newTargetInstance);
            }

            return targets;
        }
    }
}