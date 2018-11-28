using Novel.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Novel.Models
{
    public class BookViewModel
    {
        public Book Book { get; set; }
        public List<BookItem> Items { get; set; }
    }
}
