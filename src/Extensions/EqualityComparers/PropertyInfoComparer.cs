namespace Extensions
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    public static partial class ObjectExtensions
    {
        public class PropertyInfoComparer : EqualityComparer<PropertyInfo>
        {
            public override bool Equals(PropertyInfo? x, PropertyInfo? y)
            {            

                return StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
            }

            public override int GetHashCode([DisallowNull] PropertyInfo obj)
            {
                return StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
            }
        }
    }
}
