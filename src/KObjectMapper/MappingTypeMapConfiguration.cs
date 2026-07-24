using System.Linq.Expressions;

namespace KObjectMapper;

/// <summary>
/// Provides fluent configuration for mapping between a source and target type.
/// </summary>
public sealed class MappingTypeMapConfiguration<TSource, TTarget>
{
    private readonly MappingTypeMap _typeMap;

    internal MappingTypeMapConfiguration(MappingTypeMap typeMap)
    {
        ArgumentNullException.ThrowIfNull(typeMap);
        _typeMap = typeMap;
    }

    internal MappingTypeMap TypeMap => _typeMap;

    /// <summary>
    /// Configures a custom member mapping from a source property to a differently named target property.
    /// </summary>
    /// <param name="sourceMember">Expression selecting the source property.</param>
    /// <param name="targetMember">Expression selecting the target property.</param>
    /// <returns>The configuration instance for fluent chaining.</returns>
    public MappingTypeMapConfiguration<TSource, TTarget> ForMember(
        Expression<Func<TSource, object?>> sourceMember,
        Expression<Func<TTarget, object?>> targetMember)
    {
        ArgumentNullException.ThrowIfNull(sourceMember);
        ArgumentNullException.ThrowIfNull(targetMember);

        string sourceMemberName = ExtractMemberName(sourceMember);
        string targetMemberName = ExtractMemberName(targetMember);

        _typeMap.AddCustomMemberMapping(sourceMemberName, targetMemberName);

        return this;
    }

    /// <summary>
    /// Configures a target member to be ignored during mapping.
    /// </summary>
    /// <param name="targetMember">Expression selecting the target property to ignore.</param>
    /// <returns>The configuration instance for fluent chaining.</returns>
    public MappingTypeMapConfiguration<TSource, TTarget> Ignore(
        Expression<Func<TTarget, object?>> targetMember)
    {
        ArgumentNullException.ThrowIfNull(targetMember);

        string targetMemberName = ExtractMemberName(targetMember);
        _typeMap.AddIgnoredMember(targetMemberName);

        return this;
    }

    private static string ExtractMemberName<T>(Expression<Func<T, object?>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        Expression body = expression.Body;

        if (body is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert)
        {
            body = unaryExpression.Operand;
        }

        if (body is MemberExpression memberExpression)
        {
            return memberExpression.Member.Name;
        }

        throw new ArgumentException(
            $"Expression '{expression}' must be a member access expression.",
            nameof(expression));
    }
}
