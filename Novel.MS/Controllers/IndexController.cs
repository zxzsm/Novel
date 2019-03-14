using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Novel.Entity.Models;
using Novel.Entity.ViewModels;
using Novel.MS.Models;
using Novel.Service;
using Novel.Utilities;

namespace Novel.MS.Controllers
{
    public class IndexController : Controller
    {
        public IActionResult Book(SearchViewModel searchViewModel)
        {
            
            IndexViewModel viewModel = new IndexViewModel();
            using (BookService  bookService = new BookService())
            {
                var t = bookService.GetBooksByBack(searchViewModel);
                var url = Url.Action("Book", "Index");
                if (!searchViewModel.keyword.IsEmpty())
                {
                    url += "?keyword=" + searchViewModel.keyword;
                }
                var pageOption = new MoPagerOption
                {
                    CurrentPage = t.PageIndex,
                    PageSize = searchViewModel.pageSize,
                    RouteUrl = url,
                    TotalPage = t.TotalPages,
                    PageIndexName= "pageIndex"
                };
                ViewBag.PagerOption = pageOption;
                ViewData["Fantasy"] = t;
            }
            return View(searchViewModel);
        }


    }
}
