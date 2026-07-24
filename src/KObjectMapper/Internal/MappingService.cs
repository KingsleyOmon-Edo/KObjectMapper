// Copyright (c) KObjectMapper contributors. All rights reserved.
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using KObjectMapper.Extensions;

namespace KObjectMapper.Internal;

/// <summary>
/// Performs object-to-object difference detection and property application.
/// </summary>
public class MappingService
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new();

    private MappingService()
    {
    }

    /// <summary>
    /// Creates a new <see cref="MappingService" /> instance.
    /// </summary>
    public static MappingService Create() => new();

    private static void ValidateParameters<T>(T source, T target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        if (!typeof(T).IsAssignableFrom(source.GetType()) || !typeof(T).IsAssignableFrom(target.GetType()))
        {
            throw new ArgumentException(
                $"{nameof(source)} and {nameof(target)} objects should be compatible with the supplied generic type");
        }
    }

    private static void PropertyNullChecks<T>(T sourceObject, PropertyInfo sourceProp, T targetObject,
        PropertyInfo targetProp)
    {
        ArgumentNullException.ThrowIfNull(sourceObject);
        ArgumentNullException.ThrowIfNull(targetObject);
        ArgumentNullException.ThrowIfNull(sourceProp);
        ArgumentNullException.ThrowIfNull(targetProp);
    }

    private static void ComparePropertyTypes(PropertyInfo sourceProp, PropertyInfo targetProp,
        out Type sourcePropType, out Type targetPropType)
    {
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

    /// <summary>
    /// Determines whether the specified source and target property values differ.
    /// </summary>
    /// <param name="sourceObject">The source object.</param>
    /// <param name="sourceProp">The source property.</param>
    /// <param name="targetObject">The target object.</param>
    /// <param name="targetProp">The target property.</param>
    /// <summary>
    /// Determines whether the specified source and target property values differ.
    /// </summary>
    /// <param name="sourceObject">The source object.</param>
    /// <param name="sourceProp">The source property.</param>
    /// <param name="targetObject">The target object.</param>
    /// <param name="targetProp">The target property.</param>
    public void ArePropValuesDifferent(object sourceObject, PropertyInfo sourceProp, object targetObject,
        PropertyInfo targetProp)
    {
        _ = this.ArePropValuesSame<object>(sourceObject, sourceProp, targetObject, targetProp);
    }

    /// <summary>
    /// Determines whether the specified source and target property values are equal.
    /// </summary>
    /// <param name="sourceObject">The source object.</param>
    /// <param name="sourceProp">The source property.</param>
    /// <param name="targetObject">The target object.</param>
    /// <param name="targetProp">The target property.</param>
    /// <summary>
    /// Determines whether the specified source and target property values are equal.
    /// </summary>
    /// <param name="sourceObject">The source object.</param>
    /// <param name="sourceProp">The source property.</param>
    /// <param name="targetObject">The target object.</param>
    /// <param name="targetProp">The target property.</param>
    public bool ArePropValuesSame(object sourceObject, PropertyInfo sourceProp, object targetObject,
        PropertyInfo targetProp)
        => this.ArePropValuesSame<object>(sourceObject, sourceProp, targetObject, targetProp);

    /// <summary>
    /// Determines whether the specified source and target property values are equal.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    /// <param name="sourceObject">The source object.</param>
    /// <param name="sourceProp">The source property.</param>
    /// <param name="targetObject">The target object.</param>
    /// <param name="targetProp">The target property.</param>
    /// <summary>
    /// Determines whether the specified source and target property values are equal.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    /// <param name="sourceObject">The source object.</param>
    /// <param name="sourceProp">The source property.</param>
    /// <param name="targetObject">The target object.</param>
    /// <param name="targetProp">The target property.</param>
    public bool ArePropValuesSame<T>(T sourceObject, PropertyInfo sourceProp, T targetObject,
        PropertyInfo targetProp)
    {
        PropertyNullChecks(sourceObject, sourceProp, targetObject, targetProp);

        if (!string.Equals(sourceProp.Name, targetProp.Name, StringComparison.Ordinal))
        {
            throw new ArgumentException(
                $"PropertyNames: {nameof(sourceProp)} and {targetProp} have dissimilar names");
        }

        Type sourcePropType, targetPropType;
        ComparePropertyTypes(sourceProp, targetProp, out sourcePropType, out targetPropType);

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

    /// <summary>
    /// Determines whether the specified source and target property values differ.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    /// <param name="sourceObject">The source object.</param>
    /// <param name="sourceProp">The source property.</param>
    /// <param name="targetObject">The target object.</param>
    /// <param name="targetProp">The target property.</param>
    /// <summary>
    /// Determines whether the specified source and target property values differ.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    /// <param name="sourceObject">The source object.</param>
    /// <param name="sourceProp">The source property.</param>
    /// <param name="targetObject">The target object.</param>
    /// <param name="targetProp">The target property.</param>
    public bool ArePropValuesDifferent<T>(T sourceObject, PropertyInfo sourceProp, T targetObject,
        PropertyInfo targetProp)
        => !this.ArePropValuesSame(sourceObject, sourceProp, targetObject, targetProp);

    /// <summary>
    /// Returns the properties whose values differ between the source and target objects.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    /// <param name="source">The source object.</param>
    /// <param name="target">The target object.</param>
    /// <returns>The differing properties.</returns>
    /// <summary>
    /// Returns the properties whose values differ between the source and target objects.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    /// <param name="source">The source object.</param>
    /// <param name="target">The target object.</param>
    /// <returns>The differing properties.</returns>
    public List<PropertyInfo> GetPropertyDiffs<T>(T source, T target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        if (!typeof(T).IsAssignableFrom(source.GetType()) || !typeof(T).IsAssignableFrom(target.GetType()))
        {
            throw new ArgumentException(
                $"{nameof(source)} and {nameof(target)} objects should be compatible with the supplied generic type");
        }

        return this.GetPropertyDiffsInternal(source, target);
    }

    /// <summary>
    /// Returns the properties whose values differ between the source and target objects.
    /// </summary>
    /// <param name="source">The source object.</param>
    /// <param name="target">The target object.</param>
    /// <returns>The differing properties.</returns>
    /// <summary>
    /// Returns the properties whose values differ between the source and target objects.
    /// </summary>
    /// <param name="source">The source object.</param>
    /// <param name="target">The target object.</param>
    /// <returns>The differing properties.</returns>
    public List<PropertyInfo> GetPropertyDiffs(object source, object target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        return this.GetPropertyDiffsInternal(source, target);
    }

    private List<PropertyInfo> GetPropertyDiffsInternal(object source, object target)
    {
        var sourceProps = MappingService.GetCachedProperties(source.GetType()).ToList();
        var targetProps = MappingService.GetCachedProperties(target.GetType()).ToList();

        return sourceProps.Except(targetProps, this.ArePropValuesSameForDiffs, source, target)
            .ToList();
    }

    private bool ArePropValuesSameForDiffs(object sourceObject, PropertyInfo sourceProp, object targetObject,
        PropertyInfo targetProp)
        => this.ArePropValuesSame<object>(sourceObject, sourceProp, targetObject, targetProp);

    /// <summary>
    /// Applies the source property values to the target object.
    /// </summary>
    /// <param name="source">The source object.</param>
    /// <param name="target">The target object.</param>
    /// <returns>The mapped target object.</returns>
    /// <summary>
    /// Applies the source property values to the target object.
    /// </summary>
    /// <param name="source">The source object.</param>
    /// <param name="target">The target object.</param>
    /// <returns>The mapped target object.</returns>
    public object ApplyDiffs(object source, object target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        var diffs = this.GetPropertyDiffsInternal(source, target);
        this.WriteToProperties(source, target, diffs);

        return target;
    }

    /// <summary>
    /// Applies the source property values to the target object.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    /// <param name="source">The source object.</param>
    /// <param name="target">The target object.</param>
    /// <returns>The mapped target object.</returns>
    /// <summary>
    /// Applies the source property values to the target object.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    /// <param name="source">The source object.</param>
    /// <param name="target">The target object.</param>
    /// <returns>The mapped target object.</returns>
    public T ApplyDiffs<T>(T source, T target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        ValidateParameters(source, target);

        var diffs = this.GetPropertyDiffsInternal(source, target);
        this.WriteToProperties(source, target, diffs);

        return target;
    }

    /// <summary>
    /// Applies the source property values to the target object with circular reference and depth tracking.
    /// </summary>
    /// <param name="source">The source object.</param>
    /// <param name="target">The target object.</param>
    /// <param name="visited">Tracks already-visited source objects to detect circular references.</param>
    /// <param name="depth">The current recursion depth.</param>
    /// <param name="maxDepth">The maximum allowed recursion depth.</param>
    /// <returns>The mapped target object.</returns>
    public object ApplyDiffs(object source, object target, Dictionary<object, object> visited, int depth, int maxDepth)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        if (depth > maxDepth)
            throw new InvalidOperationException($"Maximum mapping depth of {maxDepth} exceeded.");

        if (visited.TryGetValue(source, out object? existing))
            return existing;

        visited[source] = target;

        var diffs = this.GetPropertyDiffsInternal(source, target);
        this.WriteToPropertiesWithGraph(source, target, diffs, visited, depth, maxDepth);

        return target;
    }

    private void WriteToPropertiesWithGraph<T>(T source, T target, List<PropertyInfo> diffs,
        Dictionary<object, object> visited, int depth, int maxDepth)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        PropertyInfo[] targetProperties = MappingService.GetCachedProperties(target!.GetType());

        foreach (PropertyInfo sourceProp in diffs)
        {
            PropertyInfo? targetProp = GetMatchingTargetProperty(sourceProp, targetProperties);

            if (targetProp is null)
            {
                continue;
            }

            this.WritePropertyValueWithGraph(source, sourceProp, target, targetProp, visited, depth, maxDepth);
        }
    }

    private void WritePropertyValueWithGraph(object source, PropertyInfo sourceProp, object target,
        PropertyInfo targetProp, Dictionary<object, object> visited, int depth, int maxDepth)
    {
        object? sourceValue = sourceProp.GetValue(source);
        object? targetValue = targetProp.GetValue(target);

        if (sourceValue is null)
        {
            if (targetValue is not null)
            {
                targetProp.SetValue(target, null);
            }

            return;
        }

        if (CanMapNestedObjects(sourceProp.PropertyType, targetProp.PropertyType))
        {
            if (visited.TryGetValue(sourceValue, out object? alreadyMapped))
            {
                targetProp.SetValue(target, alreadyMapped);
                return;
            }

            object nestedTarget = targetValue ?? CreateNestedTarget(targetProp.PropertyType);
            this.ApplyDiffs(sourceValue, nestedTarget, visited, depth + 1, maxDepth);

            if (targetValue is null)
            {
                targetProp.SetValue(target, nestedTarget);
            }

            return;
        }

        object mappedValue = ConvertValue(sourceValue, targetProp.PropertyType);

        if (!Equals(mappedValue, targetValue))
        {
            targetProp.SetValue(target, mappedValue);
        }
    }

    private void WriteToProperties<T>(T source, T target, List<PropertyInfo> diffs)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        PropertyInfo[] targetProperties = MappingService.GetCachedProperties(target.GetType());

        foreach (PropertyInfo sourceProp in diffs)
        {
            PropertyInfo? targetProp = GetMatchingTargetProperty(sourceProp, targetProperties);

            if (targetProp is null)
            {
                continue;
            }

            this.WritePropertyValue(source, sourceProp, target, targetProp);
        }
    }

    private static PropertyInfo? GetMatchingTargetProperty(PropertyInfo sourceProp, IEnumerable<PropertyInfo> targetProperties)
        => targetProperties.FirstOrDefault(targetProp =>
            sourceProp.Name == targetProp.Name && targetProp.CanWrite && targetProp.SetMethod is not null);

    private void WritePropertyValue(object source, PropertyInfo sourceProp, object target, PropertyInfo targetProp)
    {
        object? sourceValue = sourceProp.GetValue(source);
        object? targetValue = targetProp.GetValue(target);

        if (sourceValue is null)
        {
            if (targetValue is not null)
            {
                targetProp.SetValue(target, null);
            }

            return;
        }

        if (CanMapNestedObjects(sourceProp.PropertyType, targetProp.PropertyType))
        {
            WriteNestedPropertyValue(sourceValue, target, targetProp, targetValue);
            return;
        }

        object mappedValue = ConvertValue(sourceValue, targetProp.PropertyType);

        if (!Equals(mappedValue, targetValue))
        {
            targetProp.SetValue(target, mappedValue);
        }
    }

    private void WriteNestedPropertyValue(object sourceValue, object target, PropertyInfo targetProp,
        object? targetValue)
    {
        object nestedTarget = targetValue ?? CreateNestedTarget(targetProp.PropertyType);
        this.ApplyDiffs(sourceValue, nestedTarget);

        if (targetValue is null)
        {
            targetProp.SetValue(target, nestedTarget);
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

}
