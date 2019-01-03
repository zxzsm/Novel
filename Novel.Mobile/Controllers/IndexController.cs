using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Novel.Entity.Models;
using Novel.Entity.ViewModels;
using Novel.Mobile.Models;
using Novel.Service;

namespace Novel.Mobile.Controllers
{
    public class IndexController : BaseController
    {
        public IActionResult Index()
        {
            IndexViewModel viewModel = new IndexViewModel();
            using (BookService bookService = new BookService())
            {
                viewModel.Fantasy = bookService.GetIndexBooks(1);
            }
            return View(viewModel);
        }

        public IActionResult Search()
        {
            return View();
        }
        public IActionResult Novel(int id)
        {
            ViewData["bookshelves"] = GetCookies("bookshelves", new List<MyBookShelfViewModel>());
            using (BookService service = new BookService())
            {
                NovelViewModel bookViewModel = service.GetBook(id);
                bookViewModel.IsThumbsup = service.GetBookThumbsup(id, GetClientIp(), DateTime.Today) != null;
                ViewData["Title"] = bookViewModel.Book.BookName;
                return View(bookViewModel);
            }
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
