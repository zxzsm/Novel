﻿using Novel.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Novel.Entity.Models
{
    public class NovelViewModel
    {
        public Book Book { get; set; }
        public List<BookItemViewModel> Items { get; set; }
        public bool IsThumbsup { get; set; }

        public BookCategory BookCategory { get; set; }
    }

    public class BookItemViewModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string Content { get; set; }
    }
}
