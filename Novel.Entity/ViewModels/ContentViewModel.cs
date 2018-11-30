using Novel.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Novel.Entity.Models
{
    public class ContentViewModel
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string Content { get; set; }

    }
}
