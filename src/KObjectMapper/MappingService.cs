namespace KObjectMapper
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

        private void ValidateParameters<T>(T source, T target)
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

        private static void ArePropValuesDifferent(object sourceObject, PropertyInfo sourceProp, object targetObject,
            PropertyInfo targetProp)
        {
            MappingService.ArePropValuesDifferent<object>(sourceObject, sourceProp, targetObject, targetProp);
        }

        private static bool ArePropValuesDifferent<T>(T sourceObject, PropertyInfo sourceProp, T targetObject,
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

        private static List<PropertyInfo> GetPropertyDiffs<T>(T source, T target)
        {
            Checker.NullChecks(source, target);
            Checker.TypeChecks(source, target);
            var diffs = MappingService.ComputeDiffs(source, target);

            return diffs; 
        }

        private static List<PropertyInfo> GetPropertyDiffs(object source, object target)
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

            new MappingService().WriteToProperties(source, target, diffs);

            return target;
        }

        private T ApplyDiffs<T>(T source, T target)
        {
            new MappingService().ValidateParameters(source, target);

            var diffs = MappingService.GetPropertyDiffs(source, target);
            var sourceProps = source.GetType().GetProperties();
            new MappingService().WriteToProperties(source, target, diffs);

            return target;
        }

        //  Algo
        //  Might need to convert MappingService methods from stateless static class to one with instance methods
        //  =================================================================
        //  1) By extract interface refactoring, move WriteToProperties to an interface, IMutator
        //  to model the behavior of an object mutator.
        //  2) Test for regressions.
        //  2) Via "Move to another type" or "method object" refactoring, MutableWriteStrategy, make this new type,
        //  the default implementation of IMutator.WriteToProperties<T>(). Test for regressions.
        //  3) Modify the signature to relax the constraint that source and target types should be of the same type  T
        //  4) Test for regressions. Fix if necessary
        //  5) Take with the interface and default implementation, as a guide provide another implementation for immutable types.
        //  Call this ImmutableWriteStrategy
        //  6) Test to confirm that the above object model could effectively write props for all common types.
        //  7) Test that collections - IEnumerable<T> also work.
        //  8) Now implement a factory that uses C#'s new type switching switch statement, to select based on types what strategy to supply at runtime
        //  based on the type of the "target" type. Use the MutableWriterStrategy as the default case or discard.
        private void WriteToProperties<T>(T source, T target, List<PropertyInfo> diffs)
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
             *  structs, etc. Or if I choose to classify types as mutable
             *  and immutable, then one each for mutable types and immutable types.
             *
             *  Then next we need a means of dynamically selecting from amongst these
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

        private static object SendUpdatesTo(object source, object target) =>
            MappingService.Create().ApplyDiffs(source, target);

        private static T Patch<T>(T source, T target) => MappingService.Create().ApplyDiffs(source, target);

        private static T AcceptChanges<T>(T target, T source) =>
            MappingService.Create().ApplyDiffs(source, target);

        private static T SendChanges<T>(T source, T target) =>
            MappingService.Create().ApplyDiffs(source, target);

        private static T AcceptPatch<T>(T target, T source) =>
            MappingService.Create().ApplyDiffs(source, target);

        private static T SendPatches<T>(T source, T target) =>
            MappingService.Create().ApplyDiffs(source, target);

        private static T PatchFrom<T>(T target, T source) => MappingService.Create().ApplyDiffs(source, target);
        private static T PatchTo<T>(T sources, T target) => MappingService.Create().ApplyDiffs(sources, target);

        private static T AcceptUpdates<T>(T target, T source) =>
            MappingService.Create().ApplyDiffs(source, target);

        private static T SendUpdates<T>(T source, T target) =>
            MappingService.Create().ApplyDiffs(source, target);
    }
}