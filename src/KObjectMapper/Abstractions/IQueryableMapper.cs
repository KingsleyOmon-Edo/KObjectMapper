using System.Linq.Expressions;

namespace KObjectMapper.Abstractions;

public interface IQueryableMapper
{
    IQueryable<TTarget> ProjectTo<TSource, TTarget>(IQueryable<TSource> source)
        where TTarget : new();

    Expression<Func<TSource, TTarget>> GetProjectionExpression<TSource, TTarget>()
        where TTarget : new();
}
