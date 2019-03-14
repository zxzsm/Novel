using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Novel.Utilities
{
    public static class LinqExtent
    {


        public static PaginatedList<TResult> Query<TEntity, TOrderBy, TResult>(this IQueryable<TEntity> query, int index, int pageSize,

                                                         Expression<Func<TEntity, TOrderBy>> orderby,
                                                         Func<IQueryable<TEntity>, List<TResult>> selector, bool isAsc = true, List<Expression<Func<TEntity, bool>>> wheres = null)
        {
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            PageInfo.CheckPageIndexAndSize(ref index, ref pageSize);
            IQueryable<TEntity> queryable = query;
            if (wheres != null)
            {
                wheres.ForEach(p => queryable = p != null ? queryable.Where(p) : queryable);
            }

            int count = queryable.Count();
            PageInfo.CheckPageIndexAndSize(ref index, pageSize, count);
            if (count > 0)
            {
                if (orderby != null)
                {
                    queryable = isAsc ? queryable.OrderBy(orderby) : queryable.OrderByDescending(orderby);
                }
                queryable = queryable.Skip(pageSize * (index - 1)).Take(pageSize);
                return new PaginatedList<TResult>(selector(queryable), count, index, pageSize);
            }

            return new PaginatedList<TResult>(new List<TResult>(), count, index, pageSize);
        }

    }
}
