using System;
using System.Collections.Generic;
using System.Text;

namespace Novel.Entity.ViewModels
{
    public class BookReadViewModel
    {
        public int id { get; set; }
        public int bookid { get; set; }
        public string bookurl { get; set; }
        public string bookauthor { get; set; }

        public string bookname { get; set; }
        public string bookimage { get; set; }
        public int currentreaditemid { get; set; }
        public string currentitemname { get; set; }
        public string currentitemurl { get; set; }
        public int lastitemid { get; set; }
        public string lastitemname { get; set; }
        public string lasitemurl { get; set; }
        /// <summary>
        /// 上次阅读时间
        /// </summary>
        public DateTime lastreadtime { get; set; }
        /// <summary>
        /// 最后一次更新时间
        /// </summary>
        public DateTime update { get; set; }
    }
}
