using Microsoft.EntityFrameworkCore;
using SK.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SK.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<TEntity> IgnoreQueryFilters<TEntity>(
            this IQueryable<TEntity> source,
            bool ignoreQueryFilter
        ) where TEntity : class, IEntity
        {
            return ignoreQueryFilter
                ? source.IgnoreQueryFilters()
                : source;
        }

        public static IQueryable<TSource> WhereIf<TSource>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            bool condition)
        {
            return condition
                ? source.Where(predicate)
                : source;
        }

        public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector,
            bool ascending)
        {
            return ascending
                ? source.OrderBy(keySelector)
                : source.OrderByDescending(keySelector);
        }

        public static IOrderedQueryable<TSource> ThenBy<TSource, TKey>(
            this IOrderedQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector,
            bool ascending)
        {
            return ascending
                ? source.OrderBy(keySelector)
                : source.OrderByDescending(keySelector);
        }
    }
}
