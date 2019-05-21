
using Novel.Entity.ViewModels;
using Novel.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Novel.Entity.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace Novel.Service
{
    public class BookRecommendService : BaseRepository<BookRecommend>
    {
        public PaginatedList<BookRecommendViewModel> GetBookRecommends(BaseSimpleViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new BaseSimpleViewModel
                {
                    ps = 10,
                    pi = 1,
                };
            }
            if (viewModel.pi <= 0)
            {
                viewModel.pi = 1;
            }
            if (viewModel.ps <= 0)
            {
                viewModel.ps = 10;
            }
            string countSql = @"SELECT count(1)
  FROM [dbo].[BookRecommend] a inner join Book b on a.BookId=b.BookId";
            string sql = @"
SELECT a.[Id]
      ,a.[BookId]
      ,a.[DataYM]
      ,a.[Order]
      ,a.[Created],
	  b.BookName
  FROM [dbo].[BookRecommend] a inner join Book b on a.BookId=b.BookId order by a.id  offset @pageindex rows fetch next @pagesize rows only";
            int pageindex = (viewModel.pi - 1) * viewModel.ps;

            List<SqlParameter> sqlParameters = new List<SqlParameter> {
                new SqlParameter("pageindex",pageindex),
                new SqlParameter("pagesize",viewModel.ps)
            };
            var q = Db.Database.SqlQuery<BookRecommendViewModel>(sql, sqlParameters.ToArray());
            var dt = Db.Database.SqlQuery(countSql);
            int count = dt != null && dt.Rows.Count != 0 ? dt.Rows[0].ToString().AsInt() : 0;

            return new PaginatedList<BookRecommendViewModel>(q, count, viewModel.pi, viewModel.ps);
        }

        public BookRecommendViewModel GetBookRecommendViewModel(int id)
        {
            string sql = @"
SELECT a.[Id]
      ,a.[BookId]
      ,a.[DataYM]
      ,a.[Order]
      ,a.[Created],
	  b.BookName
  FROM [dbo].[BookRecommend] a inner join Book b on a.BookId=b.BookId where a.Id=@Id ";

            List<SqlParameter> sqlParameters = new List<SqlParameter> {
                new SqlParameter("Id",id)
            };
            var q = Db.Database.SqlQuery<BookRecommendViewModel>(sql, sqlParameters.ToArray());
            return q != null && q.Count != 0 ? q.First() : null;
        }

        public List<Book> GetAllBooks(string name)
        {
            return Db.Book.Where(m=>m.BookName.Contains(name)).Take(10).Select(m => new Book { BookId = m.BookId, BookName = m.BookName }).ToList();
        }
    }
}
