using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Novel.Entity;
using Novel.Service;
using Novel.Utilities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Novel.Controllers
{
    public class BookController : BaseController
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult BookShelf(int id)
        {
            List<int> shelves = GetCookies<List<int>>("bookshelves", new List<int>());
            if (!shelves.Any(p => p == id))
            {
                shelves.Add(id);
            }
            SetCookies("bookshelves", JsonUtil.SerializeObject(shelves), SAVECOOKIESTIME);
            return Json(new ApiResult { data = shelves, status = 0, msg = "请求成功" });
        }
        [HttpPost]
        public JsonResult Thumbsup(int id)
        {
            using (BookService service = new BookService())
            {
                var ip = GetClientIp();
                service.AddBookThumbsup(id,ip, DateTime.Today, null);
            }
            return Json(new ApiResult { data = "", status = 0, msg = "请求成功" });
        }
    }
}
