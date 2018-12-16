using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Novel.Controllers
{
    public class BackstageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Books()
        {
            return View();
        }
    }
}