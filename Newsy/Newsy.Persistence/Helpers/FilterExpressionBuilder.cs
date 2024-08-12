using Newsy.Abstractions.Models;
using System.Linq.Expressions;

namespace Newsy.Persistence.Helpers;
public static class FilterExpressionBuilder
{
    public static Expression<Func<T, bool>> GetFiltersExpression<T>(IEnumerable<Filter> filters)
        where T : class
    {
        IEnumerable<Filter> enumerableFilters = filters as Filter[] ?? filters.ToArray();

        if (!enumerableFilters.Any())
        {
            return x => true;
        }

        var param = Expression.Parameter(typeof(T), "x");
        var expression = GetFilterExpression<T>(enumerableFilters.First(), param);
        for (int i = 1; i < enumerableFilters.Count(); i++)
        {
            expression = Expression.And(expression, GetFilterExpression<T>(enumerableFilters.ElementAt(i), param));
        }

        return Expression.Lambda<Func<T, bool>>(expression, param);
    }

    private static Expression GetFilterExpression<T>(Filter filter, ParameterExpression param)
        where T : class
    {
        var filterFieldExpression = BuildFilterFieldPropertyExpression(filter, param);

        var propertyType = filterFieldExpression.Type;
        var valueConverted = ConvertValue(filter.Value, propertyType);

        var filterExpression = Expression.Equal(filterFieldExpression, Expression.Constant(valueConverted, propertyType));
        return filterExpression;
    }

    private static Expression BuildFilterFieldPropertyExpression(Filter filter, ParameterExpression param)
    {
        var expr = filter.Field.Split('.')
            .Aggregate<string, Expression>
            (param, (c, m) =>
            {
                MemberExpression prop;
                try
                {
                    prop = Expression.Property(c, m);
                }
                catch (Exception)
                {
                    throw new Exception($"Provided invalid filter field name {filter.Field}");
                }

                return prop;
            });

        if (expr.Type == typeof(string))
            expr = Expression.Call(expr, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!);

        return expr;
    }

    private static object? ConvertValue(string value, Type propType)
    {
        if (propType == typeof(string))
        {
            return value.ToLower();
        }

        if (propType == typeof(Guid?))
        {
            if (value == null)
            {
                return null;
            }
            else if (Guid.TryParse(value, out Guid result))
            {
                return (Guid?)result;
            }
            else
            {
                throw new Exception($"Value: {value} must be in valid Guid format");
            }
        }

        return Convert.ChangeType(value, propType);
    }
}

