using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Novel.Entity;
using Novel.Entity.ViewModels;
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

            if (!User.Identity.IsAuthenticated)
            {
                return Json(ApiResult<string>.Fail("请先登陆"));
            }
            int userId = 0;
            if (HttpContext.User.Claims.Any(m => m.Type == ClaimTypes.PrimarySid))
            {
                userId = HttpContext.User.Claims.First(m => m.Type == ClaimTypes.PrimarySid).Value.AsInt();
            }
            using (UserService userService = new UserService())
            {
                userService.AddBookShelf(userId, id);
            }
            return Json(new ApiResult<List<MyBookShelfViewModel>> { data = null, status = 0, msg = "请求成功" });
        }
        [HttpPost]
        public JsonResult Thumbsup(int id)
        {
            using (BookService service = new BookService())
            {
                var ip = GetClientIp();
                service.AddBookThumbsup(id, ip, DateTime.Today, null);
            }
            return Json(new ApiResult<string> { data = "", status = 0, msg = "请求成功" });
        }
    }
}
