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
        public List<Book> GetIndexBooks(int category)
        {
            return Db.Book.Take(8).ToList();
        }
        public ContentViewModel GetContentViewModel(int itemId, int userId = 0)
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

            var relation = Db.BookGroupCategroyRelation.FirstOrDefault(m => m.BookId == book.BookId);
            if (relation != null)
            {
                contentViewModel.Category = GetCategories().FirstOrDefault(m => m.CategoryId == relation.CategoryId);
            }

            if (userId > 0)
            {
                var readBookHistories = Db.UserReadBookHistory.Where(m => m.UserId == userId).OrderBy(m => m.UpdateTime);
                var h = readBookHistories.FirstOrDefault(m => m.BookId == book.BookId);
                if (h == null)
                {
                    if (readBookHistories.Count() > 20)
                    {
                        Db.UserReadBookHistory.Remove(readBookHistories.First());
                    }
                    h = new UserReadBookHistory { BookId = book.BookId, ReadItemId = itemId, CreateTime = now, UpdateTime = now, UserId = userId };
                    Db.UserReadBookHistory.Add(h);
                }
                else
                {
                    h.ReadItemId = itemId;
                    h.UpdateTime = now;
                }

                var shelf = Db.BookShelf.FirstOrDefault(m => m.UserId == userId && m.BookId == book.BookId);
                if (shelf != null)
                {
                    shelf.ReadItemId = itemId;
                    shelf.UpdateTime = now;
                }
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
        public NovelViewModel GetBookDetail(int id)
        {
            NovelViewModel bookViewModel = new NovelViewModel();
            var book = Db.Book.FirstOrDefault(m => m.BookId == id);
            var bookItems = Db.BookItem.Where(m => m.BookId == id).Select(m => new BookItemViewModel { ItemName = m.ItemName, ItemId = m.ItemId }).ToList();

            var relation = Db.BookGroupCategroyRelation.FirstOrDefault(m => m.BookId == id);
            if (relation != null)
            {
                bookViewModel.BookCategory = GetCategories().FirstOrDefault(m => m.CategoryId == relation.CategoryId);
            }
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
            if (viewModel.pageIndex == 0)
            {
                viewModel.pageIndex = 1;
            }
            if (viewModel.pageSize == 0)
            {
                viewModel.pageSize = 10;
            }
            List<Func<Book, bool>> funcs = new List<Func<Book, bool>>();
            Func<Book, bool> func = null;
            if (!viewModel.keyword.IsEmpty())
            {
                func = m => m.BookName.Contains(viewModel.keyword);
                funcs.Add(func);
            }
            int total = 0;
            int totalPage = 0;
            var t = LoadPagerEntities(viewModel.pageSize, viewModel.pageIndex, m => m.UpdateTime, out total, out totalPage, isAsc: false, whereLambda: funcs.ToArray());
            return new PaginatedList<Book>(t.ToList(), total, viewModel.pageIndex, viewModel.pageSize);
        }
        public List<Book> GetBooksByBack(SearchViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new SearchViewModel
                {
                    pageSize = 10,
                    pageIndex = 1,
                };
            }
            if (viewModel.pageIndex == 0)
            {
                viewModel.pageIndex = 1;
            }
            if (viewModel.pageSize == 0)
            {
                viewModel.pageSize = 10;
            }
            var q = Db.Book.AsQueryable();

            if (!viewModel.keyword.IsEmpty())
            {
                q = q.Where(m => m.BookName.Contains(viewModel.keyword));
            }
            if (viewModel.nocategory.HasValue)
            {
                var relations = Db.BookGroupCategroyRelation.AsQueryable();
                if (viewModel.nocategory.Value)
                {
                    q = q.Where(m => !relations.Any(p => p.BookId == m.BookId));
                }
                else
                {
                    q = q.Where(m => relations.Any(p => p.BookId == m.BookId));
                }
            }
            return q.Take(10).ToList();
        }
        public List<BookCategory> GetCategories()
        {
            var c = CacheHelper.CacheValue("Categories") as List<BookCategory>;
            if (c == null)
            {
                c = Db.BookCategory.ToList();
                CacheHelper.CacheInsertAddMinutes("Categories", c, 12 * 60);
            }
            return c;
        }
        public BookCategory GetBookCategory(int bookId)
        {
            var relation = Db.BookGroupCategroyRelation.FirstOrDefault(m => m.BookId == bookId);
            if (relation == null)
            {
                return null;
            }
            return Db.BookCategory.FirstOrDefault(m => m.CategoryId == relation.CategoryId);
        }
        public bool SaveBook(Book book, int categoryId)
        {
            if (book == null)
            {
                return false;
            }
            var t = Db.Book.FirstOrDefault(m => m.BookId == book.BookId);
            if (t == null)
            {
                return false;
            }
            t.BookName = book.BookName.AsTrim();
            t.BookAuthor = book.BookAuthor.AsTrim();
            t.BookSummary = book.BookSummary;
            var relation = Db.BookGroupCategroyRelation.FirstOrDefault(m => m.BookId == book.BookId);
            if (relation == null)
            {
                relation = new BookGroupCategroyRelation
                {
                    BookId = book.BookId,
                    CategoryId = categoryId,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                Db.BookGroupCategroyRelation.Add(relation);
            }
            else
            {
                if (relation.CategoryId != categoryId)
                {
                    relation.CategoryId = categoryId;
                    relation.UpdateTime = DateTime.Now;
                }
            }
            return Db.SaveChanges() > 0;
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
        public List<BookReadViewModel> GetReadBookHistory(int userId)
        {
            var hs = Db.UserReadBookHistory.Where(m => m.UserId == userId).OrderByDescending(m => m.UpdateTime);
            List<BookReadViewModel> readViewModels = new List<BookReadViewModel>();
            // var q = Db.BookItem.Where(m => readViewModels.Any(p => p.bookid == m.BookId));
            foreach (var h in hs)
            {
                BookReadViewModel item = null;
                var book = Db.Book.FirstOrDefault(m => m.BookId == h.BookId);
                if (book == null)
                {
                    continue;
                }
                item = new BookReadViewModel
                {
                    id = h.Id,
                    bookid = book.BookId,
                    bookauthor = book.BookAuthor,
                    bookname = book.BookName,
                    bookimage = book.BookImage,
                    currentreaditemid = h.ReadItemId,
                    lastreadtime = h.UpdateTime.AsDateTime(),
                };
                readViewModels.Add(item);
                if (h.ReadItemId > 0)
                {
                    var currentItem = Db.BookItem.FirstOrDefault(m => m.ItemId == h.ReadItemId && m.BookId == h.BookId);
                    if (currentItem != null)
                    {
                        item.currentitemname = currentItem.ItemName;
                    }
                }
                var maxPri = Db.BookItem.Where(m => m.BookId == h.BookId).Max(m => m.Pri);
                var lastItem = Db.BookItem.FirstOrDefault(m => m.BookId == h.BookId && m.Pri == maxPri);
                if (lastItem != null)
                {
                    item.lastitemname = lastItem.ItemName;
                    item.lastitemid = lastItem.ItemId;
                    item.update = lastItem.UpdateTime.AsDateTime();
                }
            }
            return readViewModels;
        }

        public PaginatedList<MyBookShelfViewModel> GetBookShlef(List<MyBookShelfViewModel> shelevs, int pageIndex, int pageSize)
        {
            var q = Db.Book.Where(m => shelevs.Any(p => p.bookid == m.BookId));
            int total = 0;
            int totalPage = 0;
            Func<Book, bool> whereLambda = m => shelevs.Any(p => p.bookid == m.BookId);
            var t = LoadPagerEntities(pageSize, pageIndex, m => m.UpdateTime, out total, out totalPage, whereLambda: whereLambda);
            var result = t.Select(m => new MyBookShelfViewModel
            {
                bookid = m.BookId,
                bookname = m.BookName,
                bookauthor = m.BookAuthor,
                bookimage = m.BookImage,
                update = m.UpdateTime.AsDateTime()
            }).ToList();
            foreach (var item in result)
            {
                var shelfViewModel = shelevs.FirstOrDefault(m => m.bookid == item.bookid);
                if (shelfViewModel != null)
                {
                    item.currentreaditemid = shelfViewModel.currentreaditemid;
                    item.lastitemid = shelfViewModel.lastitemid;
                    item.lastreadtime = shelfViewModel.lastreadtime;
                }
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


        public PaginatedList<Book> GetBooksByCategory(int categoryId, int pageIndex, int pageSize)
        {
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }
            if (pageSize <= 0)
            {
                pageSize = 20;
            }
            var q = Db.Book.Join(Db.BookGroupCategroyRelation.Where(m => m.CategoryId == categoryId), m => m.BookId, m => m.BookId, (b, r) => b);
            int total = 0;
            int totalPage = 0;
            var t = LoadPagerEntities(q, pageSize, pageIndex, m => m.ReadVolume, out total, out totalPage, false);
            return new PaginatedList<Book>(t.ToList(), total, pageIndex, pageSize);
        }


        public List<Book> GetHotBooksByCategory(int categoryId, int pageSize)
        {
            var q = Db.Book.Join(Db.BookGroupCategroyRelation.Where(m => m.CategoryId == categoryId), m => m.BookId, m => m.BookId, (b, r) => b);
            return q.OrderByDescending(m => m.ReadVolume).ThenByDescending(m => m.UpdateTime).Take(pageSize).ToList();
        }

    }
}
