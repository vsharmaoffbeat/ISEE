using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using DHTMLX.Scheduler;
using DHTMLX.Common;
using DHTMLX.Scheduler.Data;
using DHTMLX.Scheduler.Controls;

using ISEE.Models;
using ISEEDataModel.Repository;
using ISEE.Common;
namespace ISEE.Controllers
{
    public class CalendarController : Controller
    {
        const string Status_stop = "Stop without a Customer";
        const string Status_1 = "Select Customer";
        const string Status_2 = "Selected Customer";
        const string Status_3 = "Close Customer";
        const string Status_4 = "Far Customer";
        const string StopPoint = "Stop Point";

        static ISEEEntities datacontext = new ISEEEntities();
        #region DXHTML Schduler Methods
        public ActionResult Index()
        {
            //Being initialized in that way, scheduler will use CalendarController.Data as a the datasource and CalendarController.Save to process changes
            var scheduler = new DHXScheduler(this);

            /*
             * It's possible to use different actions of the current controller
             *      var scheduler = new DHXScheduler(this);     
             *      scheduler.DataAction = "ActionName1";
             *      scheduler.SaveAction = "ActionName2";
             * 
             * Or to specify full paths
             *      var scheduler = new DHXScheduler();
             *      scheduler.DataAction = Url.Action("Data", "Calendar");
             *      scheduler.SaveAction = Url.Action("Save", "Calendar");
             */

            /*
             * The default codebase folder is ~/Scripts/dhtmlxScheduler. It can be overriden:
             *      scheduler.Codebase = Url.Content("~/customCodebaseFolder");
             */


            //scheduler.InitialDate = new DateTime(2012, 09, 03);

            //scheduler.LoadData = true;
            //scheduler.EnableDataprocessor = true;

            return View();
        }

        public ContentResult Data(int ID, DateTime from, DateTime to)
        {
            List<CalendarEvent> listCalendarEvent = new List<CalendarEvent>();
            // int ID = 2;
            DateTime dt1 = from;
            DateTime dt2 = to;
            TimeSpan from1 = from.TimeOfDay;
            TimeSpan to1 = to.TimeOfDay;
            var employeeStopPoints = GetStopPoints(SessionManagement.FactoryID, ID, dt1, dt2, from1, to1);

            var employeeCustomerPoints = GetGpsEmployeeCustomerPoints(ID, dt1, dt2, from1, to1);


            bool blnFlag;

            foreach (EmployeeGpsPoint p in employeeStopPoints.ToList())
            {
                blnFlag = false;
                if (p.StopStartTime != null && p.StopTime != null)
                {
                    TimeSpan timestop = (TimeSpan)p.StopTime;
                    CalendarEvent stop = new CalendarEvent { id = p.EmployeeId ?? 0, text = Status_stop, uniqueid = p.SysId.ToString(), start_date = (DateTime)p.StopStartTime, end_date = Convert.ToDateTime(p.StopStartTime).AddMinutes(timestop.TotalMinutes), color = Category.Red.ToString() };

                    foreach (CalendarEvent p1 in listCalendarEvent)
                        if (p1.uniqueid == stop.uniqueid)
                            blnFlag = true;

                    if (!blnFlag)
                        listCalendarEvent.Add(stop);
                }
            }

            string strSubject = "";
            string cur_category = "";
            foreach (GpsEmployeeCustomer p in employeeCustomerPoints.ToList())
            {
                strSubject = p.Customer.LastName + " - " + p.Customer.CustomerNumber;
                blnFlag = false;
                if (p.EmployeeGpsPoint != null)
                {
                    TimeSpan timestop = (TimeSpan)p.EmployeeGpsPoint.StopTime;

                    if (p.EmployeeGpsPoint != null && p.EmployeeGpsPoint.StopStartTime != null && p.EmployeeGpsPoint.StopTime != null)
                    {

                        switch (p.InsertStatus)
                        {
                            case 0:
                                cur_category = Category.Green.ToString();
                                break;
                            case 1:
                                cur_category = Category.Blue.ToString();
                                break;
                            case 2:
                                cur_category = Category.Custom.ToString(); //System.Drawing.Color.FromArgb(0xFF, 0x46, 0x82, 0xB4).ToString(); //TODO: to get hexadecimal code of color
                                break;
                            case 3:
                                cur_category = Category.Yellow.ToString();
                                break;
                            case 4:
                                cur_category = Category.Orange.ToString();
                                break;
                            default:
                                cur_category = null;
                                break;
                        }
                        if (cur_category != null)
                        {
                            if (p.InsertStatus == 1)
                            {
                                CalendarEvent app1 = listCalendarEvent.Where(x => x.uniqueid.CompareTo(Convert.ToString(p.GpsPointId)) == 0).FirstOrDefault();
                                if (app1 == null)
                                {
                                    CalendarEvent s = new CalendarEvent { id = p.EmployeeId, customerId = p.CustomerId.ToString(), text = Status_1, uniqueid = Convert.ToString(p.GpsPointId), start_date = (DateTime)p.EmployeeGpsPoint.StopStartTime, end_date = Convert.ToDateTime(p.EmployeeGpsPoint.StopStartTime).AddMinutes(timestop.TotalMinutes), color = cur_category };
                                    foreach (CalendarEvent p1 in listCalendarEvent)
                                        if (p1.uniqueid == s.uniqueid)
                                            blnFlag = true;

                                    if (!blnFlag)
                                        listCalendarEvent.Add(s);
                                }

                            }

                            else
                            {
                                CalendarEvent s = new CalendarEvent { id = p.EmployeeId, customerId = p.CustomerId.ToString(), text = strSubject, uniqueid = Convert.ToString(p.GpsPointId), start_date = (DateTime)p.EmployeeGpsPoint.StopStartTime, end_date = Convert.ToDateTime(p.EmployeeGpsPoint.StopStartTime).AddMinutes(timestop.TotalMinutes), color = cur_category };
                                foreach (CalendarEvent p1 in listCalendarEvent)
                                    if (p1.uniqueid == s.uniqueid)
                                        blnFlag = true;

                                if (!blnFlag)
                                    listCalendarEvent.Add(s);
                            }
                        }
                    }
                }
            }


            listCalendarEvent.Add(new CalendarEvent() { id = 1, text = "test", start_date = DateTime.Now, end_date = DateTime.Now.AddMinutes(20), color = "red" });
            listCalendarEvent.Add(new CalendarEvent() { id = 2, text = "test 2", start_date = DateTime.Now.AddMinutes(30), end_date = DateTime.Now.AddMinutes(90), color = "yellow" });


            var data = new SchedulerAjaxData(listCalendarEvent);

            return (ContentResult)data;
        }

        public ContentResult Save(int? id, FormCollection actionValues)
        {
            var action = new DataAction(actionValues);

            try
            {
                var changedEvent = (CalendarEvent)DHXEventsHelper.Bind(typeof(CalendarEvent), actionValues);



                switch (action.Type)
                {
                    case DataActionTypes.Insert:
                        //do insert
                        //action.TargetId = changedEvent.id;//assign postoperational id
                        break;
                    case DataActionTypes.Delete:
                        //do delete
                        break;
                    default:// "update"                          
                        //do update
                        break;
                }
            }
            catch
            {
                action.Type = DataActionTypes.Error;
            }
            return (ContentResult)new AjaxSaveResponse(action);
        }
        #endregion


        #region "SchedulerEmployees"

        /// <summary>
        ///  To get Employee Appointment where Customer not schduled
        /// </summary>
        /// <param name="factoryId">Factory ID</param>
        /// <param name="ID">Employee ID</param>
        /// <param name="dt1">Start Date</param>
        /// <param name="dt2">End Date</param>
        /// <param name="from_time">Start Time</param>
        /// <param name="to_time">End Time</param>
        /// <returns></returns>
        public IQueryable<EmployeeGpsPoint> GetStopPoints(int factoryId, int ID, DateTime dt1, DateTime dt2, TimeSpan from_time, TimeSpan to_time)
        {

            var factory = datacontext.FactoryParms.FirstOrDefault(x => x.FactoryId == factoryId);

            if (factory != null)
            {
                if (factory.StopEmployeeTime != null)
                {
                    var stopTime = (int)factory.StopEmployeeTime;

                    var query = from c in datacontext.EmployeeGpsPoints
                                where !(from o in datacontext.GpsEmployeeCustomers
                                        where o.EmployeeId == ID &&
                                              (o.EmployeeGpsPoint.GpsDate >= dt1 &&
                                               o.EmployeeGpsPoint.GpsDate <= dt2) &&
                                              (o.EmployeeGpsPoint.GpsTime >= from_time &&
                                               o.EmployeeGpsPoint.GpsTime <= to_time)
                                        select o.GpsPointId).Contains((int)c.SysId) && (c.GpsDate >= dt1 && c.GpsDate <= dt2) && (c.GpsTime >= from_time && c.GpsTime <= to_time) && c.EmployeeId == ID && (c.PointStatus == 2 || c.PointStatus == 3)
                                select c;



                    var q1 = from x in query.ToList()
                             where x.StopTime.HasValue && x.StopTime.Value.TotalMinutes >= stopTime
                             select x;


                    return q1.AsQueryable();
                }
            }
            return null;
        }

        /// <summary>
        /// Get Employee Appointments where customer assigned
        /// </summary>
        /// <param name="ID">Employee Id</param>
        /// <param name="dt1">Start Date</param>
        /// <param name="dt2">End Date</param>
        /// <param name="from1">Start Time</param>
        /// <param name="to1">End Time</param>
        /// <returns></returns>
        public IQueryable<GpsEmployeeCustomer> GetGpsEmployeeCustomerPoints(int ID, DateTime dt1, DateTime dt2, TimeSpan from1, TimeSpan to1)
        {
            return datacontext.GpsEmployeeCustomers.Include("EmployeeGpsPoint").Include("Customer").Include("Customer.Building").Include("Customer.Building.Street").Include("Customer.Building.Street.City").Where(c => c.EmployeeId == ID && (c.EmployeeGpsPoint.GpsDate >= dt1 && c.EmployeeGpsPoint.GpsDate <= dt2) && (c.EmployeeGpsPoint.GpsTime >= from1 && c.EmployeeGpsPoint.GpsTime <= to1));
        }

        #endregion
    }

    public enum Category
    {
        Green,
        Blue,
        Custom,
        Yellow,
        Orange,
        Red
    }
}

