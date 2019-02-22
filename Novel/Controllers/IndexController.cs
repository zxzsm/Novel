using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Novel.Entity;
using Novel.Entity.Models;
using Novel.Entity.ViewModels;
using Novel.Service;
using Novel.Utilities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Novel.Controllers
{
    public class IndexController : BaseController
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewData["cdatas"] = BookCommon.IndexCategoryBooks;
            ViewData["keywords"] = "书客来,小说,小说网,免费小说网,玄幻奇幻小说,武侠小说,都市言情小说,仙侠小说,历史军事小说,网游竞技小说";
            ViewData["description"] = "书客来,小说阅读,精彩小说尽在书客来,书客来提供玄幻奇幻小说,武侠小说,都市言情小说,仙侠小说,历史军事小说,网游竞技小说,首发小说,最新章节免费";
            ViewData["mobileurl"] = BookCommon.MOBILEURL;
            return View();
        }
        public IActionResult Novel(int id)
        {
            ViewData["isshelf"] = false;
            NovelViewModel bookViewModel = null;
            using (BookService service = new BookService())
            {
                bookViewModel = service.GetBookDetail(id);
                bookViewModel.IsThumbsup = service.GetBookThumbsup(id, GetClientIp(), DateTime.Today) != null;

                ViewData["Title"] = bookViewModel.Book.BookName+"-"+bookViewModel.Book.BookAuthor+"著-书客来-免费小说网";
                ViewData["keywords"] = bookViewModel.Book.BookName + "," + bookViewModel.Book.BookName + "最新章节," + bookViewModel.Book.BookAuthor;
                ViewData["description"] = bookViewModel.Book.BookName + "最新章节由网友提供," + bookViewModel.Book.BookName + "情节跌宕起伏、扣人心弦,是一本情节与文笔俱佳的网络小说,书客来免费提供" + bookViewModel.Book.BookName + "凉爽干净的文字章节在线阅读。";
                ViewData["mobileurl"] = BookCommon.MOBILEURL.TrimEnd('/')  + Url.Action("Novel", "Index", new { id });
            }
            if (UserId > 0)
            {
                using (BookShelfService shelfService = new BookShelfService())
                {
                    var t = shelfService.GetBookShelf(UserId, bookViewModel.Book.BookId);
                    ViewData["isshelf"] = t != null;
                }
            }
            return View(bookViewModel);

        }
        public IActionResult Content(int itemId)
        {
            ContentViewModel contentViewModel = null;
            using (BookService service = new BookService())
            {
                contentViewModel = service.GetContentViewModel(itemId, UserId);
                ViewData["Title"] =  contentViewModel.ItemName+"-"+ contentViewModel.BookName+ "-书客来";
                ViewData["keywords"] = contentViewModel.BookName + "," + contentViewModel.ItemName;
                ViewData["description"] = "书客来提供了小说" + contentViewModel.BookName + "最新的文字章节:" + contentViewModel.ItemName + "在线阅读。";
                ViewData["ReadSetting"] = GetCookies("rsetting", new BookReadSettingViewModel());
                ViewData["mobileurl"] = BookCommon.MOBILEURL.TrimEnd('/')+ Url.Action("Content", "Index", new { itemId });
            }
            return View(contentViewModel);
        }

        [Authorize]
        public IActionResult BookShelf()
        {
            List<BookReadViewModel> t = null;
            PaginatedList<MyBookShelfViewModel> shelves = null;
            using (BookService bookService = new BookService())
            {
                ViewData["mobileurl"] = BookCommon.MOBILEURL.TrimEnd('/') + Url.Action("BookShelf", "Index");
                t = bookService.GetReadBookHistory(UserId);

            }
            using (BookShelfService shelfService = new BookShelfService())
            {
                shelves = shelfService.GetBookShlef(UserId, 1, int.MaxValue);
            }

            if (t != null && t.Count > 0)
            {
                foreach (var item in t)
                {
                    item.lasitemurl = Url.Action("Content", new { itemId = item.lastitemid });
                    item.currentitemurl = Url.Action("Content", new { itemId = item.currentreaditemid });
                    item.bookurl = Url.Action("Novel", new { id = item.bookid });
                }
            }
            if (shelves != null && shelves.Count > 0)
            {
                foreach (var item in shelves)
                {
                    item.lasitemurl = Url.Action("Content", new { itemId = item.lastitemid });
                    item.currentitemurl = Url.Action("Content", new { itemId = item.currentreaditemid });
                    item.bookurl = Url.Action("Novel", new { id = item.bookid });
                }
            }
            ViewData["BookShelves"] = shelves;
            ViewData["HistoryReadBooks"] = t;
            return View();
        }
        [HttpGet]
        public IActionResult Search(SearchViewModel viewModel)
        {
            using (BookService service = new BookService())
            {
                var d = service.GetBooks(viewModel);
                ViewData["BookCategory"] = service.GetCategories();
                ViewData["Title"] = viewModel.keyword + "_搜索_书客来";
                return View(viewModel);
            }
        }
        [HttpPost]
        public JsonResult SearchKeyword(SearchViewModel viewModel)
        {
            using (BookService service = new BookService())
            {
                viewModel.keyword = viewModel.keyword.AsTrim();
                var d = service.GetBooks(viewModel);
                return Json(new
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
                });
            }
        }
        public IActionResult Login(string ReturnUrl = "")
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            ViewData["mobileurl"] = BookCommon.MOBILEURL.TrimEnd('/')  + Url.Action("Login", "Index");
            ViewData["returnurl"] = ReturnUrl;
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> Sign(UserInfo user, string returnurl = "")
        {
            user.UserName = user.UserName.AsTrim();
            user.Uesrpwd = user.Uesrpwd.AsTrim();
            using (UserService userService = new UserService())
            {
                var t = userService.GetUserInfo(user.UserName);
                if (t == null)
                {
                    return Json(ApiResult<object>.Fail("用户未找到"));
                }
                if (t != null && t.Uesrpwd != user.Uesrpwd)
                {
                    return Json(ApiResult<object>.Fail("密码错误"));
                }
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                //可以放用户唯一标识。 然后再BaseController中使用User.Identity.Name获取， 再查询数据库/缓存获取用户信息
                identity.AddClaim(new Claim(ClaimTypes.Name, t.UserName)); //取值 User.Identity.Name
                identity.AddClaim(new Claim(ClaimTypes.PrimarySid, t.UserId.ToString()));
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity), new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddMonths(1),
                        AllowRefresh = true
                    });
                return Json(ApiResult<UserInfo>.OK(user));
            }
        }
        public IActionResult Register()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        public JsonResult RegisterUser(UserInfo info)
        {
            UserInfo t = null;
            using (UserService userService = new UserService())
            {
                if (!userService.CheckUserInfo(info, true, true))
                {
                    return Json(ApiResult<object>.Fail("请检查信息是否填写完整"));
                }
                t = userService.GetUserInfo(info.UserName);
                if (t != null)
                {
                    return Json(ApiResult<object>.Fail("该用户名已注册"));
                }
                t = userService.GetUserInfo(info.UserMoblie.AsTrim(), info.UserEmail.AsTrim());
                if (t != null)
                {
                    return Json(ApiResult<object>.Fail("该手机号码和邮箱已注册"));
                }
                t = userService.AddUserInfo(info);
            }
            if (t == null)
            {
                return Json(ApiResult<object>.Fail("注册失败"));
            }
            return Json(ApiResult<object>.OK(null));
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }
        public IActionResult Category(string category, int p)
        {
            var c = BookCommon.Categories.FirstOrDefault(m => Pinyin.GetPinYin(m.CategoryName) == category);
            if (c == null)
            {
                return RedirectToAction("Index");
            }
            ViewData["Title"] = string.Format("{0}_书客来_免费小说网站", c.CategoryName);
            ViewData["keywords"] = string.Format("{0},{0}阅读,{0}下载,{1}", c.CategoryName, "书客来");
            ViewData["description"] = string.Format("读{0}小说尽在小说网:{1}免费小说网。本站提供{0}小说阅读。以及{0}小说最新更新章节和{0}小说章节列表尽在小说网站-{1}小说网,www.shukelai.com", c.CategoryName, "书客来");
            if (p<=1)
            {
                ViewData["mobileurl"] = BookCommon.MOBILEURL.TrimEnd('/') + Url.Action("Category", "Index", new { category });
            }
            else
            {
                ViewData["mobileurl"] = BookCommon.MOBILEURL.TrimEnd('/') + Url.Action("Category", "Index", new { category, p });
            }

            ViewData["category"] = c;
            using (BookService bookService = new BookService())
            {
                var r = bookService.GetBooksByCategory(c.CategoryId, p, BookCommon.PAGESIZE);

                var pageOption = new MoPagerOption
                {
                    CurrentPage = p,
                    PageSize = BookCommon.PAGESIZE,
                    RouteUrl = Url.Action("Category", "Index"),
                    TotalPage = r.TotalPages
                };
                ViewBag.PagerOption = pageOption;
                ViewData["books"] = r;
            }

            return View();
        }
    }
}
