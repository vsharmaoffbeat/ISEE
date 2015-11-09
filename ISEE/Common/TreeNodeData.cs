using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISEE.Common
{
    public class TreeNodeData
    {
        public long id { get; set; }
        public string iconUrl { get; set; }
        public string liClass { get; set; }
        public string text { get; set; }
        public string textCss { get; set; }
        public string tooltip { get; set; }
        public List<TreeNodeData> children { get; set; }
        public int? objectid { get; set; }
        public string objecttype { get; set; }
        
        //public long? parent { get; set; }

        //public object icon { get; set; }
        //public int? objectid { get; set; }
        //public string objecttype { get; set; }
        //public List<string> parents { get; set; }

        //public object data { get; set; }
        //public State state { get; set; }
        //public LiAttr li_attr { get; set; }
        //public AAttr a_attr { get; set; }
        //public Original original { get; set; }

        //public List<TreeNodeData> children { get; set; }
    }
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