using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Novel.Entity.Models;
using Novel.Service;
using Novel.Utilities;

namespace Novel.MS.Controllers
{
    public class BookController : Controller
    {
        public IActionResult EditBook(int bookId)
        {
            NovelViewModel book = null;
            using (BookService bookService = new BookService())
            {
                book = bookService.GetBook(bookId);
                if (book == null)
                {
                    return RedirectToAction("Book", "Index");
                }
                ViewData["BookCategories"] = bookService.GetCategories();
                ViewData["category"] = bookService.GetBookCategory(bookId);
            }
            return View(book);
        }
        [HttpPost]
        public IActionResult SaveBook(Book book, int category)
        {
            using (BookService bookService = new BookService())
            {
                bookService.SaveBook(book, category);
            }
            return  Content("<script> window.close();</script>","text/html");
        }
    }
}