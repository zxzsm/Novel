using System;
using System.Collections.Generic;

namespace Novel.Entity.Models
{
    public partial class BookRecommend
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int DataYm { get; set; }
        public int? Order { get; set; }
        public DateTime Created { get; set; }
    }
}
