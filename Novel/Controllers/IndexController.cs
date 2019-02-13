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
        // GET: /<controller>/
        public IActionResult Index()
        {
            IndexViewModel viewModel = new IndexViewModel();
            using (BookContext bookContext = new BookContext())
            {
                var t = bookContext.Book.ToList();
                viewModel.Fantasy = t;
            }
            ViewData["keywords"] = "小说,小说网,免费小说网,书客来,玄幻奇幻小说,武侠小说,都市言情小说,仙侠小说,历史军事小说,网游竞技小说";
            ViewData["description"] = "小说阅读,精彩小说尽在书客来.书客来提供玄幻奇幻小说,武侠小说,都市言情小说,仙侠小说,历史军事小说,网游竞技小说,首发小说,最新章节免费";
            return View(viewModel);
        }

        public IActionResult Novel(int id)
        {
            ViewData["bookshelves"] = GetCookies("bookshelves", new List<MyBookShelfViewModel>());
            using (BookService service = new BookService())
            {
                NovelViewModel bookViewModel = service.GetBook(id);
                bookViewModel.IsThumbsup = service.GetBookThumbsup(id, GetClientIp(), DateTime.Today) != null;
                ViewData["Title"] = bookViewModel.Book.BookName;
                ViewData["keywords"] = bookViewModel.Book.BookName + "," + bookViewModel.Book.BookName + "最新章节," + bookViewModel.Book.BookAuthor;
                ViewData["description"] = bookViewModel.Book.BookName + "最新章节由网友提供," + bookViewModel.Book.BookName + "情节跌宕起伏、扣人心弦,是一本情节与文笔俱佳的网络小说,书客来免费提供" + bookViewModel.Book.BookName + "凉爽干净的文字章节在线阅读。";
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
                ViewData["keywords"] = contentViewModel.BookName + "," + contentViewModel.ItemName;
                ViewData["description"] = "书客来提供了小说" + contentViewModel.BookName + "凉爽干净的文字章节:" + contentViewModel.ItemName + "在线阅读。";
                ViewData["ReadSetting"] = GetCookies("rsetting", new BookReadSettingViewModel());
            }

            SetReadBookCookies(contentViewModel);
            return View(contentViewModel);
        }

        private void SetReadBookCookies(ContentViewModel contentViewModel)
        {
            var bookReadViewModels = GetCookies("historyreadbooks", new List<BookReadViewModel>());

            var r = bookReadViewModels.FirstOrDefault(m => m.bookid == contentViewModel.BookId);
            if (r == null)
            {
                if (bookReadViewModels.Count > 20)
                {
                    bookReadViewModels.Remove(bookReadViewModels.Last());
                }
                r = new BookReadViewModel
                {
                    bookid = contentViewModel.BookId,
                };
            }
            else
            {
                bookReadViewModels.Remove(r);
            }
            r.currentreaditemid = contentViewModel.ItemId;
            r.lastreadtime = DateTime.Now;
            bookReadViewModels.Insert(0, r);

            var shelves = GetCookies("bookshelves", new List<MyBookShelfViewModel>());
            var mybook = shelves.FirstOrDefault(p => p.bookid == contentViewModel.BookId);
            if (mybook != null)
            {
                mybook.currentreaditemid = contentViewModel.BookId;
                SetCookies("bookshelves", JsonUtil.SerializeObject(shelves), SAVECOOKIESTIME);
            }

            SetCookies("historyreadbooks", JsonUtil.SerializeObject(bookReadViewModels), SAVECOOKIESTIME);
        }

        public IActionResult BookShelf()
        {
            var t = GetCookies("historyreadbooks", new List<BookReadViewModel>());
            var shelves = GetCookies("bookshelves", new List<MyBookShelfViewModel>());
            using (BookService bookService = new BookService())
            {
                bookService.GetReadBookHistory(t);
                ViewData["BookShelves"] = bookService.GetBookShlef(shelves, 1, 10);
            }
            if (t.Count > 0)
            {
                foreach (var item in t)
                {
                    item.lasitemurl = Url.Action("Content", new { itemId = item.lastitemid });
                    item.currentitemurl = Url.Action("Content", new { itemId = item.currentreaditemid });
                    item.bookurl = Url.Action("Novel", new { id = item.bookid });
                }
            }
            ViewData["HistoryReadBooks"] = t;
            return View();
        }
        [HttpGet]
        public IActionResult Search(SearchViewModel viewModel)
        {
            using (BookService service = new BookService())
            {
                var d = service.GetBooks(viewModel);
                ViewData["BookCategory"] = service.GetCategories();
                ViewData["Title"] = viewModel.keyword + "_搜索_书客来";
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
                    items = d.Select(m => new
                    {
                        m.BookName,
                        m.BookSummary,
                        m.BookAuthor,
                        m.BookImage,
                        Url = Url.Action("Novel", new { id = m.BookId })
                    }).ToList()
                });
            }
        }


    }
}
