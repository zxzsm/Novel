﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Novel.Entity;
using Novel.Entity.Models;
using Novel.Entity.ViewModels;
using Novel.Service;
using Novel.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Novel.Mobile.Controllers
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
                return View(bookViewModel);
            }

        }

        public IActionResult Content(int itemId)
        {
            ContentViewModel contentViewModel = null;
            using (BookService service = new BookService())
            {
                contentViewModel = service.GetContentViewModel(itemId);
                ViewData["Title"] = contentViewModel.ItemName + "_" + contentViewModel.BookName;
                ViewData["ReadSetting"] = GetCookies("rsetting", new BookReadSettingViewModel());
            }

            SetReadBookCookies(contentViewModel);
            return View(contentViewModel);
        }

        private void SetReadBookCookies(ContentViewModel contentViewModel)
        {
            var historyBooks = GetCookies("historyreadbooks", new List<BookReadViewModel>());
            var myShelves = GetCookies("bookshelves", new List<MyBookShelfViewModel>());
            var r = historyBooks.FirstOrDefault(m => m.bookid == contentViewModel.BookId);
            if (r == null)
            {
                if (historyBooks.Count > 20)
                {
                    historyBooks.Remove(historyBooks.Last());
                }
                r = new BookReadViewModel
                {
                    bookid = contentViewModel.BookId,
                };
            }
            else
            {
                historyBooks.Remove(r);
            }
            var mybook = myShelves.FirstOrDefault(p => p.bookid == contentViewModel.BookId);
            if (mybook != null)
            {
                mybook.currentreaditemid = contentViewModel.ItemId;
                mybook.lastreadtime = DateTime.Now;
                SetCookies("bookshelves", JsonUtil.SerializeObject(myShelves), SAVECOOKIESTIME);
            }
            
            r.currentreaditemid = contentViewModel.ItemId;
            r.lastreadtime = DateTime.Now;
            historyBooks.Insert(0, r);
            
            SetCookies("historyreadbooks", JsonUtil.SerializeObject(historyBooks), SAVECOOKIESTIME);
        }

        public IActionResult BookShelf()
        {
            var t = GetCookies("historyreadbooks", new List<BookReadViewModel>());
            var shelves = GetCookies("bookshelves", new List<MyBookShelfViewModel>());
            PaginatedList<MyBookShelfViewModel> shelfViewModels = null;
            using (BookService bookService = new BookService())
            {
                bookService.GetReadBookHistory(t);
                shelfViewModels = bookService.GetBookShlef(shelves, 1, 20);

            }
            foreach (var item in shelfViewModels)
            {
                item.lasitemurl = Url.Action("Content", new { itemId = item.lastitemid });
                item.currentitemurl = Url.Action("Content", new { itemId = item.currentreaditemid });
                item.bookurl = Url.Action("Novel", new { id = item.bookid });
            }
            ViewData["BookShelves"] = shelfViewModels;
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

        public IActionResult Search(SearchViewModel viewModel)
        {
            using (BookService service = new BookService())
            {
                var d = service.GetBooks(viewModel);
                var initDatas = new
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
                };
                ViewData["keyword"] = viewModel.keyword;
                ViewData["InitDatas"] = JsonUtil.SerializeObject(initDatas);
                return View(viewModel);
            }
        }
        [HttpPost]
        public JsonResult SearchKeyword(SearchViewModel viewModel)
        {
            using (BookService service = new BookService())
            {
                var d = service.GetBooks(viewModel);
                var data = new
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
                };
                return base.Json(data);
            }
        }


    }
}
