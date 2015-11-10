using ISEE.Common;
using ISEEDataModel.Repository;
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
        //
        // GET: /Employee/
        public ActionResult Index()
        {
            return View();
        }
        // GET: /Employee/
        public ActionResult Employee()
        {
             SessionManegment.SessionManagement.FactoryID = 1;
            SessionManegment.SessionManagement.FactoryDesc = "Demo Company";
            List<TreeView> data = context.TreeViews.Where(tt => tt.FactoryID == SessionManegment.SessionManagement.FactoryID && tt.ParentID == null).ToList();

          
            var serializer = new JavaScriptSerializer();
            ViewBag.JsonData = serializer.Serialize(context.PhoneManufactures.Select(pm => new { pm.PhoneManufacturId, pm.PhoneManufacture1 }).ToList());
            ViewBag.TreeJsonData = serializer.Serialize(AdminController.CreateJsonTree(data));
            return View();
        }

	}
}