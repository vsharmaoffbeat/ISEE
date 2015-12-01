using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISEE.Common
{
     public class State
    {
        public bool loaded { get; set; }
        public bool opened { get; set; }
        public bool selected { get; set; }
        public bool disabled { get; set; }
    }

    public class LiAttr
    {
        public string id { get; set; }
    }

    public class AAttr
    {
        public string href { get; set; }
        public string id { get; set; }
    }

    public class Original
    {
        public string type { get; set; }
        public string text { get; set; }
    }

}