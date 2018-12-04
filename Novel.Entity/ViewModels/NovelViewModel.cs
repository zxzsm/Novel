using Novel.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Novel.Entity.Models
{
    public class NovelViewModel
    {
        public Book Book { get; set; }
        public List<BookItem> Items { get; set; }
    }
}
