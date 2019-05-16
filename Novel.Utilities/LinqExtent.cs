using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

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


        private static DbCommand CreateCommand(DatabaseFacade facade, string sql, out DbConnection connection, params object[] parameters)
        {
            var conn = facade.GetDbConnection();
            connection = conn;
            conn.Open();
            var cmd = conn.CreateCommand();
            if (facade.IsSqlServer())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(parameters);
            }
            return cmd;
        }

        public static DataTable SqlQuery(this DatabaseFacade facade, string sql, params object[] parameters)
        {
            var command = CreateCommand(facade, sql, out DbConnection conn, parameters);
            var reader = command.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            conn.Close();
            return dt;
        }

        public static List<T> SqlQuery<T>(this DatabaseFacade facade, string sql, params object[] parameters) where T : class, new()
        {
            var dt = SqlQuery(facade, sql, parameters);
            return dt.ToList<T>();
        }

        public static List<T> ToList<T>(this DataTable dt) where T : class, new()
        {
            var propertyInfos = typeof(T).GetProperties();
            var list = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                var t = new T();
                foreach (PropertyInfo p in propertyInfos)
                {
                    if (dt.Columns.IndexOf(p.Name) != -1 && row[p.Name] != DBNull.Value)
                        p.SetValue(t, row[p.Name], null);
                }
                list.Add(t);
            }
            return list;
        }

    }
}
