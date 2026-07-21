// Copyright (c) KObjectMapper contributors. All rights reserved.
using System.Globalization;
using System.Reflection;
using KObjectMapper.Extensions;

namespace KObjectMapper;

public class MappingService
{
    private MappingService()
    {
    }

    public static MappingService Create() => new();

    private void ValidateParameters<T>(T source, T target)
    {
        _ = this.GetHashCode();
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        if (source.GetType() != target.GetType())
        {
            throw new ArgumentException(
                $"{nameof(source)} and {nameof(target)} objects should be of the same type");
        }
    }

    private void PropertyNullChecks<T>(T sourceObject, PropertyInfo sourceProp, T targetObject,
        PropertyInfo targetProp)
    {
        _ = this.GetHashCode();
        ArgumentNullException.ThrowIfNull(sourceObject);
        ArgumentNullException.ThrowIfNull(targetObject);
        ArgumentNullException.ThrowIfNull(sourceProp);
        ArgumentNullException.ThrowIfNull(targetProp);
    }

    private void ComparePropertyTypes(PropertyInfo sourceProp, PropertyInfo targetProp,
        out Type sourcePropType, out Type targetPropType)
    {
        _ = this.GetHashCode();
        ArgumentNullException.ThrowIfNull(sourceProp);
        ArgumentNullException.ThrowIfNull(targetProp);

        sourcePropType = sourceProp.PropertyType;
        targetPropType = targetProp.PropertyType;
        if (Equals(sourcePropType, targetPropType) == false)
        {
            throw new ArgumentException(
                $"PropertyTypes: {nameof(sourceProp)} and {targetProp} have dissimilar types");
        }
    }

    public void ArePropValuesDifferent(object sourceObject, PropertyInfo sourceProp, object targetObject,
        PropertyInfo targetProp)
    {
        this.ArePropValuesDifferent<object>(sourceObject, sourceProp, targetObject, targetProp);
    }

    public bool ArePropValuesDifferent<T>(T sourceObject, PropertyInfo sourceProp, T targetObject,
        PropertyInfo targetProp)
    {
        this.PropertyNullChecks(sourceObject, sourceProp, targetObject, targetProp);

        if (!string.Equals(sourceProp.Name, targetProp.Name, StringComparison.Ordinal))
        {
            throw new ArgumentException(
                $"PropertyNames: {nameof(sourceProp)} and {targetProp} have dissimilar names");
        }

        Type sourcePropType, targetPropType;
        this.ComparePropertyTypes(sourceProp, targetProp, out sourcePropType, out targetPropType);

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

    public List<PropertyInfo> GetPropertyDiffs<T>(T source, T target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        if (source.GetType() != target.GetType())
        {
            throw new ArgumentException(
                $"{nameof(source)} and {nameof(target)} objects should be of the same type");
        }

        var diffs = this.ComputeDiffs(source, target);

        return diffs;
    }

    public List<PropertyInfo> GetPropertyDiffs(object source, object target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        var diffs = this.ComputeDiffs(source, target);

        return diffs;
    }

    private List<PropertyInfo> ComputeDiffs<T>(T source, T target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        var sourceProps = source.GetType().GetProperties().ToList();
        var targetProps = target.GetType().GetProperties().ToList();

        return sourceProps.Except(targetProps, (
                object sourceObject,
                PropertyInfo sourceProp,
                object targetObject,
                PropertyInfo targetProp) => this.ArePropValuesDifferent<object>(sourceObject,
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

        var diffs = this.GetPropertyDiffs(safeSource, safeTarget);
        var sourceProps = safeSource.GetType().GetProperties();

        _ = sourceProps;
        this.WriteToProperties(safeSource, safeTarget, diffs);

        return safeTarget;
    }

    public T ApplyDiffs<T>(T source, T target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        T safeSource = source;
        T safeTarget = target;

        this.ValidateParameters(safeSource, safeTarget);

        var diffs = this.GetPropertyDiffs(safeSource, safeTarget);
        var sourceProps = safeSource.GetType().GetProperties();
        _ = sourceProps;
        this.WriteToProperties(safeSource, safeTarget, diffs);

        return safeTarget;
    }

    private void WriteToProperties<T>(T source, T target, List<PropertyInfo> diffs)
    {
        _ = this.GetHashCode();
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

    public object SendUpdatesTo(object source, object target) =>
        this.ApplyDiffs(source, target);

    public T Patch<T>(T source, T target) => this.ApplyDiffs(source, target);

    public T AcceptChanges<T>(T target, T source) =>
        this.ApplyDiffs(source, target);

    public T SendChanges<T>(T source, T target) =>
        this.ApplyDiffs(source, target);

    public T AcceptPatch<T>(T target, T source) =>
        this.ApplyDiffs(source, target);

    public T SendPatches<T>(T source, T target) =>
        this.ApplyDiffs(source, target);

    public T PatchFrom<T>(T target, T source) => this.ApplyDiffs(source, target);
    public T PatchTo<T>(T sources, T target) => this.ApplyDiffs(sources, target);

    public T AcceptUpdates<T>(T target, T source) =>
        this.ApplyDiffs(source, target);

    public T SendUpdates<T>(T source, T target) =>
        this.ApplyDiffs(source, target);
}
