using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISEE.Models
{
    public class jstreenode
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public jstreenode()
        {
            parent = "#";
            icon = "jstree-icon jstree-themeicon";
        }

    }
}