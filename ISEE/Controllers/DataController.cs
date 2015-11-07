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
                                CalenderShowZoomLevel = fd.CalenderShowZoomLevel
                            };





                var loginData = query.ToList()[0];
                SessionManegment.SessionManagement.FactoryID = loginData.FactoryID;

                SessionManegment.SessionManagement.FactoryDesc = loginData.FactoryDesc;
                SessionManegment.SessionManagement.CountryDesc = loginData.CountryDesc;
                SessionManegment.SessionManagement.FactoryID = loginData.FactoryID;
                SessionManegment.SessionManagement.Country = loginData.Country;
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
        public JsonResult GetEmployee(string manufacture, string lastname, string firstname, string empNumber, string phoneType)
        {
            firstname = string.IsNullOrEmpty(firstname) ? null : firstname;
            lastname = string.IsNullOrEmpty(lastname) ? null : lastname;
            empNumber = string.IsNullOrEmpty(empNumber) ? null : empNumber;

            using (ISEEEntities context = new ISEEEntities())
            {
                int factoryId = ISEE.Common.SessionManegment.SessionManagement.FactoryID = 1;
                bool _Active = true;
                var empData = context.Employees.ToList().Where(x => x.Factory == factoryId
                                                         && (string.IsNullOrEmpty(firstname) || x.FirstName.Contains(firstname))
                    //       x.FirstName.Contains(firstname == null ? x.FirstName : firstname)
                                                         && (string.IsNullOrEmpty(lastname) || x.LastName.Contains(lastname))
                                                           && (string.IsNullOrEmpty(empNumber) || x.EmployeeNum.Contains(empNumber))
                                                         && x.PhoneManufactory == (Common.GetInteger(manufacture) == 0 ? x.PhoneManufactory : Common.GetInteger(manufacture))
                                                                  && x.PhoneType == (Common.GetInteger(phoneType) == 0 ? x.PhoneType : Common.GetInteger(phoneType))
                                                                  && (_Active == true ? (x.EndDay == null || (x.EndDay != null && x.EndDay >= DateTime.Now)) : (x.EndDay != null && x.EndDay < DateTime.Now))).OrderBy(x => x.EmployeeNum).Select(x => new
                                                                  {
                                                                      EmployeeId = x.EmployeeId,
                                                                      EmployeeNum = x.EmployeeNum,
                                                                      Mail = x.Mail == null ? "" : x.Mail,
                                                                      FirstName = x.FirstName == null ? "" : x.FirstName,
                                                                      LastName = x.LastName == null ? "" : x.LastName,
                                                                      StartDay = x.StartDay,
                                                                      MainAreaPhone = x.MainAreaPhone == null ? "" : x.MainAreaPhone,
                                                                      MainPhone = x.MainPhone == null ? "" : x.MainPhone,
                                                                      SecondAreaPhone = x.SecondAreaPhone == null ? "" : x.SecondAreaPhone,
                                                                      SecondPhone = x.SecondPhone == null ? "" : x.SecondPhone,
                                                                      LastSendApp = x.LastSendApp,
                                                                      EndDay = x.EndDay
                                                                  }).ToList();


                 return new JsonResult { Data = empData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public JsonResult GetMessageHistory(int employeeId, string start, string end)
        {
            using (ISEEEntities context = new ISEEEntities())
            {
            var msgHistory=    context.EmployeeSmsSends.ToList().Where(x => x.EmployeeId == employeeId && x.SmsCreatDate >= Convert.ToDateTime(start) 
                    && x.SmsCreatDate <= Convert.ToDateTime(end))
                    .Select(x => new {x.SmsCreatDate, x.SmsMsg,x.SmsStatus,x.SmsCount }).ToList();

            return new JsonResult { Data = msgHistory, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public JsonResult GetEmployeeDiaryTemplate(int employeeId)
        {
            
            using (ISEEEntities context = new ISEEEntities())
            {
                var EmpHours = context.EmployeeDiaryTemplates.Where(s => s.EmployeeId== employeeId).ToList().Select(e => new { Day = ((ISEE.Controllers.AdminController.Days)Enum.ToObject(typeof(ISEE.Controllers.AdminController.Days), e.DayStatus)).ToString(), Start1 = e.Start1 != null ? Convert.ToDateTime(e.Start1.Value.ToString()).ToShortTimeString() : null, End1 = e.Stop1 != null ? Convert.ToDateTime(e.Stop1.Value.ToString()).ToShortTimeString() : null, Start2 = e.Start2 != null ? Convert.ToDateTime(e.Start2.Value.ToString()).ToShortTimeString() : null, End2 = e.Stop2 != null ? Convert.ToDateTime(e.Stop2.Value.ToString()).ToShortTimeString() : null }).ToList();
                return new JsonResult { Data = EmpHours, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        public JsonResult getEmployeeTimeHistoryDiary(int employeeId, int month, int year)
        {
            DateTime start = new DateTime();
            using (ISEEEntities context = new ISEEEntities())
            {
                var EmpHours = context.EmployeeDiaryTemplates.Where(s => s.EmployeeId== employeeId).ToList().Select(e => new { Day = ((ISEE.Controllers.AdminController.Days)Enum.ToObject(typeof(ISEE.Controllers.AdminController.Days), e.DayStatus)).ToString(), Start1 = e.Start1 != null ? Convert.ToDateTime(e.Start1.Value.ToString()).ToShortTimeString() : null, End1 = e.Stop1 != null ? Convert.ToDateTime(e.Stop1.Value.ToString()).ToShortTimeString() : null, Start2 = e.Start2 != null ? Convert.ToDateTime(e.Start2.Value.ToString()).ToShortTimeString() : null, End2 = e.Stop2 != null ? Convert.ToDateTime(e.Stop2.Value.ToString()).ToShortTimeString() : null }).ToList();
                return new JsonResult { Data = EmpHours, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }


        public void SendSms()
        {
            string _strMsg = "";
            string _phone = "";
            string UserName = "sparta";
            string Password = "5632455";
            string msg = System.Security.SecurityElement.Escape(_strMsg);
            string senderName = "regionSEE";
            string senderNumber = "5632455";

            //set phone numbers "0545500378;0545500379;"
            string phonesList = _phone;

            var empID = 3;

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
            double CurrentGmt = 0.0;
            EmployeeSmsSend emp_sms = new EmployeeSmsSend();
            emp_sms.EmployeeId = empID;
            emp_sms.SmsCreatDate = DateTime.Now.AddHours(CurrentGmt);
            emp_sms.SmsMsg = _strMsg;
            emp_sms.SmsCount = 1;
            emp_sms.SmsStatus = Convert.ToInt32(Status);

            //this.ObjectContext.EmployeeSmsSend.AddObject(emp_sms);
            //this.ObjectContext.SaveChanges();

           // return result;

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

        #endregion
    }
}
