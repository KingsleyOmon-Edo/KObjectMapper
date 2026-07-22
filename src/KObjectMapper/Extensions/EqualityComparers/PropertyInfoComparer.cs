using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace KObjectMapper.Extensions.EqualityComparers;

    /// <summary>
    /// Provides comparer helpers used by the mapping LINQ extensions.
    /// </summary>
    public static partial class ObjectExtensions
    {
        /// <summary>
        /// Compares <see cref="PropertyInfo" /> instances using structural equality.
        /// </summary>
        public class PropertyInfoComparer : EqualityComparer<PropertyInfo>
        {
            /// <summary>
            /// Determines whether the specified property infos are equal.
            /// </summary>
            public override bool Equals(PropertyInfo? x, PropertyInfo? y)
            {
                return StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
            }

            /// <summary>
            /// Returns a hash code for the specified property info.
            /// </summary>
            public override int GetHashCode([DisallowNull] PropertyInfo obj)
            {
                return StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
            }
        }
    }

