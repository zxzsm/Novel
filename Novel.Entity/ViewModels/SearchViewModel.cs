using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Novel.Entity.ViewModels
{
    public class SearchViewModel
    {
        public string keyword { get; set; }
        public int pageSize { get; set; }
        public int pageIndex { get; set; }
        public List<int> categories { get; set; }
        public List<double> wordNumbers { get; set; }
        public List<int> states { get; set; }
        public bool? nocategory { get; set; }
    }
}
