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
            source = source ?? throw new ArgumentNullException(nameof(source));
            target = target?? throw new ArgumentNullException(nameof(target));
            
            if (source.GetType() != target.GetType())
            {
                throw new ArgumentException($"{nameof(source)} and {nameof(target)} objects should be of the same type");
            }

            var sourceProps = source.GetType().GetProperties().ToList();
            var targetProps = target.GetType().GetProperties().ToList();

            var diffs = sourceProps.Except(targetProps, (
                T sourceObject,
                PropertyInfo sourceProp,
                T targetObject,
                PropertyInfo targetProp) => ArePropValuesDifferent(sourceObject,
                                                                   sourceProp,
                                                                   targetObject,
                                                                   targetProp), source, target)
                                            .ToList();

            return diffs;  
        }

        public static bool ArePropValuesDifferent<T>(T sourceObject, PropertyInfo sourceProp, T targetObject, PropertyInfo targetProp)
        {
            sourceObject = sourceObject ?? throw new ArgumentNullException(nameof(sourceObject));
            targetObject = targetObject ?? throw new ArgumentNullException(nameof(targetObject));

            sourceProp = sourceProp ?? throw new ArgumentNullException(nameof(sourceProp));
            targetProp = targetProp ?? throw new ArgumentNullException(nameof(targetProp));

            if (object.Equals(sourceProp.Name, targetProp.Name) == false)
            {
                throw new ArgumentException($"PropertyNames: {nameof(sourceProp)} and {targetProp} have dissimilar names");
            }

            Type sourcePropType = sourceProp.PropertyType;
            Type targetPropType = targetProp.PropertyType;

            if (object.Equals(sourcePropType, targetPropType) == false)
            {
                throw new ArgumentException($"PropertyTypes: {nameof(sourceProp)} and {targetProp} have dissimilar types");
            }

            object? sourcePropValue = Convert.ChangeType(sourceProp.GetValue(sourceObject), sourcePropType);
            object? targetPropValue = Convert.ChangeType(targetProp.GetValue(targetObject), targetPropType);

            if (object.Equals(sourcePropValue, targetPropValue) == true)
            {
                return true;
            }

            return false;
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

            var diffs = source.GetPropertyDiffs<T>(target);
            var sourceProps = source.GetType().GetProperties();

            foreach (var sourceProp in sourceProps)
            {
                foreach (var destProp in diffs)
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
