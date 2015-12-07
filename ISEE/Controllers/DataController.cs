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
using ISEEDataModel.Repository.Services;

namespace ISEEREGION.Controllers
{
    public class DataController : Controller
    {
        //
        // GET: /Data/  
        ISEEFactory _facory = new ISEEFactory();
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult UserLogin(LoginViewModel data)
        {
            try
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
                    SessionManagement.FactoryID = loginData.FactoryID;
                    SessionManagement.FactoryDesc = loginData.FactoryDesc;
                    SessionManagement.CountryDesc = loginData.CountryDesc;
                    SessionManagement.FactoryID = loginData.FactoryID;
                    SessionManagement.Country = loginData.Country;
                    SessionManagement.CurrentGmt = loginData.CurrentGmt;
                    SessionManagement.SmsProvider = loginData.SmsProvider;
                    SessionManagement.PhoneAreaCode = loginData.PhoneAreaCode;
                    //SessionManegment.SessionManagement. = loginData.;
                    //SessionManegment.SessionManagement. = loginData.;
                    //SessionManegment.SessionManagement. = loginData.;
                    //SessionManegment.SessionManagement. = loginData.;
                    //SessionManegment.SessionManagement. = loginData.;
                    var defaultLang = context.MainStringTables.ToList();
                    if (SessionManagement.Language == "HE")
                    {
                        SessionManagement.Sunday = defaultLang.Where(x => x.DefaultLanguage == "Sunday").Select(x => x.HE).FirstOrDefault();
                        SessionManagement.Monday = defaultLang.Where(x => x.DefaultLanguage == "Monday").Select(x => x.HE).FirstOrDefault();
                        SessionManagement.Tuesday = defaultLang.Where(x => x.DefaultLanguage == "Tuesday").Select(x => x.HE).FirstOrDefault();
                        SessionManagement.Wednesday = defaultLang.Where(x => x.DefaultLanguage == "Wednesday").Select(x => x.HE).FirstOrDefault();
                        SessionManagement.Thursday = defaultLang.Where(x => x.DefaultLanguage == "Thursday").Select(x => x.HE).FirstOrDefault();
                        SessionManagement.Friday = defaultLang.Where(x => x.DefaultLanguage == "Friday").Select(x => x.HE).FirstOrDefault();
                        SessionManagement.Saturday = defaultLang.Where(x => x.DefaultLanguage == "Saturday").Select(x => x.HE).FirstOrDefault();
                        //SessionManagement.Language 
                    }
                    else if (SessionManagement.Language == "RU")
                    {
                        SessionManagement.Sunday = defaultLang.Where(x => x.DefaultLanguage == "Sunday").Select(x => x.RU).FirstOrDefault();
                        SessionManagement.Monday = defaultLang.Where(x => x.DefaultLanguage == "Monday").Select(x => x.RU).FirstOrDefault();
                        SessionManagement.Tuesday = defaultLang.Where(x => x.DefaultLanguage == "Tuesday").Select(x => x.RU).FirstOrDefault();
                        SessionManagement.Wednesday = defaultLang.Where(x => x.DefaultLanguage == "Wednesday").Select(x => x.RU).FirstOrDefault();
                        SessionManagement.Thursday = defaultLang.Where(x => x.DefaultLanguage == "Thursday").Select(x => x.RU).FirstOrDefault();
                        SessionManagement.Friday = defaultLang.Where(x => x.DefaultLanguage == "Friday").Select(x => x.RU).FirstOrDefault();
                        SessionManagement.Saturday = defaultLang.Where(x => x.DefaultLanguage == "Saturday").Select(x => x.RU).FirstOrDefault();
                        //SessionManagement.Language 
                    }

                    else if (SessionManagement.Language == "ES")
                    {
                        SessionManagement.Sunday = defaultLang.Where(x => x.DefaultLanguage == "Sunday").Select(x => x.ES).FirstOrDefault();
                        SessionManagement.Monday = defaultLang.Where(x => x.DefaultLanguage == "Monday").Select(x => x.ES).FirstOrDefault();
                        SessionManagement.Tuesday = defaultLang.Where(x => x.DefaultLanguage == "Tuesday").Select(x => x.ES).FirstOrDefault();
                        SessionManagement.Wednesday = defaultLang.Where(x => x.DefaultLanguage == "Wednesday").Select(x => x.ES).FirstOrDefault();
                        SessionManagement.Thursday = defaultLang.Where(x => x.DefaultLanguage == "Thursday").Select(x => x.ES).FirstOrDefault();
                        SessionManagement.Friday = defaultLang.Where(x => x.DefaultLanguage == "Friday").Select(x => x.ES).FirstOrDefault();
                        SessionManagement.Saturday = defaultLang.Where(x => x.DefaultLanguage == "Saturday").Select(x => x.ES).FirstOrDefault();
                        //SessionManagement.Language 
                    }

                    else if (SessionManagement.Language == "DE")
                    {
                        SessionManagement.Sunday = defaultLang.Where(x => x.DefaultLanguage == "Sunday").Select(x => x.DE).FirstOrDefault();
                        SessionManagement.Monday = defaultLang.Where(x => x.DefaultLanguage == "Monday").Select(x => x.DE).FirstOrDefault();
                        SessionManagement.Tuesday = defaultLang.Where(x => x.DefaultLanguage == "Tuesday").Select(x => x.DE).FirstOrDefault();
                        SessionManagement.Wednesday = defaultLang.Where(x => x.DefaultLanguage == "Wednesday").Select(x => x.DE).FirstOrDefault();
                        SessionManagement.Thursday = defaultLang.Where(x => x.DefaultLanguage == "Thursday").Select(x => x.DE).FirstOrDefault();
                        SessionManagement.Friday = defaultLang.Where(x => x.DefaultLanguage == "Friday").Select(x => x.DE).FirstOrDefault();
                        SessionManagement.Saturday = defaultLang.Where(x => x.DefaultLanguage == "Saturday").Select(x => x.DE).FirstOrDefault();
                        //SessionManagement.Language 
                    }

                    else
                    {
                        SessionManagement.Sunday = defaultLang.Where(x => x.DefaultLanguage == "Sunday").Select(x => x.EN).FirstOrDefault();
                        SessionManagement.Monday = defaultLang.Where(x => x.DefaultLanguage == "Monday").Select(x => x.EN).FirstOrDefault();
                        SessionManagement.Tuesday = defaultLang.Where(x => x.DefaultLanguage == "Tuesday").Select(x => x.EN).FirstOrDefault();
                        SessionManagement.Wednesday = defaultLang.Where(x => x.DefaultLanguage == "Wednesday").Select(x => x.EN).FirstOrDefault();
                        SessionManagement.Thursday = defaultLang.Where(x => x.DefaultLanguage == "Thursday").Select(x => x.EN).FirstOrDefault();
                        SessionManagement.Friday = defaultLang.Where(x => x.DefaultLanguage == "Friday").Select(x => x.EN).FirstOrDefault();
                        SessionManagement.Saturday = defaultLang.Where(x => x.DefaultLanguage == "Saturday").Select(x => x.EN).FirstOrDefault();
                        //SessionManagement.Language 
                    }

                }
            }
            catch (Exception)
            {

                throw;
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
            try
            {
                var empData = _facory.GetEmployees(SessionManagement.FactoryID, Common.GetNullableValues(lastname), Common.GetNullableValues(firstname), Common.GetNullableValues(empNumber), Common.GetInteger(manufacture), Common.GetInteger(phoneType), isActive).ToList().Select(x => new
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
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public JsonResult GetMessageHistory(int employeeId, string start, string end)
        {
            try
            {
                var msgHistory = _facory.GetSMSFilter(employeeId, Common.ConvertDateTimeN(start), Common.ConvertDateTimeN(end)).ToList()
                    .Select(x => new { SmsCreatDate = x.SmsCreatDate.ToString("dd/MM/yyyy HH:mm"), x.SmsMsg, x.SmsStatus, x.SmsCount }).ToList();
                return new JsonResult { Data = msgHistory, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult GetEmployeeDiaryTemplate(int employeeId)
        {
            try
            {
                var EmpHours = _facory.GetEmpDiaryTemplate(employeeId).ToList().Select(e => new { Day = ((ISEE.Common.Common.WeekDays)Enum.ToObject(typeof(ISEE.Common.Common.WeekDays), e.DayStatus)).ToString(), Start1 = e.Start1 != null ? Convert.ToDateTime(e.Start1.Value.ToString()).ToShortTimeString() : null, End1 = e.Stop1 != null ? Convert.ToDateTime(e.Stop1.Value.ToString()).ToShortTimeString() : null, Start2 = e.Start2 != null ? Convert.ToDateTime(e.Start2.Value.ToString()).ToShortTimeString() : null, End2 = e.Stop2 != null ? Convert.ToDateTime(e.Stop2.Value.ToString()).ToShortTimeString() : null, DayStatus = e.DayStatus }).ToList();
                object[] EmpHoursData = new object[7];
                for (int i = 0; i < EmpHours.Count; i++)
                {

                    if (EmpHours[i].DayStatus == 1)
                        EmpHoursData[i] = new { Day = SessionManagement.Sunday, Start1 = EmpHours[i].Start1 ?? "", End1 = EmpHours[i].End1 ?? "", Start2 = EmpHours[i].Start2 ?? "", End2 = EmpHours[i].End2 ?? "", DayStatus = EmpHours[i].DayStatus };
                    if (EmpHours[i].DayStatus == 2)
                        EmpHoursData[i] = new { Day = SessionManagement.Monday, Start1 = EmpHours[i].Start1 ?? "", End1 = EmpHours[i].End1 ?? "", Start2 = EmpHours[i].Start2 ?? "", End2 = EmpHours[i].End2 ?? "", DayStatus = EmpHours[i].DayStatus };
                    if (EmpHours[i].DayStatus == 3)
                        EmpHoursData[i] = new { Day = SessionManagement.Tuesday, Start1 = EmpHours[i].Start1 ?? "", End1 = EmpHours[i].End1 ?? "", Start2 = EmpHours[i].Start2 ?? "", End2 = EmpHours[i].End2 ?? "", DayStatus = EmpHours[i].DayStatus };
                    if (EmpHours[i].DayStatus == 4)
                        EmpHoursData[i] = new { Day = SessionManagement.Wednesday, Start1 = EmpHours[i].Start1 ?? "", End1 = EmpHours[i].End1 ?? "", Start2 = EmpHours[i].Start2 ?? "", End2 = EmpHours[i].End2 ?? "", DayStatus = EmpHours[i].DayStatus };
                    if (EmpHours[i].DayStatus == 5)
                        EmpHoursData[i] = new { Day = SessionManagement.Thursday, Start1 = EmpHours[i].Start1 ?? "", End1 = EmpHours[i].End1 ?? "", Start2 = EmpHours[i].Start2 ?? "" , End2 = EmpHours[i].End2 ?? "", DayStatus = EmpHours[i].DayStatus };
                    if (EmpHours[i].DayStatus == 6)
                        EmpHoursData[i] = new { Day = SessionManagement.Friday, Start1 = EmpHours[i].Start1 ?? "", End1 = EmpHours[i].End1 ?? "", Start2 = EmpHours[i].Start2 ?? "", End2 = EmpHours[i].End2 ?? "", DayStatus = EmpHours[i].DayStatus };
                    if (EmpHours[i].DayStatus == 7)
                        EmpHoursData[i] = new { Day = SessionManagement.Saturday, Start1 = EmpHours[i].Start1 ?? "", End1 = EmpHours[i].End1 ?? "", Start2 = EmpHours[i].Start2 ?? "", End2 = EmpHours[i].End2 ?? "", DayStatus = EmpHours[i].DayStatus };
                    //EmpHours[i].Day = SessionManagement.Sunday;
                }
                //     var EmpHour = context.FactoryDairyTemplets.Where(s => s.Factory == factoryId).ToList().Select(e => new { Day = ((Days)Enum.ToObject(typeof(Days), e.DayStatus)).ToString(), Start1 = e.Start1 != null ? Convert.ToDateTime(e.Start1.Value.ToString()).ToShortTimeString() : null, End1 = e.Stop1 != null ? Convert.ToDateTime(e.Stop1.Value.ToString()).ToShortTimeString() : null, Start2 = e.Start2 != null ? Convert.ToDateTime(e.Start2.Value.ToString()).ToShortTimeString() : null, End2 = e.Stop2 != null ? Convert.ToDateTime(e.Stop2.Value.ToString()).ToShortTimeString() : null }).ToList();
                return new JsonResult { Data = EmpHoursData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public JsonResult GetEmployeeTimeHistoryDiary(int employeeId, int month, int year)
        {
            try
            {
                var EmpHours = _facory.GetEmployeeDiaryMonth(employeeId, month, year).ToList().Select(e => new { Day = e.Day.ToString("dd/MM/yyyy"), Start1 = e.Start1 != null ? Convert.ToDateTime(e.Start1.Value.ToString()).ToString("h:mm tt") : null, End1 = e.Stop1 != null ? Convert.ToDateTime(e.Stop1.Value.ToString()).ToString("h:mm tt") : null, Start2 = e.Start2 != null ? Convert.ToDateTime(e.Start2.Value.ToString()).ToString("h:mm tt") : null, End2 = e.Stop2 != null ? Convert.ToDateTime(e.Stop2.Value.ToString()).ToString("h:mm tt") : null, Start3 = e.Start3 != null ? Convert.ToDateTime(e.Start3.Value.ToString()).ToString("h:mm tt") : null, End3 = e.Stop3 != null ? Convert.ToDateTime(e.Stop3.Value.ToString()).ToString("h:mm tt") : null }).ToList();
                return new JsonResult { Data = EmpHours, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string SendMessage(int employeeId, string msg, string phoneNumber)
        {
            string result = string.Empty;

            switch (SessionManagement.SmsProvider)
            {
                case 1:
                    result = SenderSMS(employeeId, msg, phoneNumber);
                    break;
                case 2:
                    result = SenderSMSMexico(employeeId, msg, phoneNumber, SessionManagement.CurrentGmt);
                    break;
                case 3:
                    //  strPhone = vm_emp.SelectedEmployee.MainAreaPhone.TrimStart('0').Trim() + vm_emp.SelectedEmployee.MainPhone.Trim();
                    result = SendSMSClickatell(employeeId, msg, phoneNumber, SessionManagement.PhoneAreaCode, SessionManagement.CurrentGmt);
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
            emp_sms.SmsCreatDate = DateTime.Now.AddHours(SessionManagement.CurrentGmt);
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

            string sss = SessionManagement.PhoneAreaCode;

            //  string _strMsg = "";
            string UserName = "sparta";
            string Password = "5632455";
            string msg = System.Security.SecurityElement.Escape(_strMsg);
            string senderName = "regionSEE";
            string senderNumber = "5632455";

            //set phone numbers "0545500378;0545500379;"
            string phonesList = SessionManagement.PhoneAreaCode + "0505774499";


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
            emp_sms.SmsCreatDate = DateTime.Now.AddHours(SessionManagement.CurrentGmt);
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

                            factoryDairyTemplet.Start1 = Common.GetTimeSpan(item.Start1);
                            factoryDairyTemplet.Stop1 = Common.GetTimeSpan(item.End1);
                            factoryDairyTemplet.Start2 = Common.GetTimeSpan(item.Start2);
                            factoryDairyTemplet.Stop2 = Common.GetTimeSpan(item.End2);
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
                        employee.Mail = mail.Trim();
                        employee.FirstName = firstName.Trim();
                        employee.LastName = lastName.Trim();
                        employee.MainAreaPhone = phone1.Trim();
                        employee.MainPhone = phone11.Trim();
                        employee.SecondAreaPhone = phone2.Trim();
                        employee.SecondPhone = phone22.Trim();
                        employee.PhoneManufactory = manufacture;
                        employee.PhoneType = phoneType;
                        if (!string.IsNullOrEmpty(Start))
                            employee.StartDay = Common.ConvertDateTime(Start);
                        if (!string.IsNullOrEmpty(end))
                            employee.EndDay = Common.ConvertDateTime(end);
                        else
                            employee.EndDay = null;
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
        public int SetViewBagProperty(int empId, int cusId)
        {
            TempData["empId"] = null;
            TempData["cusId"] = null;
            if (empId > 0)
                TempData["empId"] = empId;
            else if (cusId > 0)
                TempData["cusId"] = cusId;
            return empId;
        }


        #region Country Data Common Methods

        public JsonResult GetAllStatesByCountry()
        {
            using (ISEEEntities context = new ISEEEntities())
            {
                var CountryID = SessionManagement.Country;
                var StateDec = _facory.GetAllStates(CountryID).ToList()
                    .Select(x => new { StateCode = x.StateCode.ToString().Trim(), StateDesc = (x.StateDesc ?? string.Empty).ToString().Trim() })
                    .Distinct()
                    .ToList();

                return new JsonResult { Data = StateDec, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
        }
        public JsonResult GetAllCitysByState(int stateID)
        {
            var CountryID = SessionManagement.Country;
            var Cityes = _facory.GetAllCities(CountryID, stateID).ToList()
                .Select(d => new { CityCode = d.CityCode.ToString().Trim(), CityDesc = d.CityDesc.ToString().Trim() })
                .Distinct()
                .ToList();
            return new JsonResult { Data = Cityes, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public JsonResult GetAllStreetByCity(int cityID)
        {
            var CountryID = SessionManagement.Country;

            var Streets = _facory.GetAllStreets(CountryID, cityID).ToList().Select(d => new { StreetCode = d.StreetCode.ToString().Trim(), Streetdesc = d.StreetDesc.ToString().Trim() })
                .Distinct()
                .ToList();
            return new JsonResult { Data = Streets, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public JsonResult GetAllBuildingsByCity(int streetID, int cityID)
        {
            var CountryID = SessionManagement.Country;
            var Buildings = _facory.GetAllNumbers(CountryID, cityID, streetID).ToList().Select(d => new { BuildingCode = d.BuildingCode, BuildingLat = d.Lat, BuldingLong = d.Long, BuildingNumber = d.Number.Trim() })
                .Distinct()
                .ToList();
            return new JsonResult { Data = Buildings, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public JsonResult GetCurrentLogedUserCountery()
        {
            int FactoryId = SessionManagement.FactoryID;
            if (FactoryId > 0)
            {
                using (ISEEEntities context = new ISEEEntities())
                {
                    var CountryDetail = context.FactoryParms.Select(c => new { FactoryId = c.FactoryId, CountryID = c.Country, Lat = c.Lat, Long = c.Long, Zoom = c.Zoom }).Where(x => x.FactoryId == FactoryId).ToList();
                    return new JsonResult { Data = CountryDetail, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public JsonResult insertAddress(int stateID, int cityID, int streetID, string buildingNumber, string entry, string zipCode, string state, string city, string street)
        {
            try
            {
                int countryID = SessionManagement.Country;

                var objBuilding = _facory.GetChangeBuildingCode1(countryID, stateID, cityID, streetID, buildingNumber, entry, zipCode);

                if (objBuilding == null)//not found building code
                {
                    GoogleDomainService objGoogleDomainService = new GoogleDomainService();
                    var objAddress = new Address
                    {
                        CountryId = countryID,
                        Country = SessionManagement.CountryDesc,
                        State = state,
                        City = city,
                        Street = street,
                        Building = buildingNumber
                    };

                    var result = objGoogleDomainService.GetLocation(objAddress);
                    return new JsonResult { Data = new { IsSuccess = true, IsOpenMap = true, Result = result }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }
                else
                {
                    return new JsonResult { Data = new { IsSuccess = true, IsOpenMap = false, BuildingCode = objBuilding.BuildingCode, BuildingLat = objBuilding.Lat, BuildingLong = objBuilding.Long }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }

            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { IsSuccess = false, IsOpenMap = false, ErrorMessage = ex.Message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            //using (ISEEEntities context = new ISEEEntities())
            //{
            //    GoogleDomainService objGoogleDomainService = new GoogleDomainService();

            //    int FactoryId = ISEE.Common.SessionManegment.SessionManagement.FactoryID;
            //    int countryID = ISEE.Common.SessionManegment.SessionManagement.Country;

            //    var a = new Address
            //    {
            //        CountryId = countryID,
            //        Country = ISEE.Common.SessionManegment.SessionManagement.CountryDesc,
            //        State = stateID,
            //        City = cityID,
            //        Street = streetID,
            //        Building = buildingNumber
            //    };

            //    var result = objGoogleDomainService.GetLocation(a);


            //var result = context.Buildings.Where(x => x.CountryCode == countryID
            //    && x.StateCode == (stateID == null ? x.StateCode : stateID)
            //    && x.StreetCode == (streetID == null ? x.StreetCode : streetID)
            //    && x.CityCode == (cityID == null ? x.CityCode : cityID)).Select(s => new { CountryName = s.Street.City.State.Country.CountryDesc, StateName = s.Street.City.State.StateDesc, CityName = s.Street.City.CityDesc, StreetName = s.Street.StreetDesc, BuldingNumber = s.Number, Lat = s.Lat, Long = s.Long, Number = s.Number }).ToList();

        }

        public JsonResult GetAddressBuildingCode(int state, string citydesc, int city, int street, string streetdesc, string number, double Lat, double Long, string entry, string zipcode)
        {
            int country = SessionManagement.Country;
            var buildingCode = _facory.GetAddressBuildingCode(country, state, citydesc, city, street, streetdesc, number, Lat, Long, entry, zipcode);
            return new JsonResult { Data = new { BuildingCode = buildingCode }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        #endregion


        // Country tab start
        public JsonResult GetAllCountrys()
        {
            using (ISEEEntities context = new ISEEEntities())
            {
                var StateDec = _facory.GetAllcountries().ToList()
                .Select(x => new { CountryCode = x.CountryCode.ToString().Trim(), CountryDesc = (x.CountryDesc ?? string.Empty).ToString().Trim(), CurrentGmt = x.CurrentGmt, CountryDescEN = (x.CountryDescEn ?? string.Empty).ToString().Trim() })
                .Distinct()
                .ToList();
                return new JsonResult { Data = StateDec, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        public JsonResult GetAllStatesByCountryID(int countryID)
        {
            var StateDec = _facory.GetAllStates(countryID).ToList()
                    .Select(x => new { StateCode = x.StateCode.ToString().Trim(), StateDesc = (x.StateDesc ?? string.Empty).ToString().Trim(), StateDescEn = (x.StateDescEn ?? string.Empty).ToString().Trim() })
                    .Distinct()
                    .ToList();

            return new JsonResult { Data = StateDec, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetAllCitysByStateAndCountry(int stateID, int countyID)
        {
            var Cityes = _facory.GetAllCities(countyID, stateID).ToList()
            .Select(d => new { CityCode = d.CityCode.ToString().Trim(), CityDesc = d.CityDesc.ToString().Trim(), CityDescEn = (d.CityDescEn ?? string.Empty).ToString().Trim() })
            .Distinct()
            .ToList();
            return new JsonResult { Data = Cityes, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public JsonResult GetAllStreetsByCityByCountry(int cityID, int countyID)
        {
            var Streets = _facory.GetAllStreets(countyID, cityID).ToList().Select(d => new { StreetCode = d.StreetCode.ToString().Trim(), Streetdesc = d.StreetDesc.ToString().Trim(), StreetDescEn = (d.StreetDescEn ?? string.Empty).ToString().Trim() })
            .Distinct()
            .ToList();
            return new JsonResult { Data = Streets, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        //country tab end
        #region Company Tab

        public JsonResult GetAllCompanyDesc()
        {

            var StateDec = _facory.GetAllCompanyDesc().ToList()
            .Select(x => new { FactoryId = x.FactoryId.ToString().Trim(), FactoryDesc = (x.FactoryDesc ?? string.Empty).ToString().Trim() })
            .Distinct()
            .ToList();
            return new JsonResult { Data = StateDec, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public JsonResult GetAllFactoryDataId(int factoryId)
        {

            var StateDec = _facory.GetAllFactoryDataId(factoryId).ToList().Select(x => new { x.FactoryDesc, x.FactoryId, x.UserName, x.Password, x.FactoryParm.Lat, x.FactoryParm.Long, x.FactoryParm.SplitTime, x.FactoryParm.Zoom, x.FactoryParm.StopEmployeeTime, x.FactoryParm.MapProvider, x.FactoryParm.SmsProvider, x.FactoryParm.PhoneAreaCode, x.FactoryParm.CompanyLogo, x.FactoryParm.CustomerLinkDistanceThreshold, x.FactoryParm.RadiusSearch, x.FactoryParm.CurrentGmt, x.FactoryParm.Country }).ToList();
            var EmpHours = _facory.GetFactoryDairyTemp(factoryId).ToList().Select(e => new { Day = ((ISEE.Common.Common.WeekDays)Enum.ToObject(typeof(ISEE.Common.Common.WeekDays), e.DayStatus)).ToString(), Start1 = e.Start1 != null ? Convert.ToDateTime(e.Start1.Value.ToString()).ToShortTimeString() : null, End1 = e.Stop1 != null ? Convert.ToDateTime(e.Stop1.Value.ToString()).ToShortTimeString() : null, Start2 = e.Start2 != null ? Convert.ToDateTime(e.Start2.Value.ToString()).ToShortTimeString() : null, End2 = e.Stop2 != null ? Convert.ToDateTime(e.Stop2.Value.ToString()).ToShortTimeString() : null, DayStatus = e.DayStatus, Order = e.OrderDay }).ToList();
            object[] EmpHoursData = new object[7];
            for (int i = 0; i < EmpHours.Count; i++)
            {

                if (EmpHours[i].DayStatus == 1)
                    EmpHoursData[i] = new { Day = SessionManagement.Sunday, Start1 = EmpHours[i].Start1 ?? "", End1 = EmpHours[i].End1 ?? "", Start2 = EmpHours[i].Start2 ?? "", End2 = EmpHours[i].End2 ?? "", DayStatus = EmpHours[i].DayStatus, Order = EmpHours[i].Order };
                if (EmpHours[i].DayStatus == 2)
                    EmpHoursData[i] = new { Day = SessionManagement.Monday, Start1 = EmpHours[i].Start1 ?? "", End1 = EmpHours[i].End1 ?? "", Start2 = EmpHours[i].Start2 ?? "", End2 = EmpHours[i].End2 ?? "", DayStatus = EmpHours[i].DayStatus, Order = EmpHours[i].Order };
                if (EmpHours[i].DayStatus == 3)
                    EmpHoursData[i] = new { Day = SessionManagement.Tuesday, Start1 = EmpHours[i].Start1 ?? "", End1 = EmpHours[i].End1 ?? "", Start2 = EmpHours[i].Start2 ?? "", End2 = EmpHours[i].End2 ?? "", DayStatus = EmpHours[i].DayStatus, Order = EmpHours[i].Order };
                if (EmpHours[i].DayStatus == 4)
                    EmpHoursData[i] = new { Day = SessionManagement.Wednesday, Start1 = EmpHours[i].Start1 ?? "", End1 = EmpHours[i].End1 ?? "", Start2 = EmpHours[i].Start2 ?? "", End2 = EmpHours[i].End2 ?? "", DayStatus = EmpHours[i].DayStatus, Order = EmpHours[i].Order };
                if (EmpHours[i].DayStatus == 5)
                    EmpHoursData[i] = new { Day = SessionManagement.Thursday, Start1 = EmpHours[i].Start1 ?? "", End1 = EmpHours[i].End1 ?? "", Start2 = EmpHours[i].Start2 ?? "", End2 = EmpHours[i].End2 ?? "", DayStatus = EmpHours[i].DayStatus, Order = EmpHours[i].Order };
                if (EmpHours[i].DayStatus == 6)
                    EmpHoursData[i] = new { Day = SessionManagement.Friday, Start1 = EmpHours[i].Start1 ?? "", End1 = EmpHours[i].End1 ?? "", Start2 = EmpHours[i].Start2 ?? "", End2 = EmpHours[i].End2 ?? "", DayStatus = EmpHours[i].DayStatus, Order = EmpHours[i].Order };
                if (EmpHours[i].DayStatus == 7)
                    EmpHoursData[i] = new { Day = SessionManagement.Saturday, Start1 = EmpHours[i].Start1 ?? "", End1 = EmpHours[i].End1 ?? "", Start2 = EmpHours[i].Start2 ?? "", End2 = EmpHours[i].End2 ?? "", DayStatus = EmpHours[i].DayStatus, Order = EmpHours[i].Order };
                //EmpHours[i].Day = SessionManagement.Sunday;
            }

            //var StateDeca = StateDec.Select(x => new { FactoryId = x.FactoryId.ToString().Trim(), FactoryDesc = (x.FactoryDesc ?? string.Empty).ToString().Trim() })
            //   .Distinct()
            //   .ToList();
            // object bb= new object {StateDc = "", EmpHors = ""};
            //  return new JsonResult { Data =  StateDec, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return Json(new { Data = StateDec, Profiles = EmpHoursData }, JsonRequestBehavior.AllowGet);
        }
        #endregion



    }


}
