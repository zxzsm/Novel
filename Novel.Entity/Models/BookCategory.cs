﻿using System;
using System.Collections.Generic;

namespace Novel.Entity.Models
{
    public partial class BookCategory
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
