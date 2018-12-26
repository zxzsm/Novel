using Novel.Entity;
using Novel.Entity.Models;
using Novel.Entity.ViewModels;
using Novel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Novel.Service
{
    public class BookService : BaseRepository<Book>
    {
        public ContentViewModel GetContentViewModel(int itemId)
        {
            ContentViewModel contentViewModel = null;


            var bookItem = Db.BookItem.FirstOrDefault(m => m.ItemId == itemId);
            var book = Db.Book.FirstOrDefault(m => m.BookId == bookItem.BookId);
            if (book == null)
            {
                return contentViewModel;
            }
            contentViewModel = new ContentViewModel
            {
                BookId = book.BookId,
                BookName = book.BookName,
                Content = bookItem.Content,
                ItemId = bookItem.ItemId,
                ItemName = bookItem.ItemName
            };
            var now = DateTime.Now;
            UpdateBookIndex(book, isRead: true, isSaveChange: false);

            //上一章
            var preItem = Db.BookItem.Where(m => m.BookId == book.BookId && m.ItemId < itemId).OrderByDescending(m => m.ItemId).FirstOrDefault();
            if (preItem != null)
            {
                contentViewModel.PreId = preItem.ItemId;
                contentViewModel.PreItemName = preItem.ItemName;
            }
            //下一章节
            var nextItem = Db.BookItem.Where(m => m.BookId == book.BookId && m.ItemId > itemId).OrderBy(m => m.ItemId).FirstOrDefault();
            if (nextItem != null)
            {
                contentViewModel.NextId = nextItem.ItemId;
                contentViewModel.NextName = nextItem.ItemName;
            }
            Db.SaveChanges();
            return contentViewModel;
        }

        public BookIndex UpdateBookIndex(Book book, bool isRead = false, bool isSaveChange = true)
        {
            if (book == null)
            {
                return null;
            }
            var now = DateTime.Now;
            BookIndex bookIndex = Db.BookIndex.FirstOrDefault(m => m.BookId == book.BookId && m.Date == now.Date);
            if (bookIndex == null)
            {
                bookIndex = new BookIndex
                {
                    BookId = book.BookId,
                    BookName = book.BookName,
                    Date = now.Date,
                    DataYm = int.Parse(now.ToString("yyyyMM")),
                    ReadVolume = 0
                };
                Db.BookIndex.Add(bookIndex);
            }
            if (isRead)
            {
                bookIndex.ReadVolume++;
                book.ReadVolume++;
            }
            if (isSaveChange)
            {
                Db.SaveChanges();
            }
            return bookIndex;
        }
        public BookIndex UpdateBookIndex(int bookid, bool isRead = false, bool isSaveChange = true)
        {
            var book = Db.Book.FirstOrDefault(m => m.BookId == bookid);
            return UpdateBookIndex(book, isRead, isSaveChange);
        }

        public NovelViewModel GetBook(int id)
        {
            NovelViewModel bookViewModel = new NovelViewModel();

            var book = Db.Book.FirstOrDefault(m => m.BookId == id);
            var bookItems = Db.BookItem.Where(m => m.BookId == id).Select(m => new BookItemViewModel { ItemName = m.ItemName, ItemId = m.ItemId }).ToList();
            bookViewModel.Book = book;
            bookViewModel.Items = bookItems;
            return bookViewModel;

        }
        public PaginatedList<Book> GetBooks(SearchViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new SearchViewModel
                {
                    pageSize = 10,
                    pageIndex = 1,
                };
            }

            Func<Book, bool> func = null;
            if (!viewModel.keyword.IsEmpty())
            {
                func = m => m.BookName.Contains(viewModel.keyword);
            }

            int total = 0;
            int totalPage = 0;
            var t = LoadPagerEntities(viewModel.pageSize, viewModel.pageIndex, m => m.UpdateTime, out total, out totalPage, whereLambda: func, isAsc: false);
            return new PaginatedList<Book>(t.ToList(), total, viewModel.pageIndex, viewModel.pageSize);
        }

        public List<BookCategory> GetCategories()
        {
            return Db.BookCategory.ToList();
        }

        public BookThumbsup AddBookThumbsup(int bookid, string ip, DateTime date, int? userid = null)
        {
            BookThumbsup bookThumbsup = GetBookThumbsup(bookid, ip, date, userid);
            if (bookThumbsup == null)
            {
                bookThumbsup = new BookThumbsup
                {
                    Ip = ip,
                    Date = date,
                    UserId = userid,
                    BookId = bookid
                };
                Db.BookThumbsup.Add(bookThumbsup);
            }
            else
            {
                bookThumbsup.Ip = ip;
            }
            Db.SaveChanges();
            return bookThumbsup;
        }

        public BookThumbsup GetBookThumbsup(int bookid, string ip, DateTime date, int? userid = null)
        {
            var q = Db.BookThumbsup.Where(m => m.Date == date && m.BookId == bookid);
            BookThumbsup bookThumbsup = null;
            if (userid.HasValue)
            {
                bookThumbsup = q.FirstOrDefault(m => m.UserId == userid);
            }
            else
            {
                bookThumbsup = q.FirstOrDefault(m => m.Ip == ip);
            }
            return bookThumbsup;
        }

        public void GetReadBookHistory(List<BookReadViewModel> readViewModels)
        {
            if (readViewModels == null && readViewModels.Count == 0)
            {
                return;
            }
            var q = Db.BookItem.Where(m => readViewModels.Any(p => p.bookid == m.BookId));
            foreach (var item in readViewModels)
            {
                var book = Db.Book.FirstOrDefault(m => m.BookId == item.bookid);
                if (book != null)
                {
                    item.bookauthor = book.BookAuthor;
                    item.bookname = book.BookName;
                }
                var currentItem = q.Where(m => m.ItemId == item.currentreaditemid).FirstOrDefault();
                if (currentItem != null)
                {
                    item.currentitemname = currentItem.ItemName;
                    item.currentreaditemid = currentItem.ItemId;
                }
                var lastItem = q.Where(m => m.BookId == item.bookid).OrderByDescending(m => m.Pri).Select(m => new { m.ItemName, m.ItemId, m.UpdateTime }).FirstOrDefault();
                if (lastItem != null)
                {
                    item.lastitemname = lastItem.ItemName;
                    item.lastitemid = lastItem.ItemId;
                    item.update = lastItem.UpdateTime.AsDateTime();
                }
            }
        }

        public PaginatedList<MyBookShelfViewModel> GetBookShlef(List<MyBookShelfViewModel> shelevs, int pageIndex, int pageSize)
        {
            var q = Db.Book.Where(m => shelevs.Any(p => p.bookid == m.BookId));
            int total = 0;
            int totalPage = 0;
            Func<Book, bool> whereLambda = m => shelevs.Any(p => p.bookid == m.BookId);
            var t = LoadPagerEntities(10, 1, m => m.UpdateTime, out total, out totalPage, whereLambda: whereLambda);
            var result = t.Select(m => new MyBookShelfViewModel
            {
                bookid = m.BookId,
                bookname = m.BookName,
                bookauthor = m.BookAuthor,
                bookimage=m.BookImage,
                update = m.UpdateTime.AsDateTime()
            }).ToList();
            foreach (var item in result)
            {
                if (item.currentreaditemid<=0)
                {
                    continue;
                }
                var bookItem = Db.BookItem.FirstOrDefault(m => m.ItemId == item.currentreaditemid);
                if (bookItem != null)
                {
                    item.currentitemname = bookItem.ItemName;
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
    }
}
