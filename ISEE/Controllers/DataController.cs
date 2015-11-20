using ISEE.Common;
using ISEEDataModel.Repository;
using ISEEREGION.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace ISEEREGION.Controllers
{
    public class DataController : Controller
    {
        //
        // GET: /Data/
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult UserLogin(LoginViewModel data)
        {
            using (ISEEEntities context = new ISEEEntities())
            {
                var query = from f in context.Factories.ToList()
                            join fp in context.FactoryParms on
                             f.FactoryId equals fp.FactoryId
                            join fd in context.FactoryDistances on
                            f.FactoryId equals fd.FactoryId
                            join country in context.Countries on
                            fp.Country equals country.CountryCode

                            where (String.Compare(f.UserName, data.UserName, StringComparison.OrdinalIgnoreCase) == 0) &&
                            (String.CompareOrdinal(f.Password, data.Password) == 0)

                            select new
                            {
                                FactoryGuid = (Guid)f.FactoryKey,
                                FactoryDesc = f.FactoryDesc,
                                FactoryID = f.FactoryId,
                                Country = fp.Country,
                                CountryDesc = country.CountryDesc,
                                StopEmployeeTime = (int)fp.StopEmployeeTime,
                                SlipTime = fp.SplitTime,
                                Lat = (double)fp.Lat,
                                Long = (double)fp.Long,
                                Zoom = (int)fp.Zoom,
                                MapProvider = (int)fp.MapProvider,
                                SmsProvider = (int)fp.SmsProvider,
                                PhoneAreaCode = fp.PhoneAreaCode,
                                CompanyLogo = fp.CompanyLogo,
                                CurrentGmt = country.CurrentGmt,
                                MapSearchModule = fd.MapSearchModule,
                                RadiusSearch = fd.RadiusSearch,
                                TopEmployeeSearch = fd.TopEmployeeSearch,
                                ZoomSearchLevel = fd.ZoomSearchLevel,
                                CalendarShowRadius = fd.CalendarShowRadius,
                                CalenderShowZoomLevel = fd.CalenderShowZoomLevel,
                            };





                var loginData = query.ToList()[0];
                SessionManegment.SessionManagement.FactoryID = loginData.FactoryID;

                SessionManegment.SessionManagement.FactoryDesc = loginData.FactoryDesc;
                SessionManegment.SessionManagement.CountryDesc = loginData.CountryDesc;
                SessionManegment.SessionManagement.FactoryID = loginData.FactoryID;
                SessionManegment.SessionManagement.Country = loginData.Country;
                SessionManegment.SessionManagement.CurrentGmt = loginData.CurrentGmt;
                SessionManegment.SessionManagement.SmsProvider = loginData.SmsProvider;
                SessionManegment.SessionManagement.PhoneAreaCode = loginData.PhoneAreaCode;
                //SessionManegment.SessionManagement. = loginData.;
                //SessionManegment.SessionManagement. = loginData.;
                //SessionManegment.SessionManagement. = loginData.;
                //SessionManegment.SessionManagement. = loginData.;
                //SessionManegment.SessionManagement. = loginData.;


            }


            //using (ISEEEntities context = new ISEEEntities())
            //{
            //    var user = context.Factories.Where(d => d.UserName == data.UserName && d.Password == data.Password).Select(x => x.FactoryId).FirstOrDefault();
            //}
            return new JsonResult { Data = 1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        #region Search for Employee
        public JsonResult GetEmployee(string manufacture, string lastname, string firstname, string empNumber, string phoneType, bool isActive)
        {
            firstname = string.IsNullOrEmpty(firstname) ? null : firstname;
            lastname = string.IsNullOrEmpty(lastname) ? null : lastname;
            empNumber = string.IsNullOrEmpty(empNumber) ? null : empNumber;

            using (ISEEEntities context = new ISEEEntities())
            {
                var empData = context.Employees.ToList().Where(x => x.Factory == ISEE.Common.SessionManegment.SessionManagement.FactoryID
                                                        && (string.IsNullOrEmpty(firstname) || x.FirstName.Contains(firstname))
                    //       x.FirstName.Contains(firstname == null ? x.FirstName : firstname)
                                                        && (string.IsNullOrEmpty(lastname) || x.LastName.Contains(lastname))
                                                          && (string.IsNullOrEmpty(empNumber) || x.EmployeeNum.Contains(empNumber))
                                                        && x.PhoneManufactory == (Common.GetInteger(manufacture) == 0 ? x.PhoneManufactory : Common.GetInteger(manufacture))
                                                                 && x.PhoneType == (Common.GetInteger(phoneType) == 0 ? x.PhoneType : Common.GetInteger(phoneType))
                                                                 && (isActive == true ? (x.EndDay == null || (x.EndDay != null && x.EndDay >= DateTime.Now)) : (x.EndDay != null && x.EndDay < DateTime.Now))).ToList().OrderBy(x => x.EmployeeNum).Select(x => new
                                                                 {
                                                                     EmployeeId = x.EmployeeId,
                                                                     EmployeeNum = x.EmployeeNum,
                                                                     Mail = x.Mail == null ? "" : x.Mail,
                                                                     FirstName = x.FirstName == null ? "" : x.FirstName,
                                                                     LastName = x.LastName == null ? "" : x.LastName,
                                                                     StartDay = x.StartDay == null ? "" : x.StartDay.Value.ToString("dd/MM/yyyy"),
                                                                     MainAreaPhone = x.MainAreaPhone == null ? "" : x.MainAreaPhone,
                                                                     MainPhone = x.MainPhone == null ? "" : x.MainPhone,
                                                                     SecondAreaPhone = x.SecondAreaPhone == null ? "" : x.SecondAreaPhone,
                                                                     SecondPhone = x.SecondPhone == null ? "" : x.SecondPhone,
                                                                     LastSendApp = x.LastSendApp == null ? "" : x.LastSendApp.Value.ToString("dd/MM/yyyy"),
                                                                     EndDay = x.EndDay == null ? "" : x.EndDay.Value.ToString("dd/MM/yyyy"),
                                                                     PhoneManufactory = x.PhoneManufactory,
                                                                     PhoneType = x.PhoneType

                                                                 }).ToList();


                return new JsonResult { Data = empData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public JsonResult GetMessageHistory(int employeeId, string start, string end)
        {
           DateTime startD=ConvertDateTime(start);
            DateTime endD=ConvertDateTime(end);
            using (ISEEEntities context = new ISEEEntities())
            {
                var msgHistory = context.EmployeeSmsSends.ToList().Where(x => x.EmployeeId == employeeId && x.SmsCreatDate >= startD
                        && x.SmsCreatDate <= endD)
                        .Select(x => new { SmsCreatDate = x.SmsCreatDate.ToString("dd/MM/yyyy HH:mm"), x.SmsMsg, x.SmsStatus, x.SmsCount }).ToList();

                return new JsonResult { Data = msgHistory, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public JsonResult GetEmployeeDiaryTemplate(int employeeId)
        {

            using (ISEEEntities context = new ISEEEntities())
            {
                var EmpHours = context.EmployeeDiaryTemplates.Where(s => s.EmployeeId == employeeId).ToList().Select(e => new { Day = ((ISEE.Controllers.AdminController.Days)Enum.ToObject(typeof(ISEE.Controllers.AdminController.Days), e.DayStatus)).ToString(), Start1 = e.Start1 != null ? Convert.ToDateTime(e.Start1.Value.ToString()).ToShortTimeString() : null, End1 = e.Stop1 != null ? Convert.ToDateTime(e.Stop1.Value.ToString()).ToShortTimeString() : null, Start2 = e.Start2 != null ? Convert.ToDateTime(e.Start2.Value.ToString()).ToShortTimeString() : null, End2 = e.Stop2 != null ? Convert.ToDateTime(e.Stop2.Value.ToString()).ToShortTimeString() : null, DayStatus = e.DayStatus }).ToList();

                //     var EmpHour = context.FactoryDairyTemplets.Where(s => s.Factory == factoryId).ToList().Select(e => new { Day = ((Days)Enum.ToObject(typeof(Days), e.DayStatus)).ToString(), Start1 = e.Start1 != null ? Convert.ToDateTime(e.Start1.Value.ToString()).ToShortTimeString() : null, End1 = e.Stop1 != null ? Convert.ToDateTime(e.Stop1.Value.ToString()).ToShortTimeString() : null, Start2 = e.Start2 != null ? Convert.ToDateTime(e.Start2.Value.ToString()).ToShortTimeString() : null, End2 = e.Stop2 != null ? Convert.ToDateTime(e.Stop2.Value.ToString()).ToShortTimeString() : null }).ToList();
                return new JsonResult { Data = EmpHours, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        public JsonResult GetEmployeeTimeHistoryDiary(int employeeId, int month, int year)
        {
            var start = new DateTime(year, month, 1);
            var end = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            using (ISEEEntities context = new ISEEEntities())
            {
                var EmpHours = context.EmployeeDiaryTimes.Where(s => s.EmployeeId == employeeId && s.Day >= start && s.Day <= end).ToList().Select(e => new { Day = e.Day.ToString("dd/MM/yyyy"), Start1 = e.Start1 != null ? Convert.ToDateTime(e.Start1.Value.ToString()).ToString("h:mm tt") : null, End1 = e.Stop1 != null ? Convert.ToDateTime(e.Stop1.Value.ToString()).ToString("h:mm tt") : null, Start2 = e.Start2 != null ? Convert.ToDateTime(e.Start2.Value.ToString()).ToString("h:mm tt") : null, End2 = e.Stop2 != null ? Convert.ToDateTime(e.Stop2.Value.ToString()).ToString("h:mm tt") : null, Start3 = e.Start3 != null ? Convert.ToDateTime(e.Start3.Value.ToString()).ToString("h:mm tt") : null, End3 = e.Stop3 != null ? Convert.ToDateTime(e.Stop3.Value.ToString()).ToString("h:mm tt") : null }).ToList();
                return new JsonResult { Data = EmpHours, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public string SendMessage(int employeeId, string msg, string phoneNumber)
        {
            string result = string.Empty;

            switch (SessionManegment.SessionManagement.SmsProvider)
            {
                case 1:
                    result = SenderSMS(employeeId, msg, phoneNumber);
                    break;
                case 2:
                    result = SenderSMSMexico(employeeId, msg, phoneNumber, SessionManegment.SessionManagement.CurrentGmt);
                    break;
                case 3:
                    //  strPhone = vm_emp.SelectedEmployee.MainAreaPhone.TrimStart('0').Trim() + vm_emp.SelectedEmployee.MainPhone.Trim();
                    result = SendSMSClickatell(employeeId, msg, phoneNumber, SessionManegment.SessionManagement.PhoneAreaCode, SessionManegment.SessionManagement.CurrentGmt);
                    break;

            }


            return "true";

        }



        public string SenderSMS(int EmpID, string _strMsg, string _phone)
        {
            string UserName = "sparta";
            string Password = "5632455";
            string msg = System.Security.SecurityElement.Escape(_strMsg);
            string senderName = "regionSEE";
            string senderNumber = "5632455";

            //set phone numbers "0545500378;0545500379;"
            string phonesList = _phone;


            //create XML
            StringBuilder cbXml = new StringBuilder();
            cbXml.Append("<Inforu>");
            cbXml.Append("<User>");
            cbXml.Append("<Username>" + UserName + "</Username>");
            cbXml.Append("<Password>" + Password + "</Password>");
            cbXml.Append("</User>");
            cbXml.Append("<Content Type=\"sms\">");
            cbXml.Append("<Message>" + msg + "</Message>");
            cbXml.Append("</Content>");
            cbXml.Append("<Recipients>");
            cbXml.Append("<PhoneNumber>" + phonesList + "</PhoneNumber>");
            cbXml.Append("</Recipients>");
            cbXml.Append("<Settings>");
            cbXml.Append("<SenderName>" + senderName + "</SenderName>");
            cbXml.Append("<SenderNumber>" + senderNumber + "</SenderNumber>");
            cbXml.Append("</Settings>");
            cbXml.Append("</Inforu>");

            string strXML = HttpUtility.UrlEncode(cbXml.ToString(), System.Text.Encoding.UTF8);

            string result = PostDataToURL("http://api.inforu.co.il/SendMessageXml.ashx", "InforuXML=" + strXML);

            //one time get result empty(check )!!!!!!
            int Status = 1;
            if (!string.IsNullOrEmpty(result))
            {
                XmlDocument xmlRez = new XmlDocument();
                xmlRez.LoadXml(result);
                XmlNode xnNote = xmlRez.SelectSingleNode("Result");
                Status = Convert.ToInt32(xnNote["Status"].InnerText);
            }


            //add and save row to DB
            EmployeeSmsSend emp_sms = new EmployeeSmsSend();
            emp_sms.EmployeeId = EmpID;
            emp_sms.SmsCreatDate = DateTime.Now.AddHours(SessionManegment.SessionManagement.CurrentGmt);
            emp_sms.SmsMsg = _strMsg;
            emp_sms.SmsCount = 1;
            emp_sms.SmsStatus = Convert.ToInt32(Status);

            //this.ObjectContext.EmployeeSmsSend.AddObject(emp_sms);
            //this.ObjectContext.SaveChanges();

            return result;
        }


        public string SenderSMSMexico(int empID, string _strMsg, string _phone, double CurrentGmt)
        {

            // string strXML1 = "Appname=Port2SMS&prgname=HTTP_SimpleSMS1&AccountID=1037&UserID=10130&UserPass=1037&Phone=0506447976&Text=Test";
            string strXML = "Appname=Port2SMS&prgname=HTTP_SimpleSMS1&AccountID=1037&UserID=10130&UserPass=1037&Phone=" + _phone + "&Text=" + _strMsg;

            string result = PostDataToURL("http://ign-sms.com/Scripts/mgrqispi.dll?", strXML);

            ////one time get result empty(check )!!!!!!
            int Status;
            if (result.Contains("OK"))
                Status = 1;
            else
                Status = -1;

            //add and save row to DB
            EmployeeSmsSend emp_sms = new EmployeeSmsSend();
            emp_sms.EmployeeId = empID;
            emp_sms.SmsCreatDate = DateTime.Now.AddHours(CurrentGmt);
            emp_sms.SmsMsg = _strMsg;
            emp_sms.SmsCount = 1;
            emp_sms.SmsStatus = Convert.ToInt32(Status);
            using (ISEEEntities context = new ISEEEntities())
            {
                context.EmployeeSmsSends.Add(emp_sms);
                context.SaveChanges();
            }
            return result;
        }




        public string SendSMSClickatell(int empID, string _strMsg, string _phone, string _PhoneAreaCode, double CurrentGmt)
        {
            string result = "1";

            if (_strMsg.Length < 70) // is limited to 70 characters per message(Clickatell) for unicode
            {
                //    string phone = _PhoneAreaCode + _phone;  //972545500378
                string phone = _PhoneAreaCode + "505774499";
                WebClient client = new WebClient();
                // Add a user agent header in case the requested URI contains a query.
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                client.QueryString.Add("user", "Dshabi");
                client.QueryString.Add("password", "shabi101827");
                client.QueryString.Add("api_id", "3349639");
                client.QueryString.Add("to", phone);
                client.QueryString.Add("from", "regionSEE");
                client.QueryString.Add("text", ToUnicode(_strMsg));
                client.QueryString.Add("unicode", "1");
                string baseurl = "http://api.clickatell.com/http/sendmsg";
                Stream data = client.OpenRead(baseurl);
                StreamReader reader = new StreamReader(data);
                string s = reader.ReadToEnd();
                data.Close();
                reader.Close();


            }
            else
                result = "-1";


            //add and save row to DB
            EmployeeSmsSend emp_sms = new EmployeeSmsSend();
            emp_sms.EmployeeId = empID;
            emp_sms.SmsCreatDate = DateTime.Now.AddHours(CurrentGmt);
            emp_sms.SmsMsg = _strMsg;
            emp_sms.SmsCount = 1;
            emp_sms.SmsStatus = Convert.ToInt32(result);
            using (ISEEEntities context = new ISEEEntities())
            {
                context.EmployeeSmsSends.Add(emp_sms);
                context.SaveChanges();
            }
            return (result);


        }
        private string ToUnicode(string val)
        {

            Encoding enc = Encoding.BigEndianUnicode;
            byte[] intermediate = enc.GetBytes(val);
            StringBuilder sb = new StringBuilder(intermediate.Length * 2);
            foreach (byte b in intermediate)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }


        public string SendSms(string _strMsg, string phoneNumber, int employeeId)
        {

            string sss = SessionManegment.SessionManagement.PhoneAreaCode;

            //  string _strMsg = "";
            string UserName = "sparta";
            string Password = "5632455";
            string msg = System.Security.SecurityElement.Escape(_strMsg);
            string senderName = "regionSEE";
            string senderNumber = "5632455";

            //set phone numbers "0545500378;0545500379;"
            string phonesList = SessionManegment.SessionManagement.PhoneAreaCode + "0505774499";


            //create XML
            StringBuilder cbXml = new StringBuilder();
            cbXml.Append("<Inforu>");
            cbXml.Append("<User>");
            cbXml.Append("<Username>" + UserName + "</Username>");
            cbXml.Append("<Password>" + Password + "</Password>");
            cbXml.Append("</User>");
            cbXml.Append("<Content Type=\"sms\">");
            cbXml.Append("<Message>" + msg + "</Message>");
            cbXml.Append("</Content>");
            cbXml.Append("<Recipients>");
            cbXml.Append("<PhoneNumber>" + phonesList + "</PhoneNumber>");
            cbXml.Append("</Recipients>");
            cbXml.Append("<Settings>");
            cbXml.Append("<SenderName>" + senderName + "</SenderName>");
            cbXml.Append("<SenderNumber>" + senderNumber + "</SenderNumber>");
            cbXml.Append("</Settings>");
            cbXml.Append("</Inforu>");

            string strXML = HttpUtility.UrlEncode(cbXml.ToString(), System.Text.Encoding.UTF8);

            string result = PostDataToURL("http://api.inforu.co.il/SendMessageXml.ashx", "InforuXML=" + strXML);

            //one time get result empty(check )!!!!!!
            int Status = 1;
            if (!string.IsNullOrEmpty(result))
            {
                XmlDocument xmlRez = new XmlDocument();
                xmlRez.LoadXml(result);
                XmlNode xnNote = xmlRez.SelectSingleNode("Result");
                Status = Convert.ToInt32(xnNote["Status"].InnerText);
            }


            //add and save row to DB
            EmployeeSmsSend emp_sms = new EmployeeSmsSend();
            emp_sms.EmployeeId = employeeId;
            emp_sms.SmsCreatDate = DateTime.Now.AddHours(SessionManegment.SessionManagement.CurrentGmt);
            emp_sms.SmsMsg = _strMsg;
            emp_sms.SmsCount = 1;
            emp_sms.SmsStatus = Convert.ToInt32(Status);
            using (ISEEEntities context = new ISEEEntities())
            {
                context.EmployeeSmsSends.Add(emp_sms);
                context.SaveChanges();
            }
            //this.ObjectContext.EmployeeSmsSend.AddObject(emp_sms);
            //this.ObjectContext.SaveChanges();

            return result;

        }
        static string PostDataToURL(string szUrl, string szData)
        {
            //Setup web request
            string szResult = string.Empty;
            WebRequest Request = WebRequest.Create(szUrl);
            Request.Timeout = 3000;
            Request.Method = "POST";
            Request.ContentType = "application/x-www-form-urlencoded";

            //Set the POST data in a buffer
            byte[] PostBuffer;
            try
            {
                //replacing " " with "+" according to Http post RPC
                szData = szData.Replace(" ", "+");

                //Specify the length of the buffer
                PostBuffer = Encoding.UTF8.GetBytes(szData);
                Request.ContentLength = PostBuffer.Length;

                //Open up a request stream
                Stream RequestStream = Request.GetRequestStream();

                //Write the POST data
                RequestStream.Write(PostBuffer, 0, PostBuffer.Length);

                //Close the stream
                RequestStream.Close();
                //Create the response object
                WebResponse Response;
                Response = Request.GetResponse();

                //Create the reader for the response
                StreamReader sr = new StreamReader(Response.GetResponseStream(), Encoding.UTF8);

                //Read the response
                szResult = sr.ReadToEnd();

                //Close the reader and response
                sr.Close();
                Response.Close();

                return szResult;
            }
            catch (Exception ex)
            {
                return szResult;
            }
        }

        public string UpdateEmployee(int employeeId, string number, string mail, string firstName, string lastName, string phone1, string phone11, string phone2, string phone22, string Start, int manufacture, int phoneType, string end, string hourlyData)
        {
            var mainData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ISEEDataModel.Repository.employeeHours>>(hourlyData);
            int days = 0;
            try
            {


                foreach (var item in mainData)
                {
                    days = Int32.Parse(item.Day);
                    using (ISEEEntities db = new ISEEEntities())
                    {

                        ISEEDataModel.Repository.EmployeeDiaryTemplate factoryDairyTemplet = db.EmployeeDiaryTemplates.Where(x => x.EmployeeId == employeeId && x.DayStatus == days).FirstOrDefault();
                        if (factoryDairyTemplet != null)
                        {
                            factoryDairyTemplet.Start1 = Convert.ToDateTime(item.Start1).ToShortTimeString() != null ? (new TimeSpan(Int32.Parse(Convert.ToDateTime(item.Start1).ToShortTimeString().Split(':')[0]), Int32.Parse((Convert.ToDateTime(item.Start1).ToShortTimeString().Split(':')[1]).Split(' ')[0]), 0)) : new TimeSpan(0);
                            factoryDairyTemplet.Stop1 = Convert.ToDateTime(item.End1).ToShortTimeString() != null ? (new TimeSpan(Int32.Parse(Convert.ToDateTime(item.End1).ToShortTimeString().Split(':')[0]), Int32.Parse((Convert.ToDateTime(item.End1).ToShortTimeString().Split(':')[1]).Split(' ')[0]), 0)) : new TimeSpan(0);
                            factoryDairyTemplet.Start2 = Convert.ToDateTime(item.Start2).ToShortTimeString() != null ? (new TimeSpan(Int32.Parse(Convert.ToDateTime(item.Start2).ToShortTimeString().Split(':')[0]), Int32.Parse((Convert.ToDateTime(item.Start2).ToShortTimeString().Split(':')[1]).Split(' ')[0]), 0)) : new TimeSpan();
                            factoryDairyTemplet.Stop2 = Convert.ToDateTime(item.End2).ToShortTimeString() != null ? (new TimeSpan(Int32.Parse(Convert.ToDateTime(item.End2).ToShortTimeString().Split(':')[0]), Int32.Parse((Convert.ToDateTime(item.End2).ToShortTimeString().Split(':')[1]).Split(' ')[0]), 0)) : new TimeSpan(0);
                            db.SaveChanges();
                        }
                    }

                }
                using (ISEEEntities db = new ISEEEntities())
                {

                    Employee employee = db.Employees.Where(x => x.EmployeeId == employeeId).FirstOrDefault();
                    if (employee != null)
                    {
                        employee.EmployeeNum = number;
                        employee.Mail = mail;
                        employee.FirstName = firstName;
                        employee.LastName = lastName;
                        employee.MainAreaPhone = phone1;
                        employee.MainPhone = phone11;
                        employee.SecondAreaPhone = phone2;
                        employee.SecondPhone = phone22;
                        employee.PhoneManufactory = manufacture;
                        employee.PhoneType = phoneType;
                        if (!string.IsNullOrEmpty(Start))
                            employee.StartDay = ConvertDateTime(Start);
                        if (!string.IsNullOrEmpty(end))
                            employee.EndDay = ConvertDateTime(end);
                        db.SaveChanges();

                    }
                }
            }
            catch (Exception)
            {

                return "false";
            }
            return "true";
        }
        #endregion

        public DateTime ConvertDateTime(string date) {
            return new DateTime(Convert.ToInt32(date.Split('/')[2]), Convert.ToInt32(date.Split('/')[1]), Convert.ToInt32(date.Split('/')[0]));
        }

        #region Country Data Common Methods

        public JsonResult GetStatesByFactoryID()
        {
            using (ISEEEntities context = new ISEEEntities())
            {
                //int FactoryId = ISEE.Common.SessionManegment.SessionManagement.FactoryID;
                //var CountryID = context.FactoryParms.Where(F => F.FactoryId == FactoryId).Select(s => new { CountryID = s.Country }).FirstOrDefault();

                int FactoryId = ISEE.Common.SessionManegment.SessionManagement.FactoryID;
                var CountryID = ISEE.Common.SessionManegment.SessionManagement.Country;
                if (CountryID != null)
                {

                    var StateDec = context.States.Where(c => c.CountryCode == CountryID).Select(x => new { CountryCode = x.StateCode, CountryDescEng = x.StateDesc }).ToList();
                    return new JsonResult { Data = StateDec, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        public JsonResult GetCitysByState(int stateID)
        {
            using (ISEEEntities context = new ISEEEntities())
            {
                if (stateID != null)
                {
                    var Cityes = context.Cities.Where(p => p.StateCode == stateID).Select(d => new { CityCode = d.CityCode, CityDesc = d.CityDesc }).ToList();
                    return new JsonResult { Data = Cityes, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        public JsonResult GetStreetByCity(int stateID, int cityID)
        {
            using (ISEEEntities context = new ISEEEntities())
            {
                if (cityID != null)
                {
                    var Streets = context.Streets.Where(p => p.CityCode == cityID && p.StateCode == stateID).Select(d => new { StreetCode = d.StreetCode, Streetdesc = d.StreetDesc }).ToList();
                    return new JsonResult { Data = Streets, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public JsonResult GetBuildingsByCity(int streetID, int stateID, int cityID)
        {
            using (ISEEEntities context = new ISEEEntities())
            {
                if (streetID != null)
                {
                    var Buildings = context.Buildings.Where(p => p.StreetCode == streetID && p.StateCode == stateID && p.CityCode == cityID).Select(d => new { BuildingCode = d.BuildingCode, BuildingLat = d.Lat, BuldingLong = d.Long, BuildingNumber = d.Number }).ToList();
                    return new JsonResult { Data = Buildings, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }



        #endregion
    }
}
