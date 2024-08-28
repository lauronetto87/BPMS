using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SatelittiBpms.Data.Extensions
{
    public static class EfExtensions
    {
        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }

        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, int, bool> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }

        public static IQueryable<TSource> WhereIf<TSource>(
            this IQueryable<TSource> source,
            bool condition,
            Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }


        public static IQueryable<TSource> OrderSort<TSource>(
            this IQueryable<TSource> source,
            bool isAsc,
            Expression<Func<TSource, object>> keySelector)
        {
            return (isAsc) ? source.OrderBy(keySelector) : source.OrderByDescending(keySelector);
        }
    }
}
