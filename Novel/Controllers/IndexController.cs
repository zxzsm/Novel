using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Novel.Entity.Models;
using Novel.Models;

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
            BookViewModel bookViewModel = new BookViewModel();
            using (BookContext bookContext = new BookContext())
            {
                var book = bookContext.Book.FirstOrDefault(m => m.BookId == id);
                var bookItems = bookContext.BookItem.Where(m => m.BookId == id).ToList();
                bookViewModel.Book = book;
                bookViewModel.Items = bookItems;
            }
            return View(bookViewModel);
        }
    }
}
