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

                var ss = (from cc in context.Customers.Include("Building").Include("Building.Street").Include("Building.Street.City").Include("Building.Street.City.State")
                          join s in context.States on cc.Building.StateCode equals s.StateCode
                          join c in context.Cities on cc.Building.CityCode equals c.CityCode
                          join st in context.Streets on cc.Building.StreetCode equals st.StreetCode
                          where cc.Factory == ISEE.Common.SessionManegment.SessionManagement.FactoryID
                     &&
                      (state != 0 ? cc.Building.StateCode == state : cc.Building.StateCode == null) &&
      cc.Building.StateCode == (state == 0 ? cc.Building.StateCode : state) &&
      cc.Building.CityCode == (city == 0 ? cc.Building.CityCode : city) &&
      cc.Building.StreetCode == (street == 0 ? cc.Building.StreetCode : street) &&
      cc.Building.Number.Contains(building == null ? cc.Building.Number : building) &&
      cc.CustomerNumber.CompareTo(custNumber == null ? cc.CustomerNumber : custNumber) == 0


                 && cc.FirstName.Contains(firstName == null ? cc.FirstName : firstName) &&
      (cc.LastName == null || cc.LastName.Contains(lastName == null ? cc.LastName : lastName)) &&
      (cc.AreaPhone1 == null || cc.AreaPhone1.Contains(phone == null ? cc.AreaPhone1 : phone)) &&
      (cc.Phone1 == null || cc.Phone1.Contains(phone1 == null ? cc.Phone1 : phone1)) &&
      (isActive ? (cc.EndDate == null || (cc.EndDate != null && cc.EndDate >= DateTime.Now)) : (cc.EndDate != null && cc.EndDate < DateTime.Now))
                          select new
    {
        CustomerId = cc.CustomerId,
        CustomerNumber = cc.CustomerNumber,
        FirstName = cc.FirstName,
        LastName = cc.LastName,
        Floor = cc.Floor,
        Apartment = cc.Apartment,
        AreaPhone1 = cc.AreaPhone1,
        Phone1 = cc.Phone1,
        AreaPhone2 = cc.AreaPhone2,
        Phone2 = cc.Phone2,
        AreaFax = cc.AreaFax,
        Fax = cc.Fax,
        Mail = cc.Mail,
        CustomerRemark1 = cc.CustomerRemark1,
        CustomerRemark2 = cc.CustomerRemark2,
        VisitInterval = cc.VisitInterval,
        NextVisit = cc.NextVisit,
        VisitDate = cc.VisitDate,
        VisitTime = cc.VisitTime,
        EndDate = cc.EndDate,
        Lat = cc.Building.Lat,
        Long = cc.Building.Long,
        ZipCode = cc.Building.ZipCode,
        StateName = cc.Building.StateCode,
        IS = c.CityDesc
    }).ToList();

                //var ccccc= (from ss in context.States 


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

                return new JsonResult { Data = results, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }


    }
}