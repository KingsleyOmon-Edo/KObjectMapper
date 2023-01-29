namespace KObjectMapper
{
    using System.Reflection;
    using Extensions;
    using Helpers;

    public class MappingService
    {
        private readonly MutableWriter _mutableWriter;

        private MappingService()
        {
            _mutableWriter = new MutableWriter();
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
            
            _mutableWriter.WriteToProperties(source, target, diffs);

            return target;
        }

        private T ApplyDiffs<T>(T source, T target)
        {
            new MappingService().ValidateParameters(source, target);

            var diffs = MappingService.GetPropertyDiffs(source, target);
            var sourceProps = source.GetType().GetProperties();
            new MappingService()._mutableWriter.WriteToProperties(source, target, diffs);

            return target;
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