using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISEE.Common
{
    public class TreeNodeData
    {
        public string id { get; set; }
        public string text { get; set; }
        public object icon { get; set; }
        public LiAttr li_attr { get; set; }
        public AAttr a_attr { get; set; }
        public State state { get; set; }
        public Data data { get; set; }
        public string parent { get; set; }
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

    public class State
    {
        public bool loaded { get; set; }
        public bool opened { get; set; }
        public bool selected { get; set; }
        public bool disabled { get; set; }
    }

    public class Data
    {
    }

    //public class TreeNodes
    //{
        
    //}
}