using KObjectMapper.Abstractions;

namespace KObjectMapper;

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
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(target);

            var safeSource = source!;
            var safeTarget = target!;

            Map(safeSource, safeTarget);
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
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(target);

            object safeSource = source!;
            object safeTarget = target!;

            if (safeSource.GetType() != typeof(TSource))
            {
                throw new ArgumentException($"Parameter {nameof(safeSource)} has an incompatible type with {nameof(TSource)}");
            }

            if (safeTarget.GetType() != typeof(TTarget))
            {
                throw new ArgumentException($"Parameter {nameof(safeTarget)} has an incompatible type with {nameof(TTarget)}");
            }

            _mappingService.ApplyDiffs(safeSource, safeTarget);
        }

        /// <summary>
        /// Maps or read property data from a specified source object to a specified target object.
        /// </summary>
        /// <param name="source">The object from which property values are to be read</param>
        /// <param name="target">The target object to which property values are to be written</param>
        public void MapFrom(object source, object target)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(target);

            var safeSource = source!;
            var safeTarget = target!;

            Map(safeTarget, safeSource);
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
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(target);

            object safeSource = source!;
            object safeTarget = target!;

            if (safeSource.GetType() != typeof(TSource))
            {
                throw new ArgumentException($"Parameter {nameof(safeSource)} has an incompatible type with {nameof(TSource)}");
            }

            if (safeTarget.GetType() != typeof(TTarget))
            {
                throw new ArgumentException($"Parameter {nameof(safeTarget)} has an incompatible type with {nameof(TTarget)}");
            }

            _mappingService.ApplyDiffs(safeSource, safeTarget);
        }

        /// <summary>
        /// Writes identical, public, mutable properties from a source object to a target object
        /// </summary>
        /// <param name="source">The object from which property values are read</param>
        /// <param name="target">The object to which property values are written</param>
        public void Map(object source, object target)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(target);

            _mappingService.ApplyDiffs(source!, target!);
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
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(target);

            if (source is not null && source.GetType() != typeof(TSource))
            {
                throw new ArgumentException($"Parameter {nameof(source)} has an incompatible type with {nameof(TSource)}");
            }

            if (target is not null && target.GetType() != typeof(TTarget))
            {
                throw new ArgumentException($"Parameter {nameof(target)} has an incompatible type with {nameof(TTarget)}");
            }

            _mappingService.ApplyDiffs(source!, target!);
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

            List<TSource> sourceItems = source.ToList();

            if (target is IList<TTarget> targetList && !targetList.IsReadOnly)
            {
                int index = 0;

                foreach (TSource sourceElement in sourceItems)
                {
                    TTarget targetElement;

                    if (index < targetList.Count && targetList[index] is not null)
                    {
                        targetElement = targetList[index]!;
                    }
                    else
                    {
                        targetElement = new TTarget();

                        if (index < targetList.Count)
                        {
                            targetList[index] = targetElement;
                        }
                        else
                        {
                            targetList.Add(targetElement);
                        }
                    }

                    _mappingService.ApplyDiffs(sourceElement!, targetElement);
                    index++;
                }

                while (targetList.Count > sourceItems.Count)
                {
                    targetList.RemoveAt(targetList.Count - 1);
                }

                return targetList;
            }

            List<TTarget> resultCollection = new();

            foreach (TSource sourceElement in sourceItems)
            {
                TTarget targetElem = new();
                _mappingService.ApplyDiffs(sourceElement!, targetElem);

                resultCollection.Add(targetElem);
            }

            return resultCollection;
        }

        public static Mapper Create() => new();

        public TTarget Map<TSource, TTarget>(TSource source)
        {
            TTarget target = Activator.CreateInstance<TTarget>();
            object safeSource = source!;
            object safeTarget = target!;
            _mappingService.ApplyDiffs(safeSource, safeTarget);
            return target;
        }

        public IEnumerable<TTarget> Map<TSource, TTarget>(IEnumerable<TSource> sources)
        {
            List<TTarget> targets = new();
            foreach (var source in sources)
            {
                TTarget newTargetInstance = Activator.CreateInstance<TTarget>();
                object safeSource = source!;
                object safeTarget = newTargetInstance!;
                _mappingService.ApplyDiffs(safeSource, safeTarget);

                targets.Add(newTargetInstance);
            }

            return targets;
        }
    }
