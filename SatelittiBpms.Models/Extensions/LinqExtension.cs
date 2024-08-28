using System;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Models.Extensions
{
    public static class LinqExtension
    {

        public static IEnumerable<TSource> WhereIf<TSource>(
           this IEnumerable<TSource> source,
           bool condition,
           Func<TSource, bool> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }

        public static IEnumerable<TSource> OrderSort<TSource>(
            this IEnumerable<TSource> source,
            bool isAsc,
            Func<TSource, object> keySelector)
        {
            return (isAsc) ? source.OrderBy(keySelector) : source.OrderByDescending(keySelector);
        }
    }
}
