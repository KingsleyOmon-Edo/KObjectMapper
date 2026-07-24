using System.Globalization;
using System.Reflection;
using KObjectMapper.Abstractions;

namespace KObjectMapper;

/// <summary>
/// The Mapper class that holds the core mapping algorithms.
/// </summary>
public class Mapper : IObjectMapper
{
    private readonly MappingService _mappingService = MappingService.Create();
    private readonly IEnumerable<MappingProfile> _profiles;
    private readonly NullMappingPolicy? _globalNullPolicy;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mapper" /> class.
    /// </summary>
    public Mapper()
    {
        _profiles = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Mapper" /> class with the provided profiles.
    /// </summary>
    /// <param name="profiles">The mapping profiles to use.</param>
    public Mapper(IEnumerable<MappingProfile> profiles)
    {
        ArgumentNullException.ThrowIfNull(profiles);
        _profiles = profiles;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Mapper" /> class with profiles and a global null policy.
    /// </summary>
    /// <param name="profiles">The mapping profiles to use.</param>
    /// <param name="globalNullPolicy">The global null mapping policy.</param>
    internal Mapper(IEnumerable<MappingProfile> profiles, NullMappingPolicy? globalNullPolicy)
    {
        ArgumentNullException.ThrowIfNull(profiles);
        _profiles = profiles;
        _globalNullPolicy = globalNullPolicy;
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

            MappingTypeMap? typeMap = FindTypeMap(typeof(TSource), typeof(TTarget));

            if (typeMap is not null)
            {
                ApplyProfileBasedMapping(source!, target!, typeMap);
            }
            else
            {
                _mappingService.ApplyDiffs(source!, target!);
            }
        }

        private MappingTypeMap? FindTypeMap(Type sourceType, Type targetType)
        {
            return _profiles
                .SelectMany(profile => profile.TypeMaps)
                .FirstOrDefault(typeMap =>
                    typeMap.SourceType == sourceType &&
                    typeMap.TargetType == targetType);
        }

        private void ApplyProfileBasedMapping<TSource, TTarget>(TSource source, TTarget target, MappingTypeMap typeMap)
        {
            Type sourceType = typeof(TSource);
            Type targetType = typeof(TTarget);

            PropertyInfo[] sourceProperties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] targetProperties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            NullMappingPolicy effectiveNullPolicy = typeMap.NullPolicy ?? _globalNullPolicy ?? NullMappingPolicy.Propagate;

            foreach (PropertyInfo targetProperty in targetProperties)
            {
                if (!targetProperty.CanWrite)
                {
                    continue;
                }

                if (typeMap.IgnoredMembers.Contains(targetProperty.Name))
                {
                    continue;
                }

                PropertyInfo? sourceProperty = null;

                string? customSourceMember = typeMap.CustomMemberMappings
                    .Where(kvp => kvp.Value == targetProperty.Name)
                    .Select(kvp => kvp.Key)
                    .FirstOrDefault();

                if (customSourceMember is not null)
                {
                    sourceProperty = sourceProperties.FirstOrDefault(p => p.Name == customSourceMember);
                }
                else
                {
                    bool isTargetOfCustomMapping = typeMap.CustomMemberMappings.Values.Contains(targetProperty.Name);

                    if (!isTargetOfCustomMapping)
                    {
                        sourceProperty = sourceProperties.FirstOrDefault(p => p.Name == targetProperty.Name);
                    }
                }

                if (sourceProperty is not null && sourceProperty.CanRead)
                {
                    try
                    {
                        object? sourceValue = sourceProperty.GetValue(source);

                        if (sourceValue is null)
                        {
                            if (typeMap.NullSubstitutes.TryGetValue(targetProperty.Name, out object? substitute))
                            {
                                targetProperty.SetValue(target, substitute);
                                continue;
                            }

                            if (effectiveNullPolicy == NullMappingPolicy.Ignore)
                            {
                                continue;
                            }
                        }

                        targetProperty.SetValue(target, sourceValue);
                    }
                    catch (ArgumentException)
                    {
                    }
                }
            }
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

        /// <summary>
        /// Creates a new <see cref="Mapper" /> instance.
        /// </summary>
        public static Mapper Create() => new();

        /// <summary>
        /// Maps a source object into a newly created destination object.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TTarget">The destination type.</typeparam>
        /// <param name="source">The source object.</param>
        /// <returns>The mapped destination object.</returns>
        public TTarget Map<TSource, TTarget>(TSource source)
        {
            ArgumentNullException.ThrowIfNull(source);

            TTarget target = CreateTargetInstance<TSource, TTarget>(source);
            object safeSource = source;
            object safeTarget = target!;
            _mappingService.ApplyDiffs(safeSource, safeTarget);
            return target;
        }

        /// <summary>
        /// Maps a sequence of source objects into a newly created destination sequence.
        /// </summary>
        /// <typeparam name="TSource">The source element type.</typeparam>
        /// <typeparam name="TTarget">The destination element type.</typeparam>
        /// <param name="sources">The source collection.</param>
        /// <returns>The mapped destination collection.</returns>
        public IEnumerable<TTarget> Map<TSource, TTarget>(IEnumerable<TSource> sources)
        {
            ArgumentNullException.ThrowIfNull(sources);

            List<TTarget> targets = new();
            foreach (TSource source in sources)
            {
                ArgumentNullException.ThrowIfNull(source);

                TTarget newTargetInstance = Map<TSource, TTarget>(source);
                targets.Add(newTargetInstance);
            }

            return targets;
        }

        private static TTarget CreateTargetInstance<TSource, TTarget>(TSource source)
        {
            Type targetType = typeof(TTarget);
            ConstructorInfo? parameterlessConstructor = targetType.GetConstructor(Type.EmptyTypes);

            if (parameterlessConstructor is not null)
            {
                return (TTarget)parameterlessConstructor.Invoke(null);
            }

            object instance = CreateTargetFromConstructor(source!, targetType);
            return (TTarget)instance;
        }

        private static object CreateTargetFromConstructor<TSource>(TSource source, Type targetType)
        {
            ConstructorInfo[] constructors = targetType.GetConstructors()
                .OrderByDescending(constructor => constructor.GetParameters().Length)
                .ToArray();

            foreach (ConstructorInfo constructor in constructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                if (parameters.Length == 0)
                {
                    continue;
                }

                if (TryBuildConstructorArguments(source, parameters, out object?[] arguments))
                {
                    return constructor.Invoke(arguments);
                }
            }

            throw new InvalidOperationException(
                $"Could not create an instance of '{targetType.Name}'. Ensure it has either a public parameterless constructor or a public constructor whose parameter names and types are mappable from the source object.");
        }

        private static bool TryBuildConstructorArguments<TSource>(
            TSource source,
            IReadOnlyList<ParameterInfo> parameters,
            out object?[] arguments)
        {
            Dictionary<string, PropertyInfo> sourceProperties = source!.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(property => property.Name, StringComparer.OrdinalIgnoreCase);

            arguments = new object?[parameters.Count];

            for (int index = 0; index < parameters.Count; index++)
            {
                ParameterInfo parameter = parameters[index];
                if (parameter.Name is null || !sourceProperties.TryGetValue(parameter.Name, out PropertyInfo? sourceProperty))
                {
                    return false;
                }

                object? sourceValue = sourceProperty.GetValue(source);
                if (!TryConvertValue(sourceValue, parameter.ParameterType, out object? convertedValue))
                {
                    return false;
                }

                arguments[index] = convertedValue;
            }

            return true;
        }

        private static bool TryConvertValue(object? sourceValue, Type targetType, out object? convertedValue)
        {
            if (sourceValue is null)
            {
                if (!targetType.IsValueType || Nullable.GetUnderlyingType(targetType) is not null)
                {
                    convertedValue = null;
                    return true;
                }

                convertedValue = null;
                return false;
            }

            if (targetType.IsInstanceOfType(sourceValue))
            {
                convertedValue = sourceValue;
                return true;
            }

            try
            {
                convertedValue = Convert.ChangeType(sourceValue, targetType, CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                convertedValue = null;
                return false;
            }
        }
    }
