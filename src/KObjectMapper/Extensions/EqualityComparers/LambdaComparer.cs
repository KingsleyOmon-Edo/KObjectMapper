using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace KObjectMapper.Extensions.EqualityComparers;

    /// <summary>
    /// Provides comparer helpers used by the mapping LINQ extensions.
    /// </summary>
    public static partial class ObjectExtensions
    {
        /// <summary>
        /// Compares two values by delegating equality to the supplied predicate.
        /// </summary>
        public class LambdaComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _predicate;

            /// <summary>
            /// Initializes a new instance of the <see cref="LambdaComparer{T}" /> class.
            /// </summary>
            /// <param name="predicate">The comparison predicate.</param>
            public LambdaComparer(Func<T, T, bool> predicate)
            {
                this._predicate = predicate;
            }

            /// <summary>
            /// Determines whether the specified values are equal.
            /// </summary>
            public bool Equals(T? x, T? y)
            {
                if (x is null || y is null)
                {
                    return x is null && y is null;
                }

                return _predicate(x, y);
            }

            /// <summary>
            /// Returns a hash code for the specified value.
            /// </summary>
            public int GetHashCode([DisallowNull] T? obj)
            {
                return obj?.GetHashCode() ?? 0;
            }
        }
    }

