using Novel.Entity.Models;
using System;
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
            }
            return contentViewModel;
        }
    }
}
