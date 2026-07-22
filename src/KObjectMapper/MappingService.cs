// Copyright (c) KObjectMapper contributors. All rights reserved.
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using KObjectMapper.Extensions;

namespace KObjectMapper;

public class MappingService
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new();

    private MappingService()
    {
    }

    public static MappingService Create() => new();

    private void ValidateParameters<T>(T source, T target)
    {
        _ = this.GetHashCode();
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        if (!typeof(T).IsAssignableFrom(source.GetType()) || !typeof(T).IsAssignableFrom(target.GetType()))
        {
            throw new ArgumentException(
                $"{nameof(source)} and {nameof(target)} objects should be compatible with the supplied generic type");
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
        _ = this.ArePropValuesSame<object>(sourceObject, sourceProp, targetObject, targetProp);
    }

    public bool ArePropValuesSame(object sourceObject, PropertyInfo sourceProp, object targetObject,
        PropertyInfo targetProp)
        => this.ArePropValuesSame<object>(sourceObject, sourceProp, targetObject, targetProp);

    public bool ArePropValuesSame<T>(T sourceObject, PropertyInfo sourceProp, T targetObject,
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

    public bool ArePropValuesDifferent<T>(T sourceObject, PropertyInfo sourceProp, T targetObject,
        PropertyInfo targetProp)
        => !this.ArePropValuesSame(sourceObject, sourceProp, targetObject, targetProp);

    public List<PropertyInfo> GetPropertyDiffs<T>(T source, T target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        if (!typeof(T).IsAssignableFrom(source.GetType()) || !typeof(T).IsAssignableFrom(target.GetType()))
        {
            throw new ArgumentException(
                $"{nameof(source)} and {nameof(target)} objects should be compatible with the supplied generic type");
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

        var sourceProps = MappingService.GetCachedProperties(source.GetType()).ToList();
        var targetProps = MappingService.GetCachedProperties(target.GetType()).ToList();

        return sourceProps.Except(targetProps, (
                object sourceObject,
                PropertyInfo sourceProp,
                object targetObject,
                PropertyInfo targetProp) => this.ArePropValuesSame<object>(sourceObject,
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
        this.WriteToProperties(safeSource, safeTarget, diffs);

        return safeTarget;
    }

    private void WriteToProperties<T>(T source, T target, List<PropertyInfo> diffs)
    {
        _ = this.GetHashCode();
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        var targetProperties = MappingService.GetCachedProperties(target.GetType());

        foreach (var sourceProp in diffs)
        {
            foreach (var targetProp in targetProperties)
            {
                if (sourceProp.Name != targetProp.Name || !targetProp.CanWrite || targetProp.SetMethod is null)
                {
                    continue;
                }

                object? sourceValue = sourceProp.GetValue(source);
                object? targetValue = targetProp.GetValue(target);

                if (sourceValue is null)
                {
                    if (targetValue is not null)
                    {
                        targetProp.SetValue(target, null);
                    }

                    continue;
                }

                if (CanMapNestedObjects(sourceProp.PropertyType, targetProp.PropertyType))
                {
                    object nestedTarget = targetValue ?? CreateNestedTarget(targetProp.PropertyType);
                    this.ApplyDiffs(sourceValue, nestedTarget);

                    if (targetValue is null)
                    {
                        targetProp.SetValue(target, nestedTarget);
                    }

                    continue;
                }

                object mappedValue = ConvertValue(sourceValue, targetProp.PropertyType);

                if (!Equals(mappedValue, targetValue))
                {
                    targetProp.SetValue(target, mappedValue);
                }
            }
        }
    }

    private static bool CanMapNestedObjects(Type sourceType, Type targetType)
        => sourceType.IsClass && sourceType != typeof(string)
           && targetType.IsClass && targetType != typeof(string);

    private static object CreateNestedTarget(Type targetType)
    {
        object? instance = Activator.CreateInstance(targetType);

        return instance ?? throw new InvalidOperationException(
            $"Target type '{targetType.Name}' must have a parameterless constructor to map nested objects.");
    }

    private static object ConvertValue(object sourceValue, Type targetType)
    {
        if (targetType.IsInstanceOfType(sourceValue))
        {
            return sourceValue;
        }

        return Convert.ChangeType(sourceValue, targetType, CultureInfo.InvariantCulture)!;
    }

    private static PropertyInfo[] GetCachedProperties(Type type)
        => PropertyCache.GetOrAdd(type, static currentType => currentType.GetProperties());

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
