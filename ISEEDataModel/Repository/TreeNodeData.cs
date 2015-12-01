using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISEEDataModel.Repository
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

        public TreeNodeData()
        {
            iconUrl = "/images/img/Blank300.png";
        }


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
   
}
