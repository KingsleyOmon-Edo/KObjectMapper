namespace ObjectMapper.Extensions.Extras
{
    using System.Reflection;

    public static partial class ObjectExtensions
    {
        public static List<PropertyInfo> GetPropertyDiffs<T>(this T source, T target)
        {
            ObjectExtensions.NullChecks(source, target);
            ObjectExtensions.PropertyTypeCheck(source, target);
            var diffs = ObjectExtensions.ComputeDiffs(source, target);

            return diffs; // Shorter
        }

        public static List<PropertyInfo> GetPropertyDiffs(this object source, object target)
        {
            ObjectExtensions.NullChecks(source, target);
            var diffs = ObjectExtensions.ComputeDiffs(source, target);

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
                    PropertyInfo targetProp) => ObjectExtensions.ArePropValuesDifferent(sourceObject,
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
                throw new ArgumentException(
                    $"{nameof(source)} and {nameof(target)} objects should be of the same type");
            }
        }

        public static void ArePropValueDifferent(object sourceObject, PropertyInfo sourceProp, object targetObject,
            PropertyInfo targetProp)
        {
            ObjectExtensions.ArePropValuesDifferent(sourceObject, sourceProp, targetObject, targetProp);
        }

        public static bool ArePropValuesDifferent<T>(T sourceObject, PropertyInfo sourceProp, T targetObject,
            PropertyInfo targetProp)
        {
            ObjectExtensions.NullChecks(sourceObject, sourceProp, targetObject, targetProp);
            ObjectExtensions.PropertyNameCheck(sourceProp, targetProp);
            Type sourcePropType, targetPropType;
            ObjectExtensions.ProperyTypeCheck(sourceProp, targetProp, out sourcePropType, out targetPropType);

            var sourcePropValue = Convert.ChangeType(sourceProp.GetValue(sourceObject), sourcePropType);
            var targetPropValue = Convert.ChangeType(targetProp.GetValue(targetObject), targetPropType);

            if (object.Equals(sourcePropValue, targetPropValue))
            {
                return true;
            }

            return false;
        }

        private static void ProperyTypeCheck(PropertyInfo sourceProp, PropertyInfo targetProp, out Type sourcePropType,
            out Type targetPropType)
        {
            sourcePropType = sourceProp.PropertyType;
            targetPropType = targetProp.PropertyType;
            if (object.Equals(sourcePropType, targetPropType) == false)
            {
                throw new ArgumentException(
                    $"PropertyTypes: {nameof(sourceProp)} and {targetProp} have dissimilar types");
            }
        }

        private static void PropertyNameCheck(PropertyInfo sourceProp, PropertyInfo targetProp)
        {
            if (object.Equals(sourceProp.Name, targetProp.Name) == false)
            {
                throw new ArgumentException(
                    $"PropertyNames: {nameof(sourceProp)} and {targetProp} have dissimilar names");
            }
        }

        private static void NullChecks<T>(T sourceObject, PropertyInfo sourceProp, T targetObject,
            PropertyInfo targetProp)
        {
            sourceObject = sourceObject ?? throw new ArgumentNullException(nameof(sourceObject));
            targetObject = targetObject ?? throw new ArgumentNullException(nameof(targetObject));

            sourceProp = sourceProp ?? throw new ArgumentNullException(nameof(sourceProp));
            targetProp = targetProp ?? throw new ArgumentNullException(nameof(targetProp));
        }

        private static void ValidateParameters<T>(T source, T target)
        {
            ObjectExtensions.NullChecks(source, target);

            ObjectExtensions.TypeChecks(source, target);
        }

        private static void TypeChecks<T>(T source, T target)
        {
            if (source?.GetType() != target?.GetType())
            {
                throw new ArgumentException(
                    $"{nameof(source)} and {nameof(target)} objects should be of the same type");
            }
        }

        public static object ApplyDiffs(this object source, object target)
        {
            ObjectExtensions.NullChecks(source, target);
            var diffs = source.GetPropertyDiffs(target);
            var sourceProps = source.GetType().GetProperties();
            ObjectExtensions.WriteToProperties(source, target, diffs);

            return target;
        }

        public static T ApplyDiffs<T>(this T source, T target)
        {
            ObjectExtensions.ValidateParameters(source, target);

            var diffs = source.GetPropertyDiffs(target);
            var sourceProps = source.GetType().GetProperties();
            ObjectExtensions.WriteToProperties(source, target, diffs);

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

        public static object SendUpdatesTo(this object source, object target) => source.ApplyDiffs<object>(target);

        public static T Patch<T>(this T source, T target) => source.ApplyDiffs(target);

        public static T AcceptChanges<T>(this T target, T source) => source.ApplyDiffs(target);

        public static T SendChanges<T>(this T source, T target) => source.ApplyDiffs(target);

        public static T AcceptPatch<T>(this T target, T source) => source.ApplyDiffs(target);

        public static T SendPatches<T>(this T source, T target) => source.ApplyDiffs(target);

        public static T PatchFrom<T>(this T target, T source) => source.ApplyDiffs(target);

        public static T PatchTo<T>(this T sources, T target) => sources.ApplyDiffs(target);

        public static T AcceptUpdates<T>(this T target, T source) => source.ApplyDiffs(target);

        public static T SendUpdates<T>(this T source, T target) => source.ApplyDiffs(target);
    }
}