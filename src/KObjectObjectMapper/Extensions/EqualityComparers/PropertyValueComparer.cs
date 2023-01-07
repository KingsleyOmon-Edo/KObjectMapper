using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace KObjectObjectMapper.Extensions.EqualityComparers
{
    public static partial class ObjectExtensions
    {
        public class PropertyValueComparer<PropertyInfo, TPropContainer> : IEqualityComparer<PropertyInfo>
        {
            private readonly Func<TPropContainer, PropertyInfo, TPropContainer, PropertyInfo, bool> _predicate;
            private readonly TPropContainer _leftPropContainer;
            private readonly TPropContainer _rightPropContainer;

            public PropertyValueComparer(Func<TPropContainer, PropertyInfo, TPropContainer, PropertyInfo, bool> predicate,
                                         TPropContainer leftPropContainer,
                                         TPropContainer rightPropContainer)
            {
                _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
                
                this._leftPropContainer = leftPropContainer;
                this._rightPropContainer = rightPropContainer;
            }

            public bool Equals(PropertyInfo? x, PropertyInfo? y)
            {
                return _predicate(_leftPropContainer, x, _rightPropContainer, y);             
            }

            public int GetHashCode([DisallowNull] PropertyInfo obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
