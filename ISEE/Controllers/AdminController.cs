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
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        public ActionResult Index()
        {
            return View();
        }
        ISEEEntities dataCntext = new ISEEEntities();
        public ActionResult Admin()
        {
            //if (ISEE.Common.SessionManegment.SessionManagement.FactoryID == 0)
            //    return RedirectToAction("login", "login");
            return View();
        }
        public ActionResult _NewEmployee()
        {
            return PartialView();
        }
        public ActionResult _NewCustomer()
        {
            return PartialView();
        }

        public ActionResult _AdminTree()
        {
            SessionManegment.SessionManagement.FactoryID = 1;
            SessionManegment.SessionManagement.FactoryDesc = "Demo Company";
            List<TreeView> data = dataCntext.TreeViews.Where(tt => tt.FactoryID == SessionManegment.SessionManagement.FactoryID).ToList();

            //List<Jstreenode> treeList = new List<Jstreenode>();
            //treeList.Add(new Jstreenode() { id = "1", text = "Demo Company", icon = "" });
            //treeList.Add(new Jstreenode() { id = "2", parent = "1", text = "Root Node", icon = "jstree-icon jstree-themeicon" });
            //treeList.Add(new Jstreenode() { id = "3", parent = "1", text = "Child Node 1", icon = "jstree-icon jstree-themeicon" });
            //treeList.Add(new Jstreenode() { id = "4", parent = "2", text = "Child Node 2", icon = "jstree-icon jstree-themeicon" });
            var serializer = new JavaScriptSerializer();
            ViewBag.JsonData = serializer.Serialize(CreateJsonTree(data));

            return PartialView();
        }
        public List<TreeNodeData> CreateJsonTree(List<TreeView> data)
        {

            List<TreeNodeData> treeList = new List<TreeNodeData>();
            treeList.Add(new TreeNodeData() { id = SessionManegment.SessionManagement.FactoryID.ToString(), text = SessionManegment.SessionManagement.FactoryDesc, parent = "#" });
            foreach (var item in data)
            {
                if (item.EmployeeID != null)
                {
                var emp=dataCntext.Employees.Where(x=>x.EmployeeId==item.EmployeeID).FirstOrDefault();
                treeList.Add(new TreeNodeData() { id = item.BranchID, parent = item.ParentID, text = emp.LastName + " " + emp.FirstName, icon = "jstree-icon user", objectid = item.EmployeeID, objecttype = "employee" });
                }
                else if (item.CustomerID != null)
                {
                    var cust = dataCntext.Customers.Where(x => x.CustomerId == item.CustomerID).FirstOrDefault();

                    treeList.Add(new TreeNodeData() { id = item.BranchID, parent = item.ParentID, text = cust.LastName + " " + cust.FirstName, icon = "jstree-icon user", objectid = item.CustomerID, objecttype = "customer" });
                
                }
                else
                    treeList.Add(new TreeNodeData() { id = item.BranchID, parent = item.ParentID, text = item.Decription, icon = "jstree-icon jstree-themeicon" });
            }
            //treeList.Add(new Jstreenode() { id = "2", parent = "1", text = "Root Node", icon = "jstree-icon jstree-themeicon" });
            //treeList.Add(new Jstreenode() { id = "3", parent = "1", text = "Child Node 1", icon = "jstree-icon jstree-themeicon" });
            //treeList.Add(new Jstreenode() { id = "4", parent = "2", text = "Child Node 2", icon = "jstree-icon jstree-themeicon" });
            return treeList;
        }

        public JsonResult GetEmployee(string firstname, string lastname, string phone)
        {
            firstname = string.IsNullOrEmpty(firstname) ? null : firstname;
            lastname = string.IsNullOrEmpty(lastname) ? null : lastname;
            phone = string.IsNullOrEmpty(phone) ? null : phone;

            using (ISEEEntities context = new ISEEEntities())
            {
                int factoryId = ISEE.Common.SessionManegment.SessionManagement.FactoryID = 1;
                bool _Active = true;
                var empData = dataCntext.Employees.Where(x => x.Factory == factoryId
                                                         && x.FirstName.Contains(firstname == null ? x.FirstName : firstname)
                                                         && (string.IsNullOrEmpty(lastname) || x.LastName.Contains(lastname))
                                                         && x.EmployeeNum.Contains(phone == null ? x.EmployeeNum : phone)
                                                         && (_Active == true ? (x.EndDay == null || (x.EndDay != null && x.EndDay >= DateTime.Now)) : (x.EndDay != null && x.EndDay < DateTime.Now))).OrderBy(x => x.EmployeeNum).Select(x => new { FirstName = x.FirstName, LastName = x.LastName, MainPhone = x.MainPhone, id = x.EmployeeId, x.MainAreaPhone }).ToList();
                return new JsonResult { Data = empData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        public JsonResult GetCustomers(string state, string city, string street, string buldingNumber, string customerNumber, string contactName, string companyName, string phone1)
        {
            GetInteger(state);
            using (ISEEEntities context = new ISEEEntities())
            {
                int factoryId = ISEE.Common.SessionManegment.SessionManagement.FactoryID = 1;
                bool _Active = true;

                var custData = dataCntext.Customers.Include("Building").Include("Building.Street").Include("Building.Street.City").Include("Building.Street.City.State").Where(x => x.Factory == factoryId &&
                     (GetInteger(state) != 0 ? x.Building.StateCode == GetInteger(state) : x.Building.StateCode == null) &&
                                                                                                            x.Building.StateCode == (GetInteger(state) == 0 ? x.Building.StateCode : GetInteger(state)) &&
                                                                                                            x.Building.CityCode == (GetInteger(city) == 0 ? x.Building.CityCode : GetInteger(city)) &&
                                                                                                            x.Building.StreetCode == (GetInteger(street) == 0 ? x.Building.StreetCode : GetInteger(street)) &&
                                                                                                            x.Building.Number.Contains(buldingNumber == null ? x.Building.Number : buldingNumber) &&
                                                                                                            x.CustomerNumber.CompareTo(customerNumber == null ? x.CustomerNumber : customerNumber) == 0 &&
                                                                                                            x.FirstName.Contains(contactName == null ? x.FirstName : contactName) &&
                    // (x.LastName == null || x.LastName.Contains(_LN == null ? x.LastName : _LN)) &&
                    // (x.AreaPhone1 == null || x.AreaPhone1.Contains(_area == null ? x.AreaPhone1 : _area)) &&
                                                                                                            (x.Phone1 == null || x.Phone1.Contains(phone1 == null ? x.Phone1 : phone1)) &&

                                                                                                            (_Active ? (x.EndDate == null || (x.EndDate != null && x.EndDate >= DateTime.Now)) : (x.EndDate != null && x.EndDate < DateTime.Now))).Select(c => new { c.FirstName, c.CustomerId, c.LastName, c.AreaPhone1, c.Phone1 }).ToList();
                return new JsonResult { Data = custData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }



        public int GetInteger(string val)
        {

            int output;
            int.TryParse(val, out output);
            //if (isNaN(output))
            //    return output;
            return output;
        }

        public ActionResult _Category()
        {
            return View();
        }
        public JsonResult GetEmployeeHours()
        {
            int factoryId = ISEE.Common.SessionManegment.SessionManagement.FactoryID;
            using (ISEEEntities context = new ISEEEntities())
            {
                var EmpHours = context.FactoryDairyTemplets.Where(s => s.Factory == factoryId).ToList().Select(e => new { Day = e.DayStatus, Start1 = e.Start1 != null ? Convert.ToDateTime(e.Start1.Value.ToString()).ToShortTimeString() : null, End1 = e.Stop1 != null ? Convert.ToDateTime(e.Stop1.Value.ToString()).ToShortTimeString() : null, Start2 = e.Start2 != null ? Convert.ToDateTime(e.Start2.Value.ToString()).ToShortTimeString() : null, End2 = e.Stop2 != null ? Convert.ToDateTime(e.Stop2.Value.ToString()).ToShortTimeString() : null }).ToList();

                return new JsonResult { Data = EmpHours, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

        }
        public bool SaveEmployeeHours(string objhours, int employeeID)
        {
            int factoryId = ISEE.Common.SessionManegment.SessionManagement.FactoryID;
            var mainData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ISEEDataModel.Repository.employeeHours>>(objhours);
            if (employeeID > 0)
            {
                using (ISEEEntities context = new ISEEEntities())
                {

                    foreach (var item in mainData)
                    {
                        EmployeeDiaryTemplate factoryDairyTemplet = new EmployeeDiaryTemplate();
                        factoryDairyTemplet.DayStatus = item.Day;
                        factoryDairyTemplet.EmployeeId = employeeID;
                        factoryDairyTemplet.Start1 = Convert.ToDateTime(item.Start1).ToShortTimeString() != null ? (new TimeSpan(Int32.Parse(Convert.ToDateTime(item.Start1).ToShortTimeString().Split(':')[0]), Int32.Parse((Convert.ToDateTime(item.Start1).ToShortTimeString().Split(':')[1]).Split(' ')[0]), 0)) : new TimeSpan(0);
                        factoryDairyTemplet.Stop1 = Convert.ToDateTime(item.End1).ToShortTimeString() != null ? (new TimeSpan(Int32.Parse(Convert.ToDateTime(item.End1).ToShortTimeString().Split(':')[0]), Int32.Parse((Convert.ToDateTime(item.End1).ToShortTimeString().Split(':')[1]).Split(' ')[0]), 0)) : new TimeSpan(0);
                        factoryDairyTemplet.Start2 = Convert.ToDateTime(item.Start2).ToShortTimeString() != null ? (new TimeSpan(Int32.Parse(Convert.ToDateTime(item.Start2).ToShortTimeString().Split(':')[0]), Int32.Parse((Convert.ToDateTime(item.Start2).ToShortTimeString().Split(':')[1]).Split(' ')[0]), 0)) : new TimeSpan();
                        factoryDairyTemplet.Stop2 = Convert.ToDateTime(item.End2).ToShortTimeString() != null ? (new TimeSpan(Int32.Parse(Convert.ToDateTime(item.End2).ToShortTimeString().Split(':')[0]), Int32.Parse((Convert.ToDateTime(item.End2).ToShortTimeString().Split(':')[1]).Split(' ')[0]), 0)) : new TimeSpan(0);
                        factoryDairyTemplet.OrderDay = item.Day;
                        context.EmployeeDiaryTemplates.Add(factoryDairyTemplet);
                        context.SaveChanges();
                    }
                    return true;
                }

            }
            return false;
        }

        public int SaveEmployeeData(string number, string firstName, string lastName, string startDay, string enddate, string phone1, string phone11, string phone2, string phone22, string ManufactureChoice, string phoneTypeChoice)
        {
            int factoryId = ISEE.Common.SessionManegment.SessionManagement.FactoryID;
            int EmployeeID = 0;
            using (ISEEEntities context = new ISEEEntities())
            {
                try
                {
                    Employee emp = new Employee();
                    emp.FirstName = firstName;
                    emp.LastName = lastName;
                    emp.EmployeeNum = number;
                    emp.SysCreatDate = DateTime.Now;
                    emp.Factory = factoryId;
                    emp.StartDay = Convert.ToDateTime(startDay).Date;
                    emp.EndDay = Convert.ToDateTime(enddate).Date;
                    emp.PhoneManufactory = Convert.ToInt32(ManufactureChoice);
                    emp.PhoneType = Convert.ToInt32(phoneTypeChoice != "" ? phoneTypeChoice : "1");
                    emp.SecondPhone = phone2;
                    emp.SecondAreaPhone = phone22;
                    emp.MainAreaPhone = phone11;
                    emp.MainPhone = phone1;
                    emp.EmployeeKey = new Guid();
                    context.Employees.Add(emp);
                    context.SaveChanges();
                    EmployeeID = emp.EmployeeId;
                }
                catch (Exception ex)
                {
                    return 0;
                }

            }
            return EmployeeID;
        }
        public JsonResult GetPhoneTypes(int id)
        {

            using (ISEEEntities context = new ISEEEntities())
            {
                var phoneTypes = context.PhoneTypes.Where(x => x.PhoneManufacturId == id).Select(c => new { PhoneTypeCode = c.PhoneTypeCode, PhoneTypeDesc = c.PhoneTypeDesc }).ToList();
                return new JsonResult { Data = phoneTypes, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

        }

        public JsonResult GetCurrentLogedUserCountery()
        {
            int FactoryId = 2;
            if (FactoryId > 0)
            {
                using (ISEEEntities context = new ISEEEntities())
                {
                    var CountryDetail = context.FactoryParms.Select(c => new { FactoryId = c.FactoryId, CountryID = c.Country, Lat = c.Lat, Long = c.Long, Zoom = c.Zoom }).Where(x => x.FactoryId == FactoryId).ToList();
                    return new JsonResult { Data = CountryDetail, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public JsonResult insertAddress(string state, string city, string street, string buildingNumber, string zipCode, string visitTime, string entry, string visitInterval, string nextVisit)
        {
            if (state != "")
            {
                using (ISEEEntities context = new ISEEEntities())
                {
                    var result = context.States.Select(c => new { c.StateCode, c.StateDesc }).Where(s => s.StateDesc.StartsWith(state));


                }
            }
            return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult SaveCustomerForm(string inputCustomerNumber, string inputFloor, string inputPhone1, string inputCompanyName, string inputApartment, string inputPhone2, string inputContactName, string inputMail, string inputMobile, string inputFax, string inputState, string state, string city, string street, string buildingNumber, string zipCode, string visitTime, string entry, string visitInterval, string nextVisit)
        {
            using (ISEEEntities context = new ISEEEntities())
            {
                return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

        }

        #region Category Tab
        #region Category Level


        public JsonResult getAll(string id)
        {

            SessionManegment.SessionManagement.FactoryID = 1;
            using (ISEEEntities dataContext = new ISEEEntities())
            {
                if (string.IsNullOrEmpty(id))
                {
                    var factoryLevel1list = dataContext.RequsetToFactoryLevel1.Where(d => d.Factory == SessionManegment.SessionManagement.FactoryID).Select(x => new { x.RequestSysIdLevel1, x.RequestDescCodeLevel1, x.RequsetOrder, x.StatusCode, x.Factory }).OrderBy(x => x.RequestSysIdLevel1).ToList();
                    return Json(factoryLevel1list, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var factoryLevel1list = dataContext.RequsetToFactoryLevel1.ToList().Where(d => d.Factory == SessionManegment.SessionManagement.FactoryID && d.StatusCode == GetInteger(id)).Select(x => new { x.RequestSysIdLevel1, x.RequestDescCodeLevel1, x.RequsetOrder, x.StatusCode, x.Factory }).OrderBy(x => x.RequestSysIdLevel1).ToList();
                    return Json(factoryLevel1list, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public JsonResult GetSecondary(Int32 sysIdLevel1)
        {
            using (ISEEEntities dataContext = new ISEEEntities())
            {
                var factoryLevel2list = dataContext.RequsetToFactoryLevel2.Where(d => d.RequestSysIdLevel1 == sysIdLevel1).Select(x => new { x.RequestDescCodeLevel2, x.RequsetOrder, x.RequestSysIdLevel1, x.RequestSysIdLevel2, x.StatusCode }).ToList();
                return Json(factoryLevel2list, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveCategory(string objcategory, string objSecondary)
        {
            var MainClassification = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ISEEDataModel.Repository.RequsetToFactoryLevel1>>(objcategory);
            var SecondaryClassification = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ISEEDataModel.Repository.RequsetToFactoryLevel2[]>>(objSecondary);
            //List<ISEEDataModel.Repository.RequsetToFactoryLevel1> objcategory, List<ISEEDataModel.Repository.RequsetToFactoryLevel2[]> objSecondary)
            using (ISEEEntities db = new ISEEEntities())
            {
                foreach (var item in MainClassification)
                {
                    if (db.RequsetToFactoryLevel1.Any(a => a.RequestSysIdLevel1 == item.RequestSysIdLevel1 && a.Factory == 1)) //
                    {
                        item.Factory = 1;
                        //   db.RequsetToFactoryLevel1.Attach(item);
                        ISEEDataModel.Repository.RequsetToFactoryLevel1 objRequsetToFactoryLevel1 = db.RequsetToFactoryLevel1.Where(x => x.RequestSysIdLevel1 == item.RequestSysIdLevel1).FirstOrDefault();
                        //var empQuery = from RequsetToFactoryLevel1 in db.RequsetToFactoryLevel1
                        //              where RequsetToFactoryLevel1.RequestSysIdLevel1 == item.RequestSysIdLevel1
                        //              select RequsetToFactoryLevel1;
                        //ISEEDataModel.Repository.RequsetToFactoryLevel1 objRequsetToFactoryLevel1 = empQuery.Single();
                        //set the new values of the columns (properties), based upon the values entered using the text boxes
                        objRequsetToFactoryLevel1.Factory = item.Factory;
                        objRequsetToFactoryLevel1.RequestDescCodeLevel1 = item.RequestDescCodeLevel1;
                        objRequsetToFactoryLevel1.RequsetOrder = item.RequsetOrder;
                        objRequsetToFactoryLevel1.StatusCode = item.StatusCode;
                        db.SaveChanges();
                        foreach (var secondary in SecondaryClassification)
                        {
                            if (item.RequestSysIdLevel1 == secondary[0].RequestSysIdLevel1)
                            {
                                foreach (var sec in secondary)
                                {
                                    if (db.RequsetToFactoryLevel2.Any(a => a.RequestSysIdLevel2 == sec.RequestSysIdLevel2)) //
                                    {
                                        ISEEDataModel.Repository.RequsetToFactoryLevel2 objRequsetToFactoryLevel2 = db.RequsetToFactoryLevel2.Where(x => x.RequestSysIdLevel2 == sec.RequestSysIdLevel2).FirstOrDefault();

                                        //db.RequsetToFactoryLevel2.Attach(sec);
                                        //var Query = from RequsetToFactoryLevel2 in db.RequsetToFactoryLevel2
                                        //            where RequsetToFactoryLevel2.RequestSysIdLevel2 == sec.RequestSysIdLevel2
                                        //            select RequsetToFactoryLevel2;
                                        //ISEEDataModel.Repository.RequsetToFactoryLevel2 objRequsetToFactoryLevel2 = Query.Single();
                                        //set the new values of the columns (properties), based upon the values entered using the text boxes
                                        objRequsetToFactoryLevel2.RequestSysIdLevel1 = sec.RequestSysIdLevel1;
                                        objRequsetToFactoryLevel2.RequestDescCodeLevel2 = sec.RequestDescCodeLevel2;
                                        objRequsetToFactoryLevel2.RequsetOrder = sec.RequsetOrder;
                                        objRequsetToFactoryLevel2.StatusCode = sec.StatusCode;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        db.RequsetToFactoryLevel2.Add(sec);
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        item.Factory = 1;
                        var dateAndTime = DateTime.Now;
                        item.CreateDate = dateAndTime.Date;
                        db.RequsetToFactoryLevel1.Add(item);
                        db.SaveChanges();
                        int max = db.RequsetToFactoryLevel1.Max(p => p.RequestSysIdLevel1);
                        foreach (var secondary in SecondaryClassification)
                        {
                            if (item.RequestSysIdLevel1 == secondary[0].RequestSysIdLevel1)
                            {
                                foreach (var sec in secondary)
                                {
                                    sec.CreateDate = dateAndTime.Date;
                                    db.RequsetToFactoryLevel2.Add(sec);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
                return new JsonResult { Data = objcategory, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }



        #endregion

        #endregion
        #region Tree Tab


        public JsonResult SaveTreeViewData(string treeViewData)
        {
            var tree = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TreeNodeData>>(treeViewData);
            List<TreeView> treeData = dataCntext.TreeViews.Where(x => x.FactoryID == SessionManegment.SessionManagement.FactoryID).ToList();
            TreeView view = new TreeView();
            foreach (var item in tree)
            {
                if (item.parent != "#")
                {
                    view = treeData.Where(t => t.ParentID == item.parent && t.BranchID == item.id).FirstOrDefault();
                    if (view != null)
                    {
                        dataCntext.TreeViews.Attach(new TreeView());
                    }
                    else
                    {
                        view = new TreeView();
                        view.ParentID = item.parent;
                        view.BranchID = item.id;
                        if (!string.IsNullOrEmpty(item.objecttype) && item.objecttype == "employee" && item.objectid != null)
                            view.EmployeeID = item.objectid;
                        else if (!string.IsNullOrEmpty(item.objecttype) && item.objecttype == "customer" && item.objectid != null)
                            view.CustomerID = item.objectid;
                        else
                            view.Decription = item.text;
                        dataCntext.TreeViews.Add(view);
                    }

                }

            }

            dataCntext.SaveChanges();
            return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //  public List<Employee> GetEmployees(Guid factoryGuid,string _LN, string _FN,  string _Num, int _Manuf, int _Phonetype,bool _Active)
        //{


        //       // int factory = (int)HttpContext.Current.Session["FactoryId"];
        //    int factoryId = ISEE.Common.SessionManegment.SessionManagement.FactoryID;
        //        var abc= dataCntext.Employees.Where(x => x.Factory == factoryId
        //                                                          && x.FirstName.Contains(_FN == null ? x.FirstName : _FN)
        //                                                          && (string.IsNullOrEmpty(_LN) || x.LastName.Contains(_LN))
        //                                                          && x.EmployeeNum.Contains(_Num == null ? x.EmployeeNum : _Num)
        //                                                          && x.PhoneManufactory == (_Manuf == 0 ? x.PhoneManufactory : _Manuf)
        //                                                          && x.PhoneType == (_Phonetype == 0 ? x.PhoneType : _Phonetype)
        //                                                          && (_Active == true ? (x.EndDay == null || (x.EndDay != null && x.EndDay >= DateTime.Now)) : (x.EndDay != null && x.EndDay < DateTime.Now))).OrderBy(x => x.EmployeeNum).ToList();


        //}
        #endregion

    }
}