using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISEE.Common
{
    public static class SessionManagement
    {

        public static void Clear()
        {
            HttpContext.Current.Session.Clear();
        }

        public static int FactoryID
        {
            get { return Convert.ToInt16(HttpContext.Current.Session["FactoryID"]); }
            set { HttpContext.Current.Session["FactoryID"] = value; }

        }
        public static string Language
        {
            get
            {
                if (HttpContext.Current.Session["Language"] != null)
                    return Convert.ToString(HttpContext.Current.Session["Language"]);
                else
                {
                    //Need to remove when images names changes to Similar we get from request
                    if (HttpContext.Current.Request.UserLanguages[0].ToLower().Contains("en"))
                    {
                        HttpContext.Current.Session["Language"] = "EN";
                    }
                    else
                    {
                        HttpContext.Current.Session["Language"] = HttpContext.Current.Request.UserLanguages[0].ToUpper().ToString();
                    }
                    return Convert.ToString(HttpContext.Current.Session["Language"]);
                }
            }
            set { HttpContext.Current.Session["Language"] = value; }
        }
        public static string FactoryDesc
        {
            get
            {
                if (HttpContext.Current.Session["FactoryDesc"] != null)
                    return Convert.ToString(HttpContext.Current.Session["FactoryDesc"]);
                else
                    return "";
            }
            set { HttpContext.Current.Session["FactoryDesc"] = value; }
        }
        public static int Country
        {
            get
            {
                if (HttpContext.Current.Session["Country"] != null)
                    return Convert.ToInt32(HttpContext.Current.Session["Country"]);
                else
                    return 0;
            }
            set { HttpContext.Current.Session["Country"] = value; }
        }
        public static DateTime StopEmpTime
        {
            get
            {
                if (HttpContext.Current.Session["StopEmpTime"] == null) { return DateTime.Now; }
                else { return Convert.ToDateTime(HttpContext.Current.Session["StopEmpTime"]); }
            }
            set { HttpContext.Current.Session["StopEmpTime"] = value; }
        }

        public static DateTime SplitTime
        {
            get
            {
                if (HttpContext.Current.Session["SplitTime"] == null) { return DateTime.Now; }
                else { return Convert.ToDateTime(HttpContext.Current.Session["SplitTime"]); }
            }
            set { HttpContext.Current.Session["SplitTime"] = value; }
        }

        public static string CurrentLang
        {
            get
            {
                if (HttpContext.Current.Session["CurrentLang"] != null)
                    return Convert.ToString(HttpContext.Current.Session["CurrentLang"]);
                else
                    return "";
            }
            set { HttpContext.Current.Session["CurrentLang"] = value; }
        }

        public static string Lat
        {
            get
            {
                if (HttpContext.Current.Session["Lat"] != null)
                    return Convert.ToString(HttpContext.Current.Session["Lat"]);
                else
                    return "";
            }
            set { HttpContext.Current.Session["Lat"] = value; }
        }
        public static string Long
        {
            get
            {
                if (HttpContext.Current.Session["Long"] != null)
                    return Convert.ToString(HttpContext.Current.Session["Long"]);
                else
                    return "";
            }
            set { HttpContext.Current.Session["Long"] = value; }
        }

        public static string Zoom
        {
            get
            {
                if (HttpContext.Current.Session["Zoom"] != null)
                    return Convert.ToString(HttpContext.Current.Session["Zoom"]);
                else
                    return "";
            }
            set { HttpContext.Current.Session["Zoom"] = value; }
        }

        public static int MapProvider
        {
            get { return Convert.ToInt16(HttpContext.Current.Session["MapProvider"]); }
            set { HttpContext.Current.Session["MapProvider"] = value; }
        }

        public static int SmsProvider
        {
            get { return Convert.ToInt16(HttpContext.Current.Session["SmsProvider"]); }
            set { HttpContext.Current.Session["SmsProvider"] = value; }
        }

        public static string PhoneAreaCode
        {
            get
            {
                if (HttpContext.Current.Session["PhoneAreaCode"] != null)
                    return Convert.ToString(HttpContext.Current.Session["PhoneAreaCode"]);
                else
                    return "";
            }
            set { HttpContext.Current.Session["PhoneAreaCode"] = value; }
        }

        public static string CountryDesc
        {
            get
            {
                if (HttpContext.Current.Session["CountryDesc"] != null)
                    return Convert.ToString(HttpContext.Current.Session["CountryDesc"]);
                else
                    return "";
            }
            set { HttpContext.Current.Session["CountryDesc"] = value; }
        }

        public static string CompanyLogo
        {
            get
            {
                if (HttpContext.Current.Session["CompanyLogo"] != null)
                    return Convert.ToString(HttpContext.Current.Session["CompanyLogo"]);
                else
                    return "";
            }
            set { HttpContext.Current.Session["CompanyLogo"] = value; }
        }

        public static Double CurrentGmt
        {
            get
            {
                if (HttpContext.Current.Session["CurrentGmt"] == null) { return 0; }
                else { return Convert.ToDouble(HttpContext.Current.Session["CurrentGmt"]); }
            }
            set { HttpContext.Current.Session["CurrentGmt"] = value; }
        }

        public static string MapSearchModule
        {
            get
            {
                if (HttpContext.Current.Session["MapSearchModule"] != null)
                    return Convert.ToString(HttpContext.Current.Session["MapSearchModule"]);
                else
                    return "";
            }
            set { HttpContext.Current.Session["MapSearchModule"] = value; }
        }

        public static string RadiusSearch
        {
            get
            {
                if (HttpContext.Current.Session["RadiusSearch"] != null)
                    return Convert.ToString(HttpContext.Current.Session["RadiusSearch"]);
                else
                    return "";
            }
            set { HttpContext.Current.Session["RadiusSearch"] = value; }
        }

        public static string TopEmployeeSearch
        {
            get
            {
                if (HttpContext.Current.Session["TopEmployeeSearch"] != null)
                    return Convert.ToString(HttpContext.Current.Session["TopEmployeeSearch"]);
                else
                    return "";
            }
            set { HttpContext.Current.Session["TopEmployeeSearch"] = value; }
        }

        public static string ZoomSearchLevel
        {
            get
            {
                if (HttpContext.Current.Session["ZoomSearchLevel"] != null)
                    return Convert.ToString(HttpContext.Current.Session["ZoomSearchLevel"]);
                else
                    return "";
            }
            set { HttpContext.Current.Session["ZoomSearchLevel"] = value; }
        }

        public static string CalendarShowRadius
        {
            get
            {
                if (HttpContext.Current.Session["CalendarShowRadius"] != null)
                    return Convert.ToString(HttpContext.Current.Session["CalendarShowRadius"]);
                else
                    return "";
            }
            set { HttpContext.Current.Session["CalendarShowRadius"] = value; }
        }

        public static string CalenderShowZoomLevel
        {
            get
            {
                if (HttpContext.Current.Session["CalenderShowZoomLevel"] != null)
                    return Convert.ToString(HttpContext.Current.Session["CalenderShowZoomLevel"]);
                else
                    return "";
            }
            set { HttpContext.Current.Session["CalenderShowZoomLevel"] = value; }
        }

        public static string Sunday
        {
            get
            {
                if (HttpContext.Current.Session["Sunday"] != null)
                    return Convert.ToString(HttpContext.Current.Session["Sunday"]);
                else
                    return "";
            }
            set { HttpContext.Current.Session["Sunday"] = value; }
        }
        public static string Monday
        {
            get
            {
                if (HttpContext.Current.Session["Monday"] != null)
                    return Convert.ToString(HttpContext.Current.Session["Monday"]);
                else
                    return "";
            }
            set { HttpContext.Current.Session["Monday"] = value; }
        }
        public static string Tuesday
        {
            get
            {
                if (HttpContext.Current.Session["Tuesday"] != null)
                    return Convert.ToString(HttpContext.Current.Session["Tuesday"]);
                else
                    return "";
            }
            set { HttpContext.Current.Session["Tuesday"] = value; }
        }
        public static string Wednesday
        {
            get
            {
                if (HttpContext.Current.Session["Wednesday"] != null)
                    return Convert.ToString(HttpContext.Current.Session["Wednesday"]);
                else
                    return "";
            }
            set { HttpContext.Current.Session["Wednesday"] = value; }
        }
        public static string Thursday
        {
            get
            {
                if (HttpContext.Current.Session["Thursday"] != null)
                    return Convert.ToString(HttpContext.Current.Session["Thursday"]);
                else
                    return "";
            }
            set { HttpContext.Current.Session["Thursday"] = value; }
        }
        public static string Friday
        {
            get
            {
                if (HttpContext.Current.Session["Friday"] != null)
                    return Convert.ToString(HttpContext.Current.Session["Friday"]);
                else
                    return "";
            }
            set { HttpContext.Current.Session["Friday"] = value; }
        }
        public static string Saturday
        {
            get
            {
                if (HttpContext.Current.Session["Saturday"] != null)
                    return Convert.ToString(HttpContext.Current.Session["Saturday"]);
                else
                    return "";
            }
            set { HttpContext.Current.Session["Saturday"] = value; }
        }

    }
}