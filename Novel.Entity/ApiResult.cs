using System;
using System.Collections.Generic;
using System.Text;

namespace Novel.Entity
{
    public class ApiResult
    {
        public int status { get; set; }
        public string msg { get; set; }
        public object data { get; set; }
    }
}
