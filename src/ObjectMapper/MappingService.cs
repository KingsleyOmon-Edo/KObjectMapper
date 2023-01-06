namespace ObjectMapper
{
    using System.Reflection;
    using Extensions;
    using Helpers;

    public class MappingService
    {
        private MappingService()
        {
        }

        public static MappingService Create() => new();

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

        public static void ArePropValuesDifferent(object sourceObject, PropertyInfo sourceProp, object targetObject,
            PropertyInfo targetProp)
        {
            MappingService.ArePropValuesDifferent<object>(sourceObject, sourceProp, targetObject, targetProp);
        }

        public static bool ArePropValuesDifferent<T>(T sourceObject, PropertyInfo sourceProp, T targetObject,
            PropertyInfo targetProp)
        {
            MappingService.PropertyNullChecks(sourceObject, sourceProp, targetObject, targetProp);
            Checker.PropertyNameCheck(sourceProp, targetProp);
            Type sourcePropType, targetPropType;
            MappingService.ComparePropertyTypes(sourceProp, targetProp, out sourcePropType, out targetPropType);

            var sourcePropValue = Convert.ChangeType(sourceProp.GetValue(sourceObject), sourcePropType);
            var targetPropValue = Convert.ChangeType(targetProp.GetValue(targetObject), targetPropType);

            if (Equals(sourcePropValue, targetPropValue))
            {
                return true;
            }

            return false;
        }

        public static List<PropertyInfo> GetPropertyDiffs<T>(T source, T target)
        {
            Checker.NullChecks(source, target);
            Checker.TypeChecks(source, target);
            var diffs = MappingService.ComputeDiffs(source, target);

            return diffs; 
        }

        public static List<PropertyInfo> GetPropertyDiffs(object source, object target)
        {
            Checker.NullChecks(source, target);
            var diffs = MappingService.ComputeDiffs(source, target);

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
                    PropertyInfo targetProp) => MappingService.ArePropValuesDifferent<object>(sourceObject,
                    sourceProp,
                    targetObject,
                    targetProp), source, target)
                .ToList();
        }

        public object ApplyDiffs(object source, object target)
        {
            MappingService.NullChecks(source, target);

            var diffs = MappingService.GetPropertyDiffs(source, target);
            var sourceProps = source.GetType().GetProperties();

            MappingService.WriteToProperties(source, target, diffs);

            return target;
        }

        public T ApplyDiffs<T>(T source, T target)
        {
            MappingService.ValidateParameters(source, target);

            var diffs = MappingService.GetPropertyDiffs(source, target);
            var sourceProps = source.GetType().GetProperties();
            MappingService.WriteToProperties(source, target, diffs);

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

        public static object SendUpdatesTo(object source, object target) =>
            MappingService.Create().ApplyDiffs(source, target);

        public static T Patch<T>(T source, T target) => MappingService.Create().ApplyDiffs(source, target);

        public static T AcceptChanges<T>(T target, T source) =>
            MappingService.Create().ApplyDiffs(source, target);

        public static T SendChanges<T>(T source, T target) =>
            MappingService.Create().ApplyDiffs(source, target);

        public static T AcceptPatch<T>(T target, T source) =>
            MappingService.Create().ApplyDiffs(source, target);

        public static T SendPatches<T>(T source, T target) =>
            MappingService.Create().ApplyDiffs(source, target);

        public static T PatchFrom<T>(T target, T source) => MappingService.Create().ApplyDiffs(source, target);
        public static T PatchTo<T>(T sources, T target) => MappingService.Create().ApplyDiffs(sources, target);

        public static T AcceptUpdates<T>(T target, T source) =>
            MappingService.Create().ApplyDiffs(source, target);

        public static T SendUpdates<T>(T source, T target) =>
            MappingService.Create().ApplyDiffs(source, target);
    }
}