using ISEEDataModel.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISEE.Controllers
{
    public class CustomerController : Controller
    {
        //
        // GET: /Customer/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Customer()
        {
            return View();
        }
        public string GetNullableValues(string value)
        {
            if (string.IsNullOrEmpty(value.Trim()))
                return null;
            return value;

        }
        //{ state: state, city: city, street: street, building: building, custNumber: $('#custNumber').text().trim(), firstName: $('#custName').text().trim(), lastName: $('#custCompany').text().trim(), phone: $('#custPhone').text().trim(), phone1: $('#custPhone1').text().trim(), isActive: $('#isActive').is(':checked') }//
        public JsonResult GetCustomerSarch(int state, int city, int street, string building, string custNumber, string firstName, string lastName, string phone, string phone1, bool isActive)
        {

            building = GetNullableValues(building);
            custNumber = GetNullableValues(custNumber);
            firstName = GetNullableValues(firstName);
            lastName = GetNullableValues(lastName);
            phone = GetNullableValues(phone);
            phone1 = GetNullableValues(phone1);



            using (ISEEEntities context = new ISEEEntities())
            {
                var sbdhj = context.Customers.Where(x => x.Factory == 1).ToList();
               

                //var ccccc= (from ss in context.States 
                var results11 = context.Customers.Include("Building").Include("Building.Street").Include("Building.Street.City").Include("Building.Street.City.State").Where(x => x.Factory == ISEE.Common.SessionManegment.SessionManagement.FactoryID &&
                     (state != 0 ? x.Building.StateCode == state : x.Building.StateCode == null) &&
     x.Building.StateCode == (state == 0 ? x.Building.StateCode : state) &&
     x.Building.CityCode == (city == 0 ? x.Building.CityCode : city) &&
     x.Building.StreetCode == (street == 0 ? x.Building.StreetCode : street) &&
     x.Building.Number.Contains(building == null ? x.Building.Number : building) &&
     x.CustomerNumber.CompareTo(custNumber == null ? x.CustomerNumber : custNumber) == 0


                && x.FirstName.Contains(firstName == null ? x.FirstName : firstName) &&
     (x.LastName == null || x.LastName.Contains(lastName == null ? x.LastName : lastName)) &&
     (x.AreaPhone1 == null || x.AreaPhone1.Contains(phone == null ? x.AreaPhone1 : phone)) &&
     (x.Phone1 == null || x.Phone1.Contains(phone1 == null ? x.Phone1 : phone1)) &&
     (isActive ? (x.EndDate == null || (x.EndDate != null && x.EndDate >= DateTime.Now)) : (x.EndDate != null && x.EndDate < DateTime.Now))).ToList().Select(x => new
     {
         CustomerId = x.CustomerId,
         CustomerNumber = x.CustomerNumber,
         FirstName = x.FirstName ?? "!@#$" ,
         LastName = x.LastName ?? "!@#$",
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
         BuildingCode= x.BuildingCode,
         BuildingNumber = x.Building.Number ?? "!@#$",
         Long = x.Building.Long,
         ZipCode = x.Building.ZipCode,
         StreetName = x.Building.Street.StreetDesc ?? "!@#$",
         StreetId = x.Building.Street.StateCode,
         CityId = x.Building.Street.City.CityCode,
         CityName = x.Building.Street.City.CityDesc ?? "!@#$" ,
         StateName = x.Building.Street.City.State.StateDesc ?? "!@#$",
         StateId = x.Building.Street.City.State.StateCode



     }).ToList();







                var results = context.Customers.Include("Building").Include("Building.Street").Include("Building.Street.City").Include("Building.Street.City.State").Where(x => x.Factory == ISEE.Common.SessionManegment.SessionManagement.FactoryID &&
                     (state != 0 ? x.Building.StateCode == state : x.Building.StateCode == null) &&
     x.Building.StateCode == (state == 0 ? x.Building.StateCode : state) &&
     x.Building.CityCode == (city == 0 ? x.Building.CityCode : city) &&
     x.Building.StreetCode == (street == 0 ? x.Building.StreetCode : street) &&
     x.Building.Number.Contains(building == null ? x.Building.Number : building) &&
     x.CustomerNumber.CompareTo(custNumber == null ? x.CustomerNumber : custNumber) == 0


                && x.FirstName.Contains(firstName == null ? x.FirstName : firstName) &&
     (x.LastName == null || x.LastName.Contains(lastName == null ? x.LastName : lastName)) &&
     (x.AreaPhone1 == null || x.AreaPhone1.Contains(phone == null ? x.AreaPhone1 : phone)) &&
     (x.Phone1 == null || x.Phone1.Contains(phone1 == null ? x.Phone1 : phone1)) &&
     (isActive ? (x.EndDate == null || (x.EndDate != null && x.EndDate >= DateTime.Now)) : (x.EndDate != null && x.EndDate < DateTime.Now))).ToList().Select(x => new
     {
         CustomerId = x.CustomerId,
         CustomerNumber = x.CustomerNumber,
         FirstName = x.FirstName,
         LastName = x.LastName,
         Floor = x.Floor,
         Apartment = x.Apartment,
         AreaPhone1 = x.AreaPhone1,
         Phone1 = x.Phone1,
         AreaPhone2 = x.AreaPhone2,
         Phone2 = x.Phone2,
         AreaFax = x.AreaFax,
         Fax = x.Fax,
         Mail = x.Mail,
         CustomerRemark1 = x.CustomerRemark1,
         CustomerRemark2 = x.CustomerRemark2,
         VisitInterval = x.VisitInterval,
         NextVisit = x.NextVisit,
         VisitDate = x.VisitDate,
         VisitTime = x.VisitTime,
         EndDate = x.EndDate,
         Lat = x.Building.Lat,
         Long = x.Building.Long,
         ZipCode = x.Building.ZipCode,
         StateName = x.Building.StateCode,

     }).ToList();

                return new JsonResult { Data = results11, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }


    }
}