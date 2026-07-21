// Copyright (c) KObjectMapper contributors. All rights reserved.
using System.Globalization;
using System.Reflection;
using KObjectMapper.Extensions;
using KObjectMapper.Helpers;

namespace KObjectMapper;

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
        _ = Checker.NullChecks(sourceObject, targetObject);
        _ = Checker.NullChecks(sourceProp, targetProp);
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

        object? sourcePropValue = sourceProp.GetValue(sourceObject);
        object? targetPropValue = targetProp.GetValue(targetObject);

        if (sourcePropValue is null || targetPropValue is null)
        {
            return Equals(sourcePropValue, targetPropValue);
        }

        object sourceValue = Convert.ChangeType(sourcePropValue, sourcePropType, CultureInfo.InvariantCulture)!;
        object targetValue = Convert.ChangeType(targetPropValue, targetPropType, CultureInfo.InvariantCulture)!;

        return Equals(sourceValue, targetValue);
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
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

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
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        object safeSource = source;
        object safeTarget = target;

        var diffs = MappingService.GetPropertyDiffs(safeSource, safeTarget);
        var sourceProps = safeSource.GetType().GetProperties();

        _ = sourceProps;
        MappingService.WriteToProperties(safeSource, safeTarget, diffs);

        return safeTarget;
    }

    public T ApplyDiffs<T>(T source, T target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        T safeSource = source;
        T safeTarget = target;

        MappingService.ValidateParameters(safeSource, safeTarget);

        var diffs = MappingService.GetPropertyDiffs(safeSource, safeTarget);
        var sourceProps = safeSource.GetType().GetProperties();
        _ = sourceProps;
        MappingService.WriteToProperties(safeSource, safeTarget, diffs);

        return safeTarget;
    }

    private static void WriteToProperties<T>(T source, T target, List<PropertyInfo> diffs)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        foreach (var sourceProp in diffs)
        {
            foreach (var targetProp in target.GetType().GetProperties())
            {
                if (sourceProp.Name == targetProp.Name)
                {
                    object? sourceValue = sourceProp.GetValue(source);
                    object? targetValue = targetProp.GetValue(target);

                    if (!Equals(sourceValue, targetValue))
                    {
                        targetProp.SetValue(target, sourceValue);
                    }
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
