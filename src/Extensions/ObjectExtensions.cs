using System;
using System.Net.NetworkInformation;
using System.Reflection;

namespace Extensions
{
    public static class ObjectExtensions
    {
        public static Dictionary<PropertyInfo, PropertyInfo> GetGiffs<T>(this T source, T target)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (source.GetType() != target.GetType())
            {
                throw new ArgumentException($"{nameof(source)} and {nameof(target)} objects should be of the same type");
            }

            Dictionary<PropertyInfo, PropertyInfo> propertyDiffs = new Dictionary<PropertyInfo, PropertyInfo>();


            //  TODO: Use LINQ Except() extension method as an alternative.

            foreach (var sourceProp in source.GetType().GetProperties())
            {
                foreach (var destProp in target.GetType().GetProperties())
                {
                    if (sourceProp.Name == destProp.Name
                        && sourceProp.GetValue(source) != destProp.GetValue(target))
                    {            
                        propertyDiffs.Add(sourceProp, destProp);
                    }
                }
            }

            return propertyDiffs;

        }

        public static T ApplyDiffs<T>(this T source, T target)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (source.GetType() != target.GetType())
            {
                throw new ArgumentException($"{nameof(source)} and {nameof(target)} objects should be of the same type");
            }

            foreach (var sourceProp in source.GetType().GetProperties())
            {
                foreach (var destProp in target.GetType().GetProperties())
                {
                    if (sourceProp.Name == destProp.Name
                        && sourceProp.GetValue(source) != destProp.GetValue(target))
                    {
                        destProp.SetValue(target, sourceProp.GetValue(source));
                    }
                }
            }

            return target;

            // Simplicity is the ultimate sophistication.
        }
        public static object SendUpdatesTo(this object source, object target)
        {
            return source.ApplyDiffs(target);
            
        }

        public static T Patch<T>(this T source, T target)
        {
            return source.ApplyDiffs(target);
        }

        public static T AcceptChanges<T>(this T target, T source)
        {
            return source.ApplyDiffs(target);
        }

        public static T SendChanges<T>(this T source, T target)
        {
            return source.ApplyDiffs(target);
        }

        public static T AcceptPatch<T>(this T target, T source)
        {
            return source.ApplyDiffs(target);
        }

        public static T SendPatches<T>(this T source, T target)
        {
            return source.ApplyDiffs(target);
        }

        public static T PatchFrom<T>(this T target, T source)
        {
            return source.ApplyDiffs(target);
        }

        public static T PatchTo<T>(this T sources, T target)
        {
            return sources.ApplyDiffs(target);
        }

        public static T AcceptUpdates<T>(this T target, T source)
        {
            return source.ApplyDiffs(target);
        }

        public static T SendUpdates<T>(this T source, T target)
        {
            return source.ApplyDiffs(target);
        }
    }
}
