using System;
using System.Collections.Generic;

namespace Novel.Entity.Models
{
    public partial class BookLinks
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int SyncType { get; set; }
    }
}
