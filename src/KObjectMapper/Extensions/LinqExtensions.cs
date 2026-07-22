namespace KObjectMapper.Extensions;

    /// <summary>
    /// Provides LINQ helpers used by the mapping implementation.
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Returns the elements that exist in both sequences according to the supplied predicate.
        /// </summary>
        public static IEnumerable<T> Except<T>(this IEnumerable<T> leftList, IEnumerable<T> rightList, Func<T, T, bool> predicate)
        {
            return leftList.Except(rightList, new EqualityComparers.ObjectExtensions.LambdaComparer<T>(predicate));
        }

        /// <summary>
        /// Returns the property items that exist in both sequences according to the supplied predicate.
        /// </summary>
        public static IEnumerable<PropertyInfo> Except<PropertyInfo, TPropContainer>(this IEnumerable<PropertyInfo> leftList,
                                                                                     IEnumerable<PropertyInfo> rightList,
                                                                                     Func<TPropContainer, PropertyInfo, TPropContainer, PropertyInfo, bool> predicate,
                                                                                     TPropContainer leftPropContainer,
                                                                                     TPropContainer rightPropContainer)
        {
            return leftList.Except(rightList, new EqualityComparers.ObjectExtensions.PropertyValueComparer<PropertyInfo, TPropContainer>(predicate, leftPropContainer, rightPropContainer));
        }

        /// <summary>
        /// Returns the elements that exist in both sequences according to the supplied predicate.
        /// </summary>
        public static IEnumerable<T> Intersect<T>(this IEnumerable<T> leftList, IEnumerable<T> rightList, Func<T, T, bool> predicate)
        {
            return leftList.Intersect<T>(rightList, new EqualityComparers.ObjectExtensions.LambdaComparer<T>(predicate));
        }

        /// <summary>
        /// Returns the property items that exist in both sequences according to the supplied predicate.
        /// </summary>
        public static IEnumerable<PropertyInfo> Intersect<PropertyInfo, TPropContainer>(this IEnumerable<PropertyInfo> leftList,
                                                                                     IEnumerable<PropertyInfo> rightList,
                                                                                     Func<TPropContainer, PropertyInfo, TPropContainer, PropertyInfo, bool> predicate,
                                                                                     TPropContainer leftPropContainer,
                                                                                     TPropContainer rightPropContainer)
        {
            return leftList.Intersect(rightList, new EqualityComparers.ObjectExtensions.PropertyValueComparer<PropertyInfo, TPropContainer>(predicate, leftPropContainer, rightPropContainer));
        }

    }

