using Novel.Entity.Models;
using Novel.Service;
using Novel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Novel
{
    public class BookStore
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
                        CacheHelper.CacheInsertAddMinutes("Categories", c, 12 * 60);
                    }
                }
                return c;
            }
        }

        public static string GetPinYin(string category)
        {
            return category.IsEmpty() ? "" : Pinyin.GetPinyin(category).Replace(" ", "");
        }
    }
}
