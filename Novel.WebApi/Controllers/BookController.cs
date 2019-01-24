using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Novel.Entity.Models;
using Novel.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Novel.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class BookController : Controller
    {
        const string IMAGEDOMAIN = "http://118.25.74.102:804";
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        [Route("index")]
        public IndexViewModel Index()
        {
            IndexViewModel viewModel = new IndexViewModel();
            using (BookContext bookContext = new BookContext())
            {
                var t = bookContext.Book.Take(10).ToList();
                viewModel.Fantasy = t;
                foreach (var item in viewModel.Fantasy)
                {
                    item.BookImage = IMAGEDOMAIN + item.BookImage;
                }
            }
            return viewModel;
        }
        [HttpPost]
        [Route("getbook")]
        public NovelViewModel GetBook(int id)
        {
            using (BookService service = new BookService())
            {
                NovelViewModel bookViewModel = service.GetBook(id);
                bookViewModel.Book.BookImage = bookViewModel.Book.BookImage = IMAGEDOMAIN + bookViewModel.Book.BookImage;
                return bookViewModel;
            }
        }


        // POST api/<controller>

    }
}
