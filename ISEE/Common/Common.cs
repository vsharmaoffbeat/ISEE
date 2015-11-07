using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISEE.Common
{
    public class Common
    {
        public static int GetInteger(string val)
        {

            int output;
            int.TryParse(val, out output);
            //if (isNaN(output))
            //    return output;
            return output;
        }
    }
}