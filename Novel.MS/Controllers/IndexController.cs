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
                ViewData["Fantasy"] = t;
            }
            return View(searchViewModel);
        }


    }
}
