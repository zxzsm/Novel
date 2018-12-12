using Novel.Entity;
using Novel.Entity.Models;
using Novel.Entity.ViewModels;
using Novel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Novel.Service
{
    public class BookService
    {
        public static ContentViewModel GetContentViewModel(int itemId)
        {
            ContentViewModel contentViewModel = null;
            using (var db = new BookContext())
            {
                var content = db.BookContent.FirstOrDefault(m => m.ItemId == itemId);
                var bookItem = db.BookItem.FirstOrDefault(m => m.ItemId == itemId);
                if (content == null || bookItem == null)
                {
                    return contentViewModel;
                }
                var book = db.Book.FirstOrDefault(m => m.BookId == bookItem.BookId);
                if (book == null)
                {
                    return contentViewModel;
                }

                contentViewModel = new ContentViewModel
                {
                    BookId = book.BookId,
                    BookName = book.BookName,
                    Content = content.Content,
                    ItemId = bookItem.ItemId,
                    ItemName = bookItem.ItemName
                };
                //上一章
                var preItem = db.BookItem.Where(m => m.BookId == book.BookId && m.ItemId < itemId).OrderByDescending(m => m.ItemId).FirstOrDefault();
                if (preItem != null)
                {
                    contentViewModel.PreId = preItem.ItemId;
                    contentViewModel.PreItemName = preItem.ItemName;
                }
                //下一章节
                var nextItem = db.BookItem.Where(m => m.BookId == book.BookId && m.ItemId > itemId).OrderBy(m => m.ItemId).FirstOrDefault();
                if (nextItem != null)
                {
                    contentViewModel.NextId = nextItem.ItemId;
                    contentViewModel.NextName = nextItem.ItemName;
                }
            }
            return contentViewModel;
        }

        public static NovelViewModel GetBook(int id)
        {
            NovelViewModel bookViewModel = new NovelViewModel();
            using (var db = new BookContext())
            {
                var book = db.Book.FirstOrDefault(m => m.BookId == id);
                var bookItems = db.BookItem.Where(m => m.BookId == id).ToList();
                bookViewModel.Book = book;
                bookViewModel.Items = bookItems;
                return bookViewModel;
            }
        }
        public static PaginatedList<Book> GetBooks(SearchViewModel viewModel)
        {
            if (viewModel == null)
            {

                viewModel = new SearchViewModel
                {
                    pageSize = 10,
                    pageIndex = 1,
                };
            }
            if (viewModel.pageIndex <= 0)
            {
                viewModel.pageIndex = 1;
            }
            using (var db = new BookContext())
            {
                var q = db.Book.Where(m => 1 == 1);
                if (!viewModel.keyword.IsEmpty())
                {
                    q = q.Where(m => m.BookName.Contains(viewModel.keyword));
                }
                var list = q.ToList();
                return new PaginatedList<Book>(list, list.Count, viewModel.pageIndex, viewModel.pageSize);
            }

        }

        public static List<BookCategory> GetCategories()
        {
            using (var db = new BookContext())
            {
                return db.BookCategory.ToList();
            }
        }
    }
}
