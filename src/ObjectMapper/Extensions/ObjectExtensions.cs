namespace ObjectMapper.Extensions
{
    using System.Reflection;
    using Helpers;

    public static class ObjectExtensions
    {
        #region "Object Validators - Validators"

        private static void ValidateParameters<T>(T source, T target)
        {
            Checker.NullChecks(source, target);

            Checker.TypeChecks(source, target);
        }

        private static void PropertyNullChecks<T>(T sourceObject, PropertyInfo sourceProp, T targetObject,
            PropertyInfo targetProp)
        {
            Checker.NullChecks(sourceObject, targetObject);
            Checker.NullChecks(sourceProp, targetProp);
        }

        private static void ComparePropertyTypes(PropertyInfo sourceProp, PropertyInfo targetProp,
            out Type sourcePropType, out Type targetPropType)
        {
            sourcePropType = sourceProp.PropertyType;
            targetPropType = targetProp.PropertyType;
            if (Equals(sourcePropType, targetPropType) == false)
            {
                throw new ArgumentException(
                    $"PropertyTypes: {nameof(sourceProp)} and {targetProp} have dissimilar types");
            }
        }

        private static void NullChecks<T>(T source, T target)
        {
            source = source ?? throw new ArgumentNullException(nameof(source));
            target = target ?? throw new ArgumentNullException(nameof(target));
        }

        #endregion

        #region "Predicate logic"

        public static void ArePropValuesDifferent(object sourceObject, PropertyInfo sourceProp, object targetObject,
            PropertyInfo targetProp)
        {
            ObjectExtensions.ArePropValuesDifferent<object>(sourceObject, sourceProp, targetObject, targetProp);
        }

        public static bool ArePropValuesDifferent<T>(T sourceObject, PropertyInfo sourceProp, T targetObject,
            PropertyInfo targetProp)
        {
            ObjectExtensions.PropertyNullChecks(sourceObject, sourceProp, targetObject, targetProp);
            Checker.PropertyNameCheck(sourceProp, targetProp);
            Type sourcePropType, targetPropType;
            ObjectExtensions.ComparePropertyTypes(sourceProp, targetProp, out sourcePropType, out targetPropType);

            var sourcePropValue = Convert.ChangeType(sourceProp.GetValue(sourceObject), sourcePropType);
            var targetPropValue = Convert.ChangeType(targetProp.GetValue(targetObject), targetPropType);

            if (Equals(sourcePropValue, targetPropValue))
            {
                return true;
            }

            return false;
        }

        #endregion

        #region "Object queries - Queries"

        public static List<PropertyInfo> GetPropertyDiffs<T>(this T source, T target)
        {
            Checker.NullChecks(source, target);
            Checker.TypeChecks(source, target);
            var diffs = ObjectExtensions.ComputeDiffs(source, target);

            return diffs; // Shorter
        }

        public static List<PropertyInfo> GetPropertyDiffs(this object source, object target)
        {
            Checker.NullChecks(source, target);
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
                    PropertyInfo targetProp) => ObjectExtensions.ArePropValuesDifferent<object>(sourceObject,
                    sourceProp,
                    targetObject,
                    targetProp), source, target)
                .ToList();
        }

        #endregion

        #region "Object mutators - Commands"

        public static object ApplyDiffsTo(this object source, object target)
        {
            ObjectExtensions.NullChecks(source, target);

            var diffs = source.GetPropertyDiffs(target);
            var sourceProps = source.GetType().GetProperties();

            ObjectExtensions.WriteToProperties(source, target, diffs);

            return target;
        }

        public static T ApplyDiffsTo<T>(this T source, T target)
        {
            ObjectExtensions.ValidateParameters(source, target);

            var diffs = source.GetPropertyDiffs(target);
            var sourceProps = source.GetType().GetProperties();
            ObjectExtensions.WriteToProperties(source, target, diffs);

            return target;
        }

        private static void WriteToProperties<T>(T source, T target, List<PropertyInfo> diffs)
        {
            //  LIKELY LOCATION OF CHANGES
            //  ----------------------------
            /*  Given there are different ways to alter types based on
             *  whether they are mutable or immutable, classes, records,
             *  I need a different strategy for each type.
             *
             *  Option 1:
             *  ----------
             *  So using the "strategy design" pattern encapsulate the
             *  respective write algorithms, one each for classes, records,
             *  structs, etc. Of if I choose to classify types as mutable
             *  and immutable, then one each for mutable types and immutable types.
             *
             *  Then next we need a means of dynamically selecting form amongst these
             *  respective strategies. We could use the "factory design pattern" for this.
             *  The factory would implement the strategy selection logic thus fulfilling the
             *  selector responsibility.
             *
             *  The factory would need to determine the types and other relevant characteristics
             *  of the supplied objects, to make the selection decision. Then it determines
             *  and returns the right strategy for the given object.
             *
             *  This method would simply just invoke the strategy passing the supplied object(s).
             *  Hopefully, with minimal changes to the rest of the system.
             */

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

        public static object SendUpdatesTo(this object source, object target) => source.ApplyDiffsTo<object>(target);

        public static T Patch<T>(this T source, T target) => source.ApplyDiffsTo(target);

        public static T AcceptChanges<T>(this T target, T source) => source.ApplyDiffsTo(target);

        public static T SendChanges<T>(this T source, T target) => source.ApplyDiffsTo(target);

        public static T AcceptPatch<T>(this T target, T source) => source.ApplyDiffsTo(target);

        public static T SendPatches<T>(this T source, T target) => source.ApplyDiffsTo(target);

        public static T PatchFrom<T>(this T target, T source) => source.ApplyDiffsTo(target);

        public static T PatchTo<T>(this T sources, T target) => sources.ApplyDiffsTo(target);

        public static T AcceptUpdates<T>(this T target, T source) => source.ApplyDiffsTo(target);

        public static T SendUpdates<T>(this T source, T target) => source.ApplyDiffsTo(target);

        #endregion
    }
}