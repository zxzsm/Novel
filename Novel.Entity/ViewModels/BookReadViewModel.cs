using System;
using System.Collections.Generic;
using System.Text;

namespace Novel.Entity.ViewModels
{
    public class BookReadViewModel
    {
        public int bookid { get; set; }
        public string bookurl { get; set; }
        public string bookauthor { get; set; }

        public string bookname { get; set; }
        public int currentreaditemid { get; set; }
        public string currentitemname { get; set; }
        public string currentitemurl { get; set; }
        public DateTime update { get; set; }
    }
}
