using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace KObjectMapper.Extensions.EqualityComparers;

    /// <summary>
    /// Provides comparer helpers used by the mapping LINQ extensions.
    /// </summary>
    public static partial class ObjectExtensions
    {
        /// <summary>
        /// Compares values using structural equality.
        /// </summary>
        public class StructuralEqualityComparer<T> : IEqualityComparer<T>
        {
            /// <summary>
            /// Determines whether the specified values are equal.
            /// </summary>
            public bool Equals(T? x, T? y)
            {
                return StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
            }

            /// <summary>
            /// Returns a hash code for the specified value.
            /// </summary>
            public int GetHashCode([DisallowNull] T? obj)
            {
                return obj is null ? 0 : StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
            }
        }
    }

