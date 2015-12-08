using ISEE.Common;
using ISEEDataModel.Repository;
using ISEEDataModel.Repository.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ISEE.Controllers
{
    public class CustomerController : Controller
    {
        //
        // GET: /Customer/
        ISEEEntities context = new ISEEEntities();
        ISEEFactory _facory = new ISEEFactory();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Customer()
        {
            if (SessionManagement.FactoryID == 0)
                return RedirectToAction("login", "login");
            List<TreeView> data = context.TreeViews.Where(tt => tt.FactoryID == SessionManagement.FactoryID && tt.ParentID == null).ToList();


            var serializer = new JavaScriptSerializer();
            ViewBag.JsonData = serializer.Serialize(context.PhoneManufactures.Select(pm => new { pm.PhoneManufacturId, pm.PhoneManufacture1 }).ToList());
            ViewBag.TreeJsonData = serializer.Serialize(Common.Common.CreateJsonTree(data));
            ViewBag.DefaultTreeJsonData = serializer.Serialize(Common.Common.CreateJsonTre(data)); 
            return View();
        }

        //{ state: state, city: city, street: street, building: building, custNumber: $('#custNumber').text().trim(), firstName: $('#custName').text().trim(), lastName: $('#custCompany').text().trim(), phone: $('#custPhone').text().trim(), phone1: $('#custPhone1').text().trim(), isActive: $('#isActive').is(':checked') }//
        public JsonResult GetCustomerSarch(int state, int city, int street, string building, string custNumber, string firstName, string lastName, string phone, string phone1, bool isActive)
        {


            building = Common.Common.GetNullableValues(building);

            custNumber = Common.Common.GetNullableValues(custNumber);
            firstName = Common.Common.GetNullableValues(firstName);
            lastName = Common.Common.GetNullableValues(lastName);
            phone = Common.Common.GetNullableValues(phone);
            phone1 = Common.Common.GetNullableValues(phone1);

            var results = _facory.GetCustomersNew(SessionManagement.FactoryID, state, city, street, building, custNumber, firstName, lastName, phone, phone1, isActive).ToList().Select(x => new
                     {
                         CustomerId = x.CustomerId,
                         CustomerNumber = x.CustomerNumber,
                         FirstName = x.FirstName ?? "!@#$",
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
                         NextVisit = x.NextVisit == null ? "!@#$" : x.NextVisit.Value.ToString("dd/MM/yyyy"),
                         VisitDate = x.VisitDate ?? 0,
                         VisitTime = x.VisitTime ?? 0,
                         EndDate = x.EndDate == null ? "!@#$" : x.EndDate.Value.ToString("dd/MM/yyyy"),
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
                         StateId = x.Building.Street.City.State.StateCode,
                         Mobile = x.AreaCelolar ?? "!@#$",
                         Mobile1 = x.Celolar ?? "!@#$"


                     }).ToList();

            return new JsonResult { Data = results, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }


        //Bind Main Classsification DDL 
        public JsonResult BindClassificationDdl()
        {
            using (ISEEEntities dataContext = new ISEEEntities())
            {
                var factoryLevel1list = dataContext.RequsetToFactoryLevel1.Where(d => d.Factory == SessionManagement.FactoryID).OrderBy(c => c.RequsetOrder).Select(x => new { x.RequestSysIdLevel1, x.RequestDescCodeLevel1, x.RequsetOrder, x.StatusCode, x.Factory }).ToList();
                return Json(factoryLevel1list, JsonRequestBehavior.AllowGet);
            }
        }
        //Bind Second Classsification DDL 
        public JsonResult BindSecondClassificationDdl(int id)
        {
            using (ISEEEntities dataContext = new ISEEEntities())
            {
                var factoryLevel1list = dataContext.RequsetToFactoryLevel2.Where(d => d.RequestSysIdLevel1 == id).OrderBy(c => c.RequsetOrder).Select(x => new { x.RequestSysIdLevel2, x.RequestDescCodeLevel2 }).ToList();
                return Json(factoryLevel1list, JsonRequestBehavior.AllowGet);
            }
        }

        //Bind Request Data
        public JsonResult GetRequestCustomerByDate(int customerID, int fromyear, string from, string to, int level1, int level2)
        {
            var fromdate = Convert.ToDateTime(from);
            var todate = Convert.ToDateTime(to);
            //var customerID = this.ObjectContext.Customer.FirstOrDefault(x => x.CustomerKey == CustomerGuidID).CustomerId;
            using (ISEEEntities context = new ISEEEntities())
            {
                var customerRequest1 = context.CustomerRequests.Include("RequsetToFactoryLevel2").Include("RequsetToFactoryLevel2.RequsetToFactoryLevel1").Where(x => x.CustomerId == customerID &&
                                                                                                                                                        x.RequsetToFactoryLevel2.RequestSysIdLevel1 == (level1 == -1 ? x.RequsetToFactoryLevel2.RequestSysIdLevel1 : level1) &&
                                                                                                                                                       x.RequestSysIdLevel2 == (level2 == -1 ? x.RequestSysIdLevel2 : level2) &&
                                                                                                                                                      (x.CreateDate >= fromdate && x.CreateDate < todate)).ToList().Select(x => new { RequestDescCodeLevel1= x.RequsetToFactoryLevel2.RequsetToFactoryLevel1.RequestDescCodeLevel1, RequestDescCodeLevel2 = x.RequsetToFactoryLevel2.RequestDescCodeLevel2, CreateDate = x.CreateDate.Date.ToString("dd/MM/yyyy"), Treatment = x.Treatment, TreatmentDate = x.TreatmentDate.ToString(), Request = x.Request, RequestSysIdLevel1 = x.RequsetToFactoryLevel2.RequestSysIdLevel1, RequestSysIdLevel2 = x.RequsetToFactoryLevel2.RequestSysIdLevel2, customerRequestId = x.SysId }).ToList();
                return Json(customerRequest1, JsonRequestBehavior.AllowGet);
            }

        }

        //Save Classificion request
        public bool UpdateClassificationRequestCustomerByDate(int customerID, string requestDate, int level2, string request, string treatment, string treatmentDate, int requestSysIdLevel1, int requestSysIdLevel2,int sysId)
        {
            try
            {

                using (ISEEEntities context = new ISEEEntities())
                {
                   CustomerRequest customerRequest =     context.CustomerRequests.Where(x => x.SysId == sysId).FirstOrDefault();
                    if (!string.IsNullOrEmpty(treatmentDate))
                        customerRequest.TreatmentDate = Convert.ToDateTime(treatmentDate);
                    customerRequest.Treatment = treatment;
                    customerRequest.Request = request;
                    customerRequest.RequestSysIdLevel2 = level2;
                    customerRequest.Status = 0;
                    customerRequest.RequsetToFactoryLevel2.RequestSysIdLevel1 = requestSysIdLevel1;
                    context.CustomerRequests.Add(customerRequest);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {

                return false;
            }

        }
        public bool SaveClassificationRequestCustomerByDate(int customerID, string requestDate, int level2, string request, string treatment, string treatmentDate)
        {
            try
            {

                using (ISEEEntities context = new ISEEEntities())
                {
                    CustomerRequest customerRequest = new CustomerRequest();
                    customerRequest.CreateDate = Convert.ToDateTime(requestDate);
                    if (!string.IsNullOrEmpty(treatmentDate))
                        customerRequest.TreatmentDate = Convert.ToDateTime(treatmentDate);
                    customerRequest.Treatment = treatment;
                    customerRequest.Request = request;
                    customerRequest.RequestSysIdLevel2 = level2;
                    customerRequest.Status = 0;
                    customerRequest.CustomerId = customerID;
                    context.CustomerRequests.Add(customerRequest);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {

                return false;
            }

        }
        // Bind Visiting Data
        public JsonResult GetEmployeesToCustomerFilter(int customerID, string dtFrom, string dtTo)
        {
            using (ISEEEntities context = new ISEEEntities())
            {
                DateTime from = Convert.ToDateTime(dtFrom);
                DateTime to = Convert.ToDateTime(dtTo);
                var gpsEmployeeCustomer = context.GpsEmployeeCustomers.Include("Employee").Where(
                                                                          x => x.CustomerId == customerID &&
                                                                          x.VisiteDate >= from &&
                                                                          x.VisiteDate <= to).OrderByDescending(x => x.VisiteDate).ToList().Select(x => new { EmployeeNum = x.Employee.EmployeeNum, CreateDate = x.VisiteDate.ToShortDateString(), Time = x.VisitTime.ToString(), FirstName = x.Employee.FirstName, LastName = x.Employee.LastName, EmployeeId = x.EmployeeId, InsertStatus = x.InsertStatus }).ToList();
                return Json(gpsEmployeeCustomer, JsonRequestBehavior.AllowGet);
                //.Select(x => new { x.CreateDate, x.CustomerId, x.Employee.FirstName, x.Employee.LastName, x.InsertStatus, x.Employee.EmployeeId, x.SysId, x.VisitTime, x.VisiteDate })
            }


        }

        public string UpdateCustomer(int customerID, string cNumber,
            string cCompanyName,
            string cContactName, string cFloor, string cApartment, string cMail,
            string cPhoneOne, string cPhone11, string cPhoneTwo, string cPhone22,
            string cFax, string cFax1, string cRemarks1, string cRemarks2,
            int cbuildingCode,
            string cbuildingNumber, string cZipCode, int cvisitInterval,
            string cEndDate, string cNextVisit, int cvisitTime, string cMobile, string cMobile1)
        {
            try
            {
                using (ISEEEntities context = new ISEEEntities())
                {
                    Customer customer = context.Customers.Where(x => x.CustomerId == customerID).FirstOrDefault();
                    customer.BuildingCode = cbuildingCode;
                    customer.CustomerNumber = Common.Common.GetNullableValues(cNumber.Trim());
                    customer.LastName = Common.Common.GetNullableValues(cCompanyName.Trim());
                    customer.FirstName = Common.Common.GetNullableValues(cContactName.Trim());
                    customer.Floor = Common.Common.GetNullableValues(cFloor.Trim());
                    customer.Apartment = Common.Common.GetNullableValues(cApartment.Trim());
                    customer.AreaPhone1 = Common.Common.GetNullableValues(cPhoneOne.Trim());
                    customer.Phone1 = Common.Common.GetNullableValues(cPhone11.Trim());
                    customer.AreaPhone2 = Common.Common.GetNullableValues(cPhoneTwo.Trim());
                    customer.Phone2 = Common.Common.GetNullableValues(cPhone22.Trim());
                    customer.AreaFax = Common.Common.GetNullableValues(cFax.Trim());
                    customer.Fax = Common.Common.GetNullableValues(cFax1.Trim());
                    customer.Mail = Common.Common.GetNullableValues(cMail.Trim());
                    customer.CustomerRemark1 = Common.Common.GetNullableValues(cRemarks1.Trim());
                    customer.CustomerRemark2 = Common.Common.GetNullableValues(cRemarks2.Trim());
                    customer.EndDate = Common.Common.ConvertDateTime(cEndDate);
                    customer.NextVisit = Common.Common.ConvertDateTime(cNextVisit);
                    customer.VisitTime = cvisitTime;
                    customer.VisitInterval = cvisitInterval;
                    customer.Celolar = Common.Common.GetNullableValues(cMobile1);
                    customer.AreaCelolar = Common.Common.GetNullableValues(cMobile);
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {

                return "false";
            }
            return "true";

        }
    }
}