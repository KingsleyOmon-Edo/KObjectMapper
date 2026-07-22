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
        /// Compares property infos by evaluating their containing values with a supplied predicate.
        /// </summary>
        public class PropertyValueComparer<PropertyInfo, TPropContainer> : IEqualityComparer<PropertyInfo>
        {
            private readonly Func<TPropContainer, PropertyInfo, TPropContainer, PropertyInfo, bool> _predicate;
            private readonly TPropContainer _leftPropContainer;
            private readonly TPropContainer _rightPropContainer;

            /// <summary>
            /// Initializes a new instance of the <see cref="PropertyValueComparer{PropertyInfo, TPropContainer}" /> class.
            /// </summary>
            /// <param name="predicate">The comparison predicate.</param>
            /// <param name="leftPropContainer">The left-side container.</param>
            /// <param name="rightPropContainer">The right-side container.</param>
            public PropertyValueComparer(Func<TPropContainer, PropertyInfo, TPropContainer, PropertyInfo, bool> predicate,
                                         TPropContainer leftPropContainer,
                                         TPropContainer rightPropContainer)
            {
                _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

                this._leftPropContainer = leftPropContainer;
                this._rightPropContainer = rightPropContainer;
            }

            /// <summary>
            /// Determines whether the specified property infos are equal.
            /// </summary>
            public bool Equals(PropertyInfo? x, PropertyInfo? y)
            {
                if (x is null || y is null)
                {
                    return x is null && y is null;
                }

                return _predicate(_leftPropContainer, x, _rightPropContainer, y);
            }

            /// <summary>
            /// Returns a hash code for the specified property info.
            /// </summary>
            public int GetHashCode([DisallowNull] PropertyInfo? obj)
            {
                return obj?.GetHashCode() ?? 0;
            }
        }
    }

