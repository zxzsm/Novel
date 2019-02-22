using Novel.Entity;
using Novel.Entity.Models;
using Novel.Entity.ViewModels;
using Novel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Novel.Service
{
    public class BookShelfService : BaseRepository<BookShelf>
    {
        public PaginatedList<MyBookShelfViewModel> GetBookShlef(int userId, int pageIndex, int pageSize)
        {
            Func<BookShelf, bool> whereLambda = m => m.UserId == userId;
            int total = 0;
            int totalPage = 0;
            var t = LoadPagerEntities(pageSize, pageIndex, m => m.UpdateTime, out total, out totalPage, whereLambda: whereLambda);
            var result = t.Select(m => new MyBookShelfViewModel
            {
                id = m.Id,
                bookid = m.BookId,
                currentreaditemid = m.ReadItemId.HasValue ? m.ReadItemId.Value : 0
            }).ToList();
            var books = Db.Book.Where(m => result.Any(p => p.bookid == m.BookId)).ToList();
            foreach (var item in result)
            {
                var b = books.FirstOrDefault(m => m.BookId == item.bookid);
                if (b == null)
                {
                    continue;
                }
                item.bookname = b.BookName;
                item.bookauthor = b.BookAuthor;
                item.bookimage = b.BookImage;
                BookItem bookItem = null;
                if (item.currentreaditemid > 0)
                {
                    bookItem = Db.BookItem.FirstOrDefault(m => m.ItemId == item.currentreaditemid);
                    if (bookItem != null)
                    {
                        item.currentitemname = bookItem.ItemName;
                    }
                }
                var maxPri = Db.BookItem.Where(m => m.BookId == item.bookid).Max(m => m.Pri);
                bookItem = Db.BookItem.FirstOrDefault(m => m.BookId == item.bookid && m.Pri == maxPri);
                if (bookItem != null)
                {
                    item.lastitemid = bookItem.ItemId;
                    item.lastitemname = bookItem.ItemName;
                    item.update = bookItem.UpdateTime.AsDateTime();
                }
            }
            return new PaginatedList<MyBookShelfViewModel>(result, total, pageIndex, pageSize);
        }
        public BookShelf GetBookShelf(int userId, int bookId)
        {
            return Db.BookShelf.FirstOrDefault(m => m.UserId == userId && m.BookId == bookId);
        }
        public BookShelf AddBookShelf(int userId, int bookId)
        {
            if (userId <= 0)
            {
                return null;
            }
            var m = GetBookShelf(userId, bookId);
            if (m == null)
            {
                m = new BookShelf
                {
                    BookId = bookId,
                    UserId = userId,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                Db.BookShelf.Add(m);
            }
            return Db.SaveChanges() > 0 ? m : null;
        }

        public bool DeleteUserReadHistory(List<int> ids)
        {
            var t = Db.UserReadBookHistory.Where(m => ids.Any(id => m.Id == id));
            if (t == null)
            {
                return false;
            }
            Db.UserReadBookHistory.RemoveRange(t);
            return Db.SaveChanges() > 0;
        }
        public bool DeleteBookShelf(List<int> ids)
        {
            var t = Db.BookShelf.Where(m => ids.Any(id => m.Id == id));
            if (t == null)
            {
                return false;
            }
            Db.BookShelf.RemoveRange(t);
            return Db.SaveChanges() > 0;
        }
    }
}
