﻿using Newsy.Abstractions.Enums;
using Newsy.Abstractions.Models;
using Newsy.Persistence.Exceptions;
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

        Expression filterExpression;
        if (filter.Type == OperatorType.Contains)
        {
            if (filterFieldExpression.Type != typeof(string)) //Contains operator available for string fields only
            {
                throw new InvalidFilterException("Contains filter can be done on string columns only");
            }

            var method = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
            var valueExpression = Expression.Constant(filter.Value.ToLower(), typeof(string));

            filterExpression = Expression.Call(filterFieldExpression, method, valueExpression);
        }
        else if (filter.Type == OperatorType.Equal)
        {
            var propertyType = filterFieldExpression.Type;

            object valueConverted;
            if (propertyType == typeof(string))
                valueConverted = filter.Value.ToLower();
            else
                valueConverted = ConvertValue(filter.Value, propertyType);

            filterExpression = Expression.Equal(filterFieldExpression, Expression.Constant(valueConverted, propertyType));
        }
        else 
        {
            throw new InvalidFilterException("Unsupported filter type.");
        }

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
                    throw new InvalidFilterException($"Provided invalid filter field name '{filter.Field}'.");
                }

                return prop;
            });

        if (expr.Type == typeof(string))
            expr = Expression.Call(expr, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes)!);

        return expr;
    }

    private static object ConvertValue(string value, System.Type propType)
    {
        if (propType == typeof(string))
        {
            return value.ToLower();
        }

        if (propType == typeof(Guid))
        {
            if (Guid.TryParse(value, out Guid result))
            {
                return result;
            }
            else
            {
                throw new InvalidFilterException($"Given value '{value}' must be in valid Guid format.");
            }
        }

        return Convert.ChangeType(value, propType);
    }
}

