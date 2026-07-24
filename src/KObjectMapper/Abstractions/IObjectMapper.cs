using System.Linq.Expressions;
using KObjectMapper.Collections;

namespace KObjectMapper.Abstractions;

    /// <summary>
    /// Defines the mapping operations supported by the object mapper.
    /// </summary>
    public interface IObjectMapper
    {
        /// <summary>Maps the readable properties from <paramref name="source" /> to <paramref name="target" />.</summary>
        /// <param name="source">The source object.</param>
        /// <param name="target">The destination object.</param>
        void MapTo(object source, object target);

        /// <summary>Maps the readable properties from <paramref name="source" /> to <paramref name="target" />.</summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TTarget">The destination type.</typeparam>
        /// <param name="source">The source object.</param>
        /// <param name="target">The destination object.</param>
        void MapTo<TSource, TTarget>(TSource source, TTarget target);

        /// <summary>Maps the readable properties from <paramref name="source" /> to <paramref name="target" />.</summary>
        /// <param name="source">The source object.</param>
        /// <param name="target">The destination object.</param>
        void MapFrom(object source, object target);

        /// <summary>Maps the readable properties from <paramref name="source" /> to <paramref name="target" />.</summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TTarget">The destination type.</typeparam>
        /// <param name="source">The source object.</param>
        /// <param name="target">The destination object.</param>
        void MapFrom<TSource, TTarget>(TSource source, TTarget target);

        /// <summary>Maps the readable properties from <paramref name="source" /> to <paramref name="target" />.</summary>
        /// <param name="source">The source object.</param>
        /// <param name="target">The destination object.</param>
        void Map(object source, object target);

        /// <summary>Maps the readable properties from <paramref name="source" /> to <paramref name="target" />.</summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TTarget">The destination type.</typeparam>
        /// <param name="source">The source object.</param>
        /// <param name="target">The destination object.</param>
        void Map<TSource, TTarget>(TSource source, TTarget target);

        /// <summary>
        /// Maps a source collection into a destination collection, reusing the destination elements when possible.
        /// </summary>
        /// <typeparam name="TSource">The source element type.</typeparam>
        /// <typeparam name="TTarget">The destination element type.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="target">The destination collection.</param>
        IEnumerable<TTarget> Map<TSource, TTarget>(IEnumerable<TSource> source, IEnumerable<TTarget> target)
            where TTarget : new()
            where TSource : new();

        IEnumerable<TTarget> Map<TSource, TTarget>(
            IEnumerable<TSource> source,
            IEnumerable<TTarget> target,
            CollectionMappingOptions<TSource, TTarget> options)
            where TTarget : new()
            where TSource : new();

        MappingResult TryMap(object source, object target);

        MappingResult TryMap<TSource, TTarget>(TSource source, TTarget target);

        IQueryable<TTarget> ProjectTo<TSource, TTarget>(IQueryable<TSource> source)
            where TTarget : new();

        Expression<Func<TSource, TTarget>> GetProjectionExpression<TSource, TTarget>()
            where TTarget : new();
    }

