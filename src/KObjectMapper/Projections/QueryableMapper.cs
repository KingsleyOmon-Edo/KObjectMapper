using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using KObjectMapper.Abstractions;

namespace KObjectMapper.Projections;

public sealed class QueryableMapper : IQueryableMapper
{
    private readonly ConcurrentDictionary<(Type, Type), object> _cache = new();

    public Expression<Func<TSource, TTarget>> GetProjectionExpression<TSource, TTarget>()
        where TTarget : new()
    {
        (Type, Type) key = (typeof(TSource), typeof(TTarget));

        if (_cache.TryGetValue(key, out object? cached))
        {
            return (Expression<Func<TSource, TTarget>>)cached;
        }

        Expression<Func<TSource, TTarget>> expr = BuildExpression<TSource, TTarget>();
        _cache[key] = expr;
        return expr;
    }

    public IQueryable<TTarget> ProjectTo<TSource, TTarget>(IQueryable<TSource> source)
        where TTarget : new()
    {
        if (source is null)
        {
            throw new ProjectionException(
                typeof(TSource),
                typeof(TTarget),
                $"Source queryable cannot be null when projecting from '{typeof(TSource).Name}' to '{typeof(TTarget).Name}'.");
        }

        Expression<Func<TSource, TTarget>> expr = GetProjectionExpression<TSource, TTarget>();

        try
        {
            return source.Select(expr);
        }
        catch (Exception ex)
        {
            throw new ProjectionException(
                typeof(TSource),
                typeof(TTarget),
                $"Failed to project from '{typeof(TSource).Name}' to '{typeof(TTarget).Name}'. The expression could not be translated.",
                ex);
        }
    }

    private static Expression<Func<TSource, TTarget>> BuildExpression<TSource, TTarget>()
        where TTarget : new()
    {
        Type sourceType = typeof(TSource);
        Type targetType = typeof(TTarget);

        ParameterExpression param = Expression.Parameter(sourceType, "src");

        PropertyInfo[] sourceProps = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .ToArray();

        PropertyInfo[] targetProps = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToArray();

        List<MemberBinding> bindings = [];

        foreach (PropertyInfo targetProp in targetProps)
        {
            PropertyInfo? sourceProp = sourceProps.FirstOrDefault(sp =>
                sp.Name == targetProp.Name &&
                targetProp.PropertyType.IsAssignableFrom(sp.PropertyType));

            if (sourceProp is null)
            {
                continue;
            }

            MemberExpression sourceAccess = Expression.Property(param, sourceProp);
            MemberBinding binding = Expression.Bind(targetProp, sourceAccess);
            bindings.Add(binding);
        }

        if (bindings.Count == 0)
        {
            throw new ProjectionException(
                sourceType,
                targetType,
                $"No mappable properties found between '{sourceType.Name}' and '{targetType.Name}'. Ensure at least one property name and type matches.");
        }

        NewExpression newTarget = Expression.New(targetType);
        MemberInitExpression memberInit = Expression.MemberInit(newTarget, bindings);
        return Expression.Lambda<Func<TSource, TTarget>>(memberInit, param);
    }
}
