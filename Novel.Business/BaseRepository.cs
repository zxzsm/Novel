using Microsoft.EntityFrameworkCore;
using Novel.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Novel.Service
{
    public class BaseRepository<T> : IDisposable where T : class
    {
        public BookContext Db { get; set; } = new BookContext();



        //添加
        public T AddEntities(T entity)
        {
            Db.Entry<T>(entity).State = EntityState.Added;
            Db.SaveChanges();
            return entity;
        }

        //修改
        public bool UpdateEntities(T entity)
        {
            Db.Set<T>().Attach(entity);
            Db.Entry<T>(entity).State = EntityState.Modified;
            return Db.SaveChanges() > 0;
        }

        //删除
        public bool DeleteEntities(T entity)
        {
            Db.Set<T>().Attach(entity);
            Db.Entry<T>(entity).State = EntityState.Deleted;
            return Db.SaveChanges() > 0;
        }

        //查询
        public IQueryable<T> LoadEntities(Func<T, bool> wherelambda)
        {
            return Db.Set<T>().Where<T>(wherelambda).AsQueryable();
        }

        //分页
        public IQueryable<T> LoadPagerEntities<S>(int pageSize, int pageIndex, Func<T, S> orderByLambda, out int total, out int totalPage,
            bool isAsc = true, params Func<T, bool>[] whereLambda)
        {
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }
            var tempData = Db.Set<T>().AsQueryable();
            if (whereLambda != null && whereLambda.Length > 0)
            {
                foreach (var item in whereLambda)
                {
                    Expression<Func<T, bool>> expression = t => item(t);
                    tempData = tempData.Where(expression);
                }
            }
            total = tempData.Count();
            totalPage = (int)Math.Ceiling(total / (double)pageSize);
            if (pageIndex > totalPage)
            {
                pageIndex = 1;
            }
            //排序获取当前页的数据
            if (isAsc)
            {
                tempData = tempData.OrderBy<T, S>(orderByLambda).
                      Skip<T>(pageSize * (pageIndex - 1)).
                      Take<T>(pageSize).AsQueryable();
            }
            else
            {
                tempData = tempData.OrderByDescending<T, S>(orderByLambda).
                     Skip<T>(pageSize * (pageIndex - 1)).
                     Take<T>(pageSize).AsQueryable();
            }
            return tempData.AsQueryable();
        }


        public void Dispose()
        {
            using (Db)
            {

            }
        }
    }
}
