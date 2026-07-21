using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace KObjectMapper.Extensions.EqualityComparers
{
    public static partial class ObjectExtensions
    {
        public class StructuralEqualityComparer<T> : IEqualityComparer<T>
        {
            public bool Equals(T? x, T? y)
            {
                return StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
            }

            public int GetHashCode([DisallowNull] T? obj)
            {
                return obj is null ? 0 : StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
            }
        }
    }
}
