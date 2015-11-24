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
            var custData = _facory.GetCustomersNew(factoryId, State, City, Street, BuildingNumber, CustomerNumber, null, ContactName, null, null, true).Select(c => new
                           {
                               FirstName = c.FirstName ?? string.Empty,
                               id = c.CustomerId,
                               LastName = c.LastName ?? string.Empty,
                               AreaPhone1 = c.AreaPhone1 ?? string.Empty,
                               Phone1 = c.Phone1 ?? string.Empty
                           }).ToList();
            return new JsonResult { Data = new { Customers = custData }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }
    }
}