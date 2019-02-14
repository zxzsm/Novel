using System;
using System.Collections.Generic;

namespace Novel.Entity.Models
{
    public partial class TaskToDo
    {
        public int Id { get; set; }
        public DateTime EndTime { get; set; }
        public string Remark { get; set; }
    }
}
