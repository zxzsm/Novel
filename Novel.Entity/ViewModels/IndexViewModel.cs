﻿using Novel.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Novel.Entity.Models
{
    public class IndexViewModel
    {
        /// <summary>
        /// 玄幻奇幻
        /// </summary>
        public List<Book> Fantasy { get; set; }

        public List<BookCategory> BookCategories { get; set; }
    }
}
