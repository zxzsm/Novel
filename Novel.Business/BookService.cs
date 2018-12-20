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

        public BookIndex UpdateBookIndex(Book book, bool isRead = false, bool isRecommend = false, bool isSaveChange = true)
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
                    ReadVolume = 0,
                    Recommend = 0
                };
                Db.BookIndex.Add(bookIndex);
            }
            if (isRead)
            {
                bookIndex.ReadVolume++;
                book.ReadVolume++;
            }
            if (isRecommend)
            {
                bookIndex.Recommend++;
                book.Recommend++;
            }
            if (isSaveChange)
            {
                Db.SaveChanges();
            }
            return bookIndex;
        }
        public BookIndex UpdateBookIndex(int bookid, bool isRead = false, bool isRecommend = false, bool isSaveChange = true)
        {
            var book = Db.Book.FirstOrDefault(m => m.BookId == bookid);
            return UpdateBookIndex(book, isRead, isRecommend, isSaveChange);
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
    }
}
