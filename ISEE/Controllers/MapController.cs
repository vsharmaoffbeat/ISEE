using ISEEDataModel.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ISEE.Controllers
{
    public class MapController : Controller
    {
        //
        // GET: /Map/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Map()
        {
            return View();
        }

        public JsonResult GetEmployeeForMap(string firstName, string LastName, int Active, string Number)
        {
            using (ISEEEntities context = new ISEEEntities())
            {
                // int FactoryId = ISEE.Common.SessionManegment.SessionManagement.FactoryID;
                //int FactoryId = 2;
                var result = context.Employees.Where(x =>
                     x.FirstName.Contains(firstName != "" ? firstName : x.FirstName)
                     && x.LastName.Contains(LastName != "" ? LastName : x.LastName)
                     && x.Status == Active
                     && x.EmployeeNum == (Number != "" ? Number : x.EmployeeNum)
                    ).Select(s => new { EmployeeID = s.EmployeeId, FirstName = s.FirstName, LastName = s.LastName, Number = s.EmployeeNum }).ToList();
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

        }
        public JsonResult GetEmployeeGpsPointsByEmployeeID(int EmployeeID, string FromTime, string EndTime)
        {
            if (EmployeeID > 0)
            {
                var formats = new[] { "%h", "h\\.m" };
                TimeSpan fromTime = TimeSpan.ParseExact(FromTime, formats, CultureInfo.InvariantCulture);
                TimeSpan endTime = TimeSpan.ParseExact(EndTime, formats, CultureInfo.InvariantCulture);
                CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
                CultureInfo ci = new CultureInfo("fr-CA");
                Thread.CurrentThread.CurrentCulture = ci;
                DateTime date = DateTime.Now.AddYears(-4);
                DateTime date1 = date.AddMonths(-4).Date;
                using (ISEEEntities context = new ISEEEntities())
                {
                    var result = context.EmployeeGpsPoints.Where(s => s.EmployeeId == EmployeeID
                         && s.GpsDate == date1
                         && s.GpsTime >= fromTime
                         && s.GpsTime <= endTime
                    ).Select(s => new { Lat = s.Lat, Long = s.Long }).ToList();
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public JsonResult GetStopPointsForEmployee(int EmployeeID, string FromTime, string EndTime)
        {
            if (EmployeeID > 0)
            {
                var formats = new[] { "%h", "h\\.m" };
                TimeSpan fromTime = TimeSpan.ParseExact(FromTime, formats, CultureInfo.InvariantCulture);
                TimeSpan endTime = TimeSpan.ParseExact(EndTime, formats, CultureInfo.InvariantCulture);
                CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
                CultureInfo ci = new CultureInfo("fr-CA");
                Thread.CurrentThread.CurrentCulture = ci;
                DateTime date = DateTime.Now.AddYears(-4);
                DateTime date1 = date.AddMonths(-4).Date;
                using (ISEEEntities context = new ISEEEntities())
                {
                    var result = context.EmployeeGpsPoints.Where(s => s.EmployeeId == EmployeeID
                         && s.GpsDate == date1
                         && s.GpsTime >= fromTime
                         && s.GpsTime <= endTime
                         && s.StopTime != null
                    ).Select(s => new { Lat = s.Lat, Long = s.Long }).ToList();
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetLastPointForEmployee(int EmployeeID, string FromTime, string EndTime)
        {
            if (EmployeeID > 0)
            {
                var formats = new[] { "%h", "h\\.m" };
                TimeSpan fromTime = TimeSpan.ParseExact(FromTime, formats, CultureInfo.InvariantCulture);
                TimeSpan endTime = TimeSpan.ParseExact(EndTime, formats, CultureInfo.InvariantCulture);
                CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
                CultureInfo ci = new CultureInfo("fr-CA");
                Thread.CurrentThread.CurrentCulture = ci;
                DateTime date = DateTime.Now.AddYears(-4);
                DateTime date1 = date.AddMonths(-4).Date;
                using (ISEEEntities context = new ISEEEntities())
                {
                    var result = context.EmployeeGpsPoints.Where(s => s.EmployeeId == EmployeeID
                         && s.GpsDate == date1
                         && s.GpsTime >= fromTime
                         && s.GpsTime <= endTime
                     ).OrderBy(x => x.GpsDate).Select(s => new { Lat = s.Lat, Long = s.Long }).FirstOrDefault();
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

    }
}