using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Novel.Entity.Models;
using Novel.Entity.ViewModels;
using Novel.Service;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Novel.Controllers
{
    public class IndexController : Controller
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
            using (BookService service = new BookService())
            {
                NovelViewModel bookViewModel = service.GetBook(id);
                ViewData["Title"] = bookViewModel.Book.BookName;
                return View(bookViewModel);
            }

        }

        public IActionResult Content(int itemId)
        {
            using (BookService service = new BookService())
            {
                ContentViewModel contentViewModel = service.GetContentViewModel(itemId);
                ViewData["Title"] = contentViewModel.BookName + "-" + contentViewModel.ItemName;
                return View(contentViewModel);
            }
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
