using ISEE.Common;
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

            firstName = string.IsNullOrEmpty(firstName) ? null : firstName;
            LastName = string.IsNullOrEmpty(LastName) ? null : LastName;
            Number = string.IsNullOrEmpty(Number) ? null : Number;

            using (ISEEEntities context = new ISEEEntities())
            {
                int factoryId = SessionManagement.FactoryID;
                var empData = context.Employees.ToList().Where(x => x.Factory == factoryId
                                                       && (string.IsNullOrEmpty(firstName) || x.FirstName.Contains(firstName))
                                                       && (string.IsNullOrEmpty(LastName) || x.LastName.Contains(LastName))
                                                       && (string.IsNullOrEmpty(Number) || x.EmployeeNum.Contains(Number))
                                                       && (Active == 0 ? (x.EndDay == null || (x.EndDay != null && x.EndDay >= DateTime.Now)) : (x.EndDay != null && x.EndDay < DateTime.Now))).ToList().OrderBy(x => x.EmployeeNum).Select(x => new
                                                                 {
                                                                     EmployeeID = x.EmployeeId,
                                                                     Number = x.EmployeeNum,
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
        public JsonResult GetEmployeeGpsPointsByEmployeeID(int EmployeeID, string FromTime, string EndTime, DateTime Date)
        {
            if (EmployeeID > 0)
            {
                var formats = new[] { "%h", "h\\.m" };
                TimeSpan FromTimeGPS = new TimeSpan(0, 0, 0);
                if (FromTime != null) FromTimeGPS = TimeSpan.ParseExact(FromTime, formats, CultureInfo.InvariantCulture);

                TimeSpan ToTimeGPS = new TimeSpan(23, 59, 0);
                if (EndTime != null) ToTimeGPS = TimeSpan.ParseExact(EndTime, formats, CultureInfo.InvariantCulture);

                CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
                CultureInfo ci = new CultureInfo("fr-CA");
                Thread.CurrentThread.CurrentCulture = ci;

                using (ISEEEntities context = new ISEEEntities())
                {
                    var result = context.EmployeeGpsPoints.Where(s => s.EmployeeId == EmployeeID
                         && s.GpsDate == Date
                         && s.GpsTime >= FromTimeGPS
                         && s.GpsTime <= ToTimeGPS
                    ).Select(s => new { Lat = s.Lat, Long = s.Long }).ToList();
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public JsonResult GetStopPointsForEmployee(int EmployeeID, string FromTime, string EndTime, DateTime Date)
        {
            if (EmployeeID > 0)
            {
                var formats = new[] { "%h", "h\\.m" };
                TimeSpan FromTimeGPS = new TimeSpan(0, 0, 0);
                if (FromTime != null) FromTimeGPS = TimeSpan.ParseExact(FromTime, formats, CultureInfo.InvariantCulture);

                TimeSpan ToTimeGPS = new TimeSpan(23, 59, 0);
                if (EndTime != null) ToTimeGPS = TimeSpan.ParseExact(EndTime, formats, CultureInfo.InvariantCulture);

                CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
                CultureInfo ci = new CultureInfo("fr-CA");
                Thread.CurrentThread.CurrentCulture = ci;

                using (ISEEEntities context = new ISEEEntities())
                {
                    var result = context.EmployeeGpsPoints.Where(s => s.EmployeeId == EmployeeID
                         && s.GpsDate == Date
                         && s.GpsTime >= FromTimeGPS
                         && s.GpsTime <= ToTimeGPS
                         && s.StopTime != null
                    ).Select(s => new { Lat = s.Lat, Long = s.Long, GpsTime = s.GpsTime, StopTime = s.StopTime, LastName = s.Employee.LastName }).ToList();
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetLastPointForEmployee(int EmployeeID, string FromTime, string EndTime, DateTime Date)
        {
            if (EmployeeID > 0)
            {
                var formats = new[] { "%h", "h\\.m" };
                TimeSpan FromTimeGPS = new TimeSpan(0, 0, 0);
                if (FromTime != null) FromTimeGPS = TimeSpan.ParseExact(FromTime, formats, CultureInfo.InvariantCulture);

                TimeSpan ToTimeGPS = new TimeSpan(23, 59, 0);
                if (EndTime != null) ToTimeGPS = TimeSpan.ParseExact(EndTime, formats, CultureInfo.InvariantCulture);

                CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
                CultureInfo ci = new CultureInfo("fr-CA");
                Thread.CurrentThread.CurrentCulture = ci;

                using (ISEEEntities context = new ISEEEntities())
                {
                    var result = context.EmployeeGpsPoints.Where(s => s.EmployeeId == EmployeeID
                         && s.GpsDate == Date
                         && s.GpsTime >= FromTimeGPS
                         && s.GpsTime <= ToTimeGPS
                         && s.StopTime != null
                     ).OrderByDescending(x => x.SysId).Select(s => new { Lat = s.Lat, Long = s.Long, GpsTime = s.GpsTime, StopTime = s.StopTime, LastName = s.Employee.LastName }).FirstOrDefault();
                    Thread.CurrentThread.CurrentCulture = currentCulture;

                    if (result == null)
                    {
                        return new JsonResult { Data = false, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetCustomersForMap(int state, int city, int street, string BuildingNumber, string customerNumber, string companyName)
        {

            BuildingNumber = GetNullableValues(BuildingNumber);
            customerNumber = GetNullableValues(customerNumber);
            using (ISEEEntities context = new ISEEEntities())
            {
                var results = context.Customers.Include("Building").Include("Building.Street").Include("Building.Street.City").Include("Building.Street.City.State").Where(x => x.Factory == SessionManagement.FactoryID &&
                    (state != 0 ? x.Building.StateCode == state : x.Building.StateCode == null) &&
                     x.Building.StateCode == (state == 0 ? x.Building.StateCode : state) &&
                     x.Building.CityCode == (city == 0 ? x.Building.CityCode : city) &&
                     x.Building.StreetCode == (street == 0 ? x.Building.StreetCode : street) &&
                     x.Building.Number.Contains(BuildingNumber == null ? x.Building.Number : BuildingNumber) &&
                     x.CustomerNumber.CompareTo(customerNumber == null ? x.CustomerNumber : customerNumber) == 0

                ).ToList().Select(x => new
                {
                    CustomerId = x.CustomerId,
                    CustomerNumber = x.CustomerNumber,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Floor = x.Floor ?? "!@#$",
                    Apartment = x.Apartment ?? "!@#$",
                    AreaPhone1 = x.AreaPhone1 ?? "!@#$",
                    Phone1 = x.Phone1 ?? "!@#$",
                    AreaPhone2 = x.AreaPhone2 ?? "!@#$",
                    Phone2 = x.Phone2 ?? "!@#$",
                    AreaFax = x.AreaFax ?? "!@#$",
                    Fax = x.Fax ?? "!@#$",
                    Mail = x.Mail ?? "!@#$",
                    CustomerRemark1 = x.CustomerRemark1 ?? "!@#$",
                    CustomerRemark2 = x.CustomerRemark2 ?? "!@#$",
                    VisitInterval = x.VisitInterval ?? 0,
                    NextVisit = x.NextVisit,
                    VisitDate = x.VisitDate,
                    VisitTime = x.VisitTime,
                    EndDate = x.EndDate,
                    Lat = x.Building.Lat,
                    BuildingCode = x.BuildingCode,
                    BuildingNumber = x.Building.Number ?? "!@#$",
                    Long = x.Building.Long,
                    ZipCode = x.Building.ZipCode,
                    StreetName = x.Building.Street.StreetDesc ?? "!@#$",
                    StreetId = x.Building.Street.StateCode,
                    CityId = x.Building.Street.City.CityCode,
                    CityName = x.Building.Street.City.CityDesc ?? "!@#$",
                    StateName = x.Building.Street.City.State.StateDesc ?? "!@#$",
                    StateId = x.Building.Street.City.State.StateCode
                }).ToList();
                return new JsonResult { Data = results, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        public JsonResult GetCustomerForMapByCustomerID(string CheckedCustomers)
        {
            var numbers = CheckedCustomers.TrimEnd(',').Split(',').Select(Int32.Parse).ToList();
            using (ISEEEntities context = new ISEEEntities())
            {
                var result = context.Customers.Where(s => numbers.Contains(s.CustomerId)).Select(p => new { Lat = p.Building.Lat, Long = p.Building.Long, BuildingCode = p.BuildingCode, FirstName = p.FirstName, LastName = p.LastName }).ToList();
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        public string GetNullableValues(string value)
        {
            if (string.IsNullOrEmpty(value.Trim()))
                return null;
            return value;

        }
        public JsonResult GetAllCustomers()
        {
            using (ISEEEntities context = new ISEEEntities())
            {

                var result = context.Customers.Where(s => s.Factory == SessionManagement.FactoryID).Select(x => new
                {
                    CustomerId = x.CustomerId,
                    CustomerNumber = x.CustomerNumber,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Floor = x.Floor ?? "!@#$",
                    Apartment = x.Apartment ?? "!@#$",
                    AreaPhone1 = x.AreaPhone1 ?? "!@#$",
                    Phone1 = x.Phone1 ?? "!@#$",
                    AreaPhone2 = x.AreaPhone2 ?? "!@#$",
                    Phone2 = x.Phone2 ?? "!@#$",
                    AreaFax = x.AreaFax ?? "!@#$",
                    Fax = x.Fax ?? "!@#$",
                    Mail = x.Mail ?? "!@#$",
                    CustomerRemark1 = x.CustomerRemark1 ?? "!@#$",
                    CustomerRemark2 = x.CustomerRemark2 ?? "!@#$",
                    VisitInterval = x.VisitInterval ?? 0,
                    NextVisit = x.NextVisit,
                    VisitDate = x.VisitDate,
                    VisitTime = x.VisitTime,
                    EndDate = x.EndDate,
                    Lat = x.Building.Lat,
                    BuildingCode = x.BuildingCode,
                    BuildingNumber = x.Building.Number ?? "!@#$",
                    Long = x.Building.Long,
                    ZipCode = x.Building.ZipCode,
                    StreetName = x.Building.Street.StreetDesc ?? "!@#$",
                    StreetId = x.Building.Street.StateCode,
                    CityId = x.Building.Street.City.CityCode,
                    CityName = x.Building.Street.City.CityDesc ?? "!@#$",
                    StateName = x.Building.Street.City.State.StateDesc ?? "!@#$",
                    StateId = x.Building.Street.City.State.StateCode
                }).ToList(); ;
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }



    }
}