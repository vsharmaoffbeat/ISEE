using ISEE.Common;
using ISEEDataModel.Repository;
using ISEEDataModel.Repository.Services;
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

        ISEEFactory _facory = new ISEEFactory();
        //
        // GET: /Map/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Map()
        {
            if (TempData["empId"] != null)
            {
                ViewBag.Empoyeeid = TempData["empId"];
                ViewBag.CustomerID = null;
                TempData["empId"] = null;
            }
            else if (TempData["cusId"] != null)
            {
                ViewBag.Empoyeeid = null;
                ViewBag.CustomerID = 1;
                TempData["cusId"] = null;
            }
            return View();
        }

        public JsonResult GetEmployeeForMap(string firstname, string lastname, int active, string number)
        {

            firstname = string.IsNullOrEmpty(firstname) ? null : firstname;
            lastname = string.IsNullOrEmpty(lastname) ? null : lastname;
            number = string.IsNullOrEmpty(number) ? null : number;

            using (ISEEEntities context = new ISEEEntities())
            {
                int factoryId = SessionManagement.FactoryID;
                var empData = context.Employees.ToList().Where(x => x.Factory == factoryId
                                                       && (string.IsNullOrEmpty(firstname) || x.FirstName.Contains(firstname))
                                                       && (string.IsNullOrEmpty(lastname) || x.LastName.Contains(lastname))
                                                       && (string.IsNullOrEmpty(number) || x.EmployeeNum.Contains(number))
                                                       && (active == 0 ? (x.EndDay == null || (x.EndDay != null && x.EndDay >= DateTime.Now)) : (x.EndDay != null && x.EndDay < DateTime.Now))).ToList().OrderBy(x => x.EmployeeNum).Select(x => new
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
        public JsonResult GetEmployeeGpsPointsByEmployeeID(int employeeID, string fromtime, string endtime, DateTime date)
        {
            if (employeeID > 0)
            {
                var formats = new[] { "%h", "h\\.m" };
                TimeSpan FromTimeGPS = new TimeSpan(0, 0, 0);
                if (fromtime != null) FromTimeGPS = TimeSpan.ParseExact(fromtime, formats, CultureInfo.InvariantCulture);

                TimeSpan ToTimeGPS = new TimeSpan(23, 59, 0);
                if (endtime != null) ToTimeGPS = TimeSpan.ParseExact(endtime, formats, CultureInfo.InvariantCulture);

                CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
                CultureInfo ci = new CultureInfo("fr-CA");
                Thread.CurrentThread.CurrentCulture = ci;

                using (ISEEEntities context = new ISEEEntities())
                {
                    var result = context.EmployeeGpsPoints.Where(s => s.EmployeeId == employeeID
                         && s.GpsDate == date
                         && s.GpsTime >= FromTimeGPS
                         && s.GpsTime <= ToTimeGPS
                    ).Select(s => new { Lat = s.Lat, Long = s.Long }).ToList();
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public JsonResult GetStopPointsForEmployee(int employeeID, string fromtime, string endtime, DateTime date)
        {
            if (employeeID > 0)
            {
                var formats = new[] { "%h", "h\\.m" };
                TimeSpan FromTimeGPS = new TimeSpan(0, 0, 0);
                if (fromtime != null) FromTimeGPS = TimeSpan.ParseExact(fromtime, formats, CultureInfo.InvariantCulture);

                TimeSpan ToTimeGPS = new TimeSpan(23, 59, 0);
                if (endtime != null) ToTimeGPS = TimeSpan.ParseExact(endtime, formats, CultureInfo.InvariantCulture);

                CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
                CultureInfo ci = new CultureInfo("fr-CA");
                Thread.CurrentThread.CurrentCulture = ci;

                using (ISEEEntities context = new ISEEEntities())
                {
                    var result = context.EmployeeGpsPoints.Where(s => s.EmployeeId == employeeID
                         && s.GpsDate == date
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

        public JsonResult GetLastPointForEmployee(int employeeID, string fromtime, string endtime, DateTime date)
        {
            if (employeeID > 0)
            {
                var formats = new[] { "%h", "h\\.m" };
                TimeSpan FromTimeGPS = new TimeSpan(0, 0, 0);
                if (fromtime != null) FromTimeGPS = TimeSpan.ParseExact(fromtime, formats, CultureInfo.InvariantCulture);

                TimeSpan ToTimeGPS = new TimeSpan(23, 59, 0);
                if (endtime != null) ToTimeGPS = TimeSpan.ParseExact(endtime, formats, CultureInfo.InvariantCulture);

                CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
                CultureInfo ci = new CultureInfo("fr-CA");
                Thread.CurrentThread.CurrentCulture = ci;

                using (ISEEEntities context = new ISEEEntities())
                {
                    var result = context.EmployeeGpsPoints.Where(s => s.EmployeeId == employeeID
                         && s.GpsDate == date
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

        public JsonResult GetCustomersForMap(int state, int city, int street, string buildingNumber, string companyName, string customerNumber)
        {
            int factoryId = SessionManagement.FactoryID;
            var custData = _facory.GetCustomersNew(factoryId, state, city, street, buildingNumber, customerNumber, null, companyName, null, null, true).Select(c => new
            {
                FirstName = c.FirstName ?? string.Empty,
                CustomerId = c.CustomerId,
                LastName = c.LastName ?? string.Empty,
                AreaPhone1 = c.AreaPhone1 ?? string.Empty,
                Phone1 = c.Phone1 ?? string.Empty,
                CityName = c.Building.Street.City.CityDesc ?? string.Empty,
                StateName = c.Building.Street.City.State.StateDesc ?? string.Empty,
            }).ToList();
            return new JsonResult { Data = custData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        public JsonResult GetCustomerForMapByCustomerID(string checkedcustomers)
        {
            var numbers = checkedcustomers.TrimEnd(',').Split(',').Select(Int32.Parse).ToList();
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
                    Floor = x.Floor ?? string.Empty,
                    Apartment = x.Apartment ?? string.Empty,
                    AreaPhone1 = x.AreaPhone1 ?? string.Empty,
                    Phone1 = x.Phone1 ?? string.Empty,
                    AreaPhone2 = x.AreaPhone2 ?? string.Empty,
                    Phone2 = x.Phone2 ?? string.Empty,
                    AreaFax = x.AreaFax ?? string.Empty,
                    Fax = x.Fax ?? string.Empty,
                    Mail = x.Mail ?? string.Empty,
                    CustomerRemark1 = x.CustomerRemark1 ?? string.Empty,
                    CustomerRemark2 = x.CustomerRemark2 ?? string.Empty,
                    VisitInterval = x.VisitInterval ?? 0,
                    NextVisit = x.NextVisit,
                    VisitDate = x.VisitDate,
                    VisitTime = x.VisitTime,
                    EndDate = x.EndDate,
                    Lat = x.Building.Lat,
                    BuildingCode = x.BuildingCode,
                    BuildingNumber = x.Building.Number ?? string.Empty,
                    Long = x.Building.Long,
                    ZipCode = x.Building.ZipCode,
                    StreetName = x.Building.Street.StreetDesc ?? string.Empty,
                    StreetId = x.Building.Street.StateCode,
                    CityId = x.Building.Street.City.CityCode,
                    CityName = x.Building.Street.City.CityDesc ?? string.Empty,
                    StateName = x.Building.Street.City.State.StateDesc ?? string.Empty,
                    StateId = x.Building.Street.City.State.StateCode
                }).ToList(); ;
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public JsonResult GetEmployeeByIdOnLoad()
        {
            int employeeID = 2;
            if (employeeID > 0)
            {
                using (ISEEEntities context = new ISEEEntities())
                {
                    int factoryId = SessionManagement.FactoryID;
                    var empData = context.Employees.ToList().Where(x => x.Factory == factoryId
                        && x.EmployeeId == employeeID).Select(x => new
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
            return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        //public JsonResult GetCustomerByIdOnLoad()
        //{
        //    int customerID = 692;
        //    if (customerID > 0)
        //    {
        //        using (ISEEEntities context = new ISEEEntities())
        //        {
        //            var result = context.Customers.Where(s => s.CustomerId == customerID).Select(x => new
        //            {
        //                CustomerId = x.CustomerId,
        //                CustomerNumber = x.CustomerNumber,
        //                FirstName = x.FirstName,
        //                LastName = x.LastName,
        //                AreaPhone1 = x.AreaPhone1 ?? "!@#$",
        //                Phone1 = x.Phone1 ?? "!@#$",
        //                Lat = x.Building.Lat,
        //                BuildingCode = x.BuildingCode,
        //                BuildingNumber = x.Building.Number ?? "!@#$",
        //                Long = x.Building.Long,
        //                ZipCode = x.Building.ZipCode,
        //                StreetName = x.Building.Street.StreetDesc ?? "!@#$",
        //                StreetId = x.Building.Street.StateCode,
        //                CityId = x.Building.Street.City.CityCode,
        //                CityName = x.Building.Street.City.CityDesc ?? "!@#$",
        //                StateName = x.Building.Street.City.State.StateDesc ?? "!@#$",
        //                StateId = x.Building.Street.City.State.StateCode
        //            }).ToList(); ;
        //            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //        }
        //    }
        //    return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}

    }
}