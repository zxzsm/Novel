using Novel.Entity.Models;
using Novel.Service;
using Novel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Novel.Mobile
{
    public class BookCommon
    {
        public static List<BookCategory> Categories
        {
            get
            {
                var c = CacheHelper.CacheValue("Categories") as List<BookCategory>;
                if (c == null)
                {
                    using (BookService bookService = new BookService())
                    {
                        c = bookService.GetCategories();
                    }
                }
                return c;
            }
        }

        public static Dictionary<BookCategory, List<Book>> IndexCategoryBooks
        {
            get
            {
                var c = CacheHelper.CacheValue("IndexBooks") as Dictionary<BookCategory, List<Book>>;
                if (c == null)
                {
                    c = new Dictionary<BookCategory, List<Book>>();
                    var t = Categories;
                    foreach (var item in t)
                    {
                        using (BookService bookService = new BookService())
                        {
                            var books = bookService.GetHotBooksByCategory(item.CategoryId, 8);
                            c[item] = books;
                        }
                    }
                }
                return c;
            }
        }

        

        public const int PAGESIZE = 20;
    }
}
