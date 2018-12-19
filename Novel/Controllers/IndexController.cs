using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Novel.Entity;
using Novel.Entity.Models;
using Novel.Entity.ViewModels;
using Novel.Service;
using Novel.Utilities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Novel.Controllers
{
    public class IndexController : BaseController
    {
        const int SAVECOOKIESTIME = 1051200;
        // GET: /<controller>/
        public IActionResult Index()
        {
            IndexViewModel viewModel = new IndexViewModel();
            using (BookContext bookContext = new BookContext())
            {
                var t = bookContext.Book.ToList();
                viewModel.Fantasy = t;
            }
            return View(viewModel);
        }

        public IActionResult Novel(int id)
        {
            ViewData["bookshelves"] = GetCookies<List<int>>("bookshelves", new List<int>());
            using (BookService service = new BookService())
            {
                NovelViewModel bookViewModel = service.GetBook(id);
                ViewData["Title"] = bookViewModel.Book.BookName;
                return View(bookViewModel);
            }

        }

        public IActionResult Content(int itemId)
        {
            ContentViewModel contentViewModel = null;
            using (BookService service = new BookService())
            {
                contentViewModel = service.GetContentViewModel(itemId);
                ViewData["Title"] = contentViewModel.BookName + "-" + contentViewModel.ItemName;
            }
           
            SetReadBookCookies(contentViewModel);
            return View(contentViewModel);
        }

        private void SetReadBookCookies(ContentViewModel contentViewModel)
        {
            List<BookReadViewModel> bookReadViewModels = GetCookies("historyreadbooks", new List<BookReadViewModel>());
            var r = bookReadViewModels.FirstOrDefault(m => m.bookid == contentViewModel.BookId);
            if (r == null)
            {
                r = new BookReadViewModel
                {
                    bookid = contentViewModel.BookId,
                    bookname = contentViewModel.BookName,
                    bookurl = Url.Action("Novel", new { id = contentViewModel.BookId }),
                };
                bookReadViewModels.Add(r);
            }
            r.currentitemname = contentViewModel.ItemName;
            r.currentreaditemid = contentViewModel.ItemId;
            r.currentitemurl = Url.Action("Content", new { itemId = contentViewModel.ItemId });
            r.update = DateTime.Now;
            SetCookies("historyreadbooks", JsonUtil.SerializeObject(bookReadViewModels), SAVECOOKIESTIME);
        }

        public IActionResult My()
        {
            return View();
        }
        [HttpPost]
        public JsonResult AddBookShelf(int id)
        {
            List<int> shelves = GetCookies<List<int>>("bookshelves", new List<int>());
            if (!shelves.Any(p => p == id))
            {
                shelves.Add(id);
            }
            SetCookies("bookshelves", JsonUtil.SerializeObject(shelves), SAVECOOKIESTIME);
            shelves = GetCookies<List<int>>("bookshelves", new List<int>());
            return Json(new ApiResult { data = shelves, status = 0, msg = "请求成功" });
        }
        public IActionResult Search(SearchViewModel viewModel)
        {
            using (BookService service = new BookService())
            {
                var d = service.GetBooks(viewModel);
                ViewData["BookCategory"] = service.GetCategories();
                using (BookContext bookContext = new BookContext())
                {
                    var t = bookContext.Book.ToList();
                    ViewData["Books"] = t;
                }
                return View(viewModel);
            }
        }
        [HttpPost]
        public JsonResult SearchKeyword(SearchViewModel viewModel)
        {
            using (BookService service = new BookService())
            {
                var d = service.GetBooks(viewModel);
                return Json(new
                {
                    currentPageIndex = d.PageIndex,
                    viewModel.pageSize,
                    pageCount = d.TotalPages,
                    items = d.ToList()
                });
            }
        }


    }
}
