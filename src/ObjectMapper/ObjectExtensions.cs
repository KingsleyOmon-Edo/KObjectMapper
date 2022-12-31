using ObjectMapper.Extensions;

namespace ObjectMapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static partial class ObjectExtensions
    {
        public static List<PropertyInfo> GetPropertyDiffs<T>(this T source, T target)
        {
            Checker.NullChecks<T>(source, target);
            Checker.TypeChecks(source, target);
            List<PropertyInfo> diffs = ComputeDiffs(source, target);

            return diffs;   // Shorter
        }

        public static List<PropertyInfo> GetPropertyDiffs(this object source, object target)
        {
            Checker.NullChecks(source, target);
            List<PropertyInfo> diffs = ComputeDiffs(source, target);

            return diffs;
        }
        private static List<PropertyInfo> ComputeDiffs<T>(T source, T target)
        {
            var sourceProps = source.GetType().GetProperties().ToList();
            var targetProps = target.GetType().GetProperties().ToList();

            return sourceProps.Except(targetProps, (
                object sourceObject,
                PropertyInfo sourceProp,
                object targetObject,
                PropertyInfo targetProp) => ArePropValuesDifferent<object>(sourceObject,
                                                                   sourceProp,
                                                                   targetObject,
                                                                   targetProp), source, target)
                                            .ToList();
        }

        private static void NullChecks<T>(T source, T target)
        {
            source = source ?? throw new ArgumentNullException(nameof(source));
            target = target ?? throw new ArgumentNullException(nameof(target));
        }

        private static void PropertyTypeCheck<T>(T source, T target)
        {
            if (source.GetType() != target.GetType())
            {
                throw new ArgumentException($"{nameof(source)} and {nameof(target)} objects should be of the same type");
            }
        }

        public static void ArePropValuesDifferent(object sourceObject, PropertyInfo sourceProp, object targetObject, PropertyInfo targetProp)
        {
            ArePropValuesDifferent<object>(sourceObject, sourceProp, targetObject, targetProp);
        }

        public static bool ArePropValuesDifferent<T>(T sourceObject, PropertyInfo sourceProp, T targetObject, PropertyInfo targetProp)
        {
            PropertyNullChecks(sourceObject, sourceProp, targetObject, targetProp);
            Checker.PropertyNameCheck(sourceProp, targetProp);
            Type sourcePropType, targetPropType;
            ComparePropertyTypes(sourceProp, targetProp, out sourcePropType, out targetPropType);

            object? sourcePropValue = Convert.ChangeType(sourceProp.GetValue(sourceObject), sourcePropType);
            object? targetPropValue = Convert.ChangeType(targetProp.GetValue(targetObject), targetPropType);

            if (Object.Equals(sourcePropValue, targetPropValue) == true)
            {
                return true;
            }

            return false;
        }

        private static void ComparePropertyTypes(PropertyInfo sourceProp, PropertyInfo targetProp, out Type sourcePropType, out Type targetPropType)
        {
            sourcePropType = sourceProp.PropertyType;
            targetPropType = targetProp.PropertyType;
            if (Object.Equals(sourcePropType, targetPropType) == false)
            {
                throw new ArgumentException($"PropertyTypes: {nameof(sourceProp)} and {targetProp} have dissimilar types");
            }
        }

        private static void PropertyNullChecks<T>(T sourceObject, PropertyInfo sourceProp, T targetObject, PropertyInfo targetProp)
            {
            Checker.NullChecks<T>(sourceObject, targetObject);
            Checker.NullChecks<PropertyInfo>(sourceProp, targetProp);
        }

        private static void NullChecks<T>(T sourceObject, PropertyInfo sourceProp, T targetObject, PropertyInfo targetProp)
        {
            sourceObject = sourceObject ?? throw new ArgumentNullException(nameof(sourceObject));
            targetObject = targetObject ?? throw new ArgumentNullException(nameof(targetObject));

            sourceProp = sourceProp ?? throw new ArgumentNullException(nameof(sourceProp));
            targetProp = targetProp ?? throw new ArgumentNullException(nameof(targetProp));
        }

        private static void ValidateParameters<T>(T source, T target)
        {
            Checker.NullChecks(source, target);

            Checker.TypeChecks(source, target);
        }

        public static object ApplyDiffsTo(this object source, object target)
        {
            NullChecks(source, target);
            
            var diffs = source.GetPropertyDiffs(target);
            var sourceProps = source.GetType().GetProperties();
            
            WriteToProperties(source, target, diffs);

            return target;
        }

        public static T ApplyDiffsTo<T>(this T source, T target)
        {
            ValidateParameters(source, target);

            var diffs = source.GetPropertyDiffs(target);
            var sourceProps = source.GetType().GetProperties();
            WriteToProperties(source, target, diffs);

            return target;

        }

        private static void WriteToProperties<T>(T source, T target, List<PropertyInfo> diffs)
        {
            foreach (var sourceProp in diffs)
            {
                foreach (var targetProp in target.GetType().GetProperties())
                {
                    if (sourceProp.Name == targetProp.Name
                        && sourceProp.GetValue(source) != targetProp.GetValue(target))
                    {
                        targetProp.SetValue(target, sourceProp.GetValue(source));
                    }
                }
            }
        }

        public static object SendUpdatesTo(this object source, object target)
        {
            return source.ApplyDiffsTo<object>(target);
        }

        public static T Patch<T>(this T source, T target)
        {
            return source.ApplyDiffsTo(target);
        }

        public static T AcceptChanges<T>(this T target, T source)
        {
            return source.ApplyDiffsTo(target);
        }

        public static T SendChanges<T>(this T source, T target)
        {
            return source.ApplyDiffsTo(target);
        }

        public static T AcceptPatch<T>(this T target, T source)
        {
            return source.ApplyDiffsTo(target);
        }

        public static T SendPatches<T>(this T source, T target)
        {
            return source.ApplyDiffsTo(target);
        }

        public static T PatchFrom<T>(this T target, T source)
        {
            return source.ApplyDiffsTo(target);
        }

        public static T PatchTo<T>(this T sources, T target)
        {
            return sources.ApplyDiffsTo(target);
        }

        public static T AcceptUpdates<T>(this T target, T source)
        {
            return source.ApplyDiffsTo(target);
        }

        public static T SendUpdates<T>(this T source, T target)
        {
            return source.ApplyDiffsTo(target);
        }

    }
}
