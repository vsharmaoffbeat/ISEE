using ISEE.Common;
using ISEEDataModel.Repository;
using ISEEDataModel.Repository.Services;
using ISEEDataModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ISEE.Controllers
{
    public class EmployeeController : Controller
    {
        ISEEEntities context = new ISEEEntities();
        ISEEFactory _facory = new ISEEFactory();

        //
        // GET: /Employee/
        public ActionResult Index()
        {
            return View();
        }
        // GET: /Employee/
        public ActionResult Employee()
        {
            if (SessionManagement.FactoryID == 0)
                return RedirectToAction("login", "login");
            List<TreeView> data = context.TreeViews.Where(tt => tt.FactoryID == SessionManagement.FactoryID && tt.ParentID == null).ToList();


            var serializer = new JavaScriptSerializer();
            ViewBag.JsonData = serializer.Serialize(context.PhoneManufactures.Select(pm => new { pm.PhoneManufacturId, pm.PhoneManufacture1 }).ToList());
            ViewBag.TreeJsonData = serializer.Serialize(Common.Common.CreateJsonTree(data));
            return View();
        }

        public JsonResult GetCustomersNew(int State, int City, int Street, string BuildingNumber, string ContactName, string CustomerNumber)
        {
            int factoryId = SessionManagement.FactoryID;

            //TODO: Will be removed when connected with login process
            if (factoryId == 0)
            {
                factoryId = 1;
            }

            var custData = _facory.GetCustomersNew(factoryId, State, City, Street, BuildingNumber, CustomerNumber, null, ContactName, null, null, true).Select(c => new CustomerModel
                           {
                               FirstName = c.FirstName ?? string.Empty,
                               id = c.CustomerId,
                               LastName = c.LastName ?? string.Empty,
                               AreaPhone1 = c.AreaPhone1 ?? string.Empty,
                               Phone1 = c.Phone1 ?? string.Empty,
                               CustomerNummber = c.CustomerNumber,
                               StreetDesc = c.Building.Street.StreetDesc,
                               CityDesc = c.Building.Street.City.CityDesc,
                               Lat = c.Building.Lat,
                               Long = c.Building.Long
                           }).ToList();
            return new JsonResult { Data = new { Customers = custData }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }

        public JsonResult GetDistance(CustomerModel model, string uniqueid)
        {
            int factoryId = SessionManagement.FactoryID;
            double Dist = 0;
            try
            {
                var objEmployeeGpsPoint = _facory.GetEmployeeGpsPointBySysId(Convert.ToInt32(uniqueid));
                if (objEmployeeGpsPoint != null)
                {
                    //distination - Lat, Long from customer; Lat, Long from stop point
                    if (model.Lat != null && model.Long != null)
                        Dist = Utility.distance((double)model.Lat, (double)model.Long, (double)objEmployeeGpsPoint.Lat, (double)objEmployeeGpsPoint.Long, 'K');
                }

                return new JsonResult { Data = new { IsSuccess = true, color = (Dist > 250 ? Category.Orange.ToString() : Category.Yellow.ToString()) }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception)
            {
                return new JsonResult { Data = new { IsSuccess = false, color = (Dist > 250 ? Category.Orange.ToString() : Category.Yellow.ToString()) }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public JsonResult SaveEvents(List<CalendarEvent> model)
        {
            int factoryId = SessionManagement.FactoryID;
            double Dist = 0;
            try
            {
                model = model.Where(c => c.customerId != null).ToList();
                _facory.SaveUpdateEvents(model);
                return new JsonResult { Data = new { IsSuccess = true, color = (Dist > 250 ? Category.Orange : Category.Yellow) }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception)
            {
                return new JsonResult { Data = new { IsSuccess = false, color = (Dist > 250 ? Category.Orange : Category.Yellow) }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
    }
}