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
            using (BookService bookService = new BookService())
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
                    PageIndexName = "pageIndex"
                };
                ViewBag.PagerOption = pageOption;
                ViewData["Fantasy"] = t;
            }
            return View(searchViewModel);
        }
        public IActionResult Task()
        {
            return View();
        }

        public IActionResult Task(TaskSearchViewModel simpleViewModel)
        {
            using (BookTaskService taskService = new BookTaskService())
            {
                var t = taskService.GetTasks(simpleViewModel);
                ViewData["Data"] = t;
                var url = Url.Action("Task", "Index");
                url += "?1=1";
                if (!simpleViewModel.k.IsEmpty())
                {
                    url += "&k=" + simpleViewModel.k;
                }
                if (simpleViewModel.synctype > 0)
                {
                    url += "&synctype=" + simpleViewModel.synctype;
                }

                var pageOption = new MoPagerOption
                {
                    CurrentPage = t.PageIndex,
                    PageSize = simpleViewModel.ps,
                    RouteUrl = url,
                    TotalPage = t.TotalPages,
                    PageIndexName = "pi"
                };
                ViewBag.PagerOption = pageOption;
            }

            return View();
        }

        public IActionResult EditTask(BookReptileTask task)
        {
            using (BookTaskService bookTaskService = new BookTaskService())
            {
                task = bookTaskService.GetTask(task.Id);
            }
            return View(task);
        }

        public IActionResult SaveTask(BookReptileTask task)
        {
            using (BookTaskService bookTaskService = new BookTaskService())
            {
                bookTaskService.AddBookReptileTask(task);
            }
            return Content("<script> window.close();</script>", "text/html");
        }

    }
}
