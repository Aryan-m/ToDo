using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDo.Models
{
    public class ToDo
    {
        public int ID { get; set; }
        public string content { get; set; }
        public bool is_done { get; set; }
        public int row_number { get; set; }
        public bool is_important { get; set; }
        public virtual ApplicationUser user { get; set; }
    }
}