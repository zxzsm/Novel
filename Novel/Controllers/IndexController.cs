using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Novel.Entity.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Novel.Controllers
{
    public class IndexController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            using (BookContext bookContext = new BookContext())
            {
                var t = bookContext.Book.ToList();
            }
            return View();
        }
    }
}
