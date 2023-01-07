using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace KObjectMapper.Extensions.EqualityComparers
{
    public static partial class ObjectExtensions
    {
        public class LambdaComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _predicate;

            public LambdaComparer(Func<T, T, bool> predicate)
            {
                this._predicate = predicate;
            }

            public bool Equals(T? x, T? y)
            {
                return _predicate(x, y);
            }

            public int GetHashCode([DisallowNull] T obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
