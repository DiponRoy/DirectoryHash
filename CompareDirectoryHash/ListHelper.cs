using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CompareDirectoryHash
{
    public static class ListHelper
    {
        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
            {
                return source.Where<TSource>(predicate.Compile());
            }
            else
            {
                return source;
            }
        }

        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
            {
                return source.Where(predicate);
            }
            else
            {
                return source;
            }
        }
    }
}
