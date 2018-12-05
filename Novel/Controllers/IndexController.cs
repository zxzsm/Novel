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
            NovelViewModel bookViewModel = new NovelViewModel();
            using (BookContext bookContext = new BookContext())
            {
                var book = bookContext.Book.FirstOrDefault(m => m.BookId == id);
                var bookItems = bookContext.BookItem.Where(m => m.BookId == id).ToList();
                bookViewModel.Book = book;
                bookViewModel.Items = bookItems;
            }
            return View(bookViewModel);
        }

        public IActionResult Content(int itemId)
        {
            ContentViewModel contentViewModel = BookService.GetContentViewModel(itemId);
            return View(contentViewModel);
        }
        [HttpPost]
        public IActionResult Search(SearchViewModel viewModel)
        {
            var d = BookService.GetBooks(viewModel);
            ViewData["BookCategory"] = BookService.GetCategories();
            return View(viewModel);
        }


    }
}
