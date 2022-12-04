namespace Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static partial class ObjectExtensions
    {
        public static List<PropertyInfo> GetPropertyDiffs<T>(this T source, T target)
        {
            ValidateParameters(source, target);

            var sourceProps = source.GetType().GetProperties().ToList();
            var targetProps = target.GetType().GetProperties().ToList();

            bool ArePropValuesDifferent(T sourceObject, PropertyInfo sourceProp, T targetObject, PropertyInfo targetProp)
            {          

                if (sourceProp is null)
                {
                    throw new ArgumentNullException(nameof(sourceProp));
                }

                if (targetProp is null)
                {
                    throw new ArgumentNullException(nameof(targetProp));
                }

                if (object.Equals(sourceProp.Name, targetProp.Name) == false)
                {
                    throw new ArgumentException($"PropertyNames: {nameof(sourceProp)} and {targetProp} have dissimilar names");
                }

                Type sourceType = sourceProp.PropertyType;
                Type targetType = targetProp.PropertyType;

                if (object.Equals(sourceType,  targetType) == false)
                {
                    throw new ArgumentException($"PropertyTypes: {nameof(sourceProp)} and {targetProp} have dissimilar types");
                }

                object? sValue = Convert.ChangeType(sourceProp.GetValue(sourceObject), sourceType);
                object? tValue = Convert.ChangeType(targetProp.GetValue(targetObject), targetType);

                if (object.Equals(sValue, tValue) == true)
                {                  
                    return true;
                }

                return false;
            }

            var diffs = sourceProps.Except(targetProps, ArePropValuesDifferent, source, target)
                                  .ToList();

            return diffs;  
        }

        private static void ValidateParameters<T>(T source, T target)
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
        }

        public static T ApplyDiffs<T>(this T source, T target)
        {
            ValidateParameters(source, target);

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
