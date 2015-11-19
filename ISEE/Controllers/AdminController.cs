using ISEE.Common;
using ISEE.Models;
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
        public enum Days
        {
            Sun = 1,
            Mon = 2,
            Tue = 3,
            Wed = 4,
            Thu = 5,
            Fri = 6,
            Sat = 7

        }

        static ISEEEntities dataContext = new ISEEEntities();
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

        public JsonResult GetEmployee(string firstname, string lastname, string phone)
        {
            firstname = string.IsNullOrEmpty(firstname) ? null : firstname;
            lastname = string.IsNullOrEmpty(lastname) ? null : lastname;
            phone = string.IsNullOrEmpty(phone) ? null : phone;

            using (ISEEEntities context = new ISEEEntities())
            {
                int factoryId = ISEE.Common.SessionManegment.SessionManagement.FactoryID;
                bool _Active = true;
                var empData = dataContext.Employees.Where(x => x.Factory == factoryId
                                                         && x.FirstName.Contains(firstname == null ? x.FirstName : firstname)
                                                         && (string.IsNullOrEmpty(lastname) || x.LastName.Contains(lastname))
                                                         && x.EmployeeNum.Contains(phone == null ? x.EmployeeNum : phone)
                                                         && (_Active == true ? (x.EndDay == null || (x.EndDay != null && x.EndDay >= DateTime.Now)) : (x.EndDay != null && x.EndDay < DateTime.Now))).OrderBy(x => x.EmployeeNum).Select(x => new { FirstName = x.FirstName, LastName = x.LastName, MainPhone = x.MainPhone, id = x.EmployeeId, x.MainAreaPhone }).ToList();
                return new JsonResult { Data = empData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        public JsonResult GetCustomers(string state, string city, string street, string buldingNumber, string customerNumber, string contactName, string companyName, string phone1)
        {
            Common.Common.GetInteger(state);
            using (ISEEEntities context = new ISEEEntities())
            {
                int factoryId = ISEE.Common.SessionManegment.SessionManagement.FactoryID;
                bool _Active = true;

                var custData = dataContext.Customers.Include("Building")
                    .Include("Building.Street")
                    .Include("Building.Street.City")
                    .Include("Building.Street.City.State")
                    .ToList().Where(x => x.Factory == factoryId
                       && (Common.Common.GetInteger(state) != 0 ? x.Building.StateCode == Common.Common.GetInteger(state) : true)
                   && x.Building.StateCode == (Common.Common.GetInteger(state) == 0 ? x.Building.StateCode : Common.Common.GetInteger(state))
                    && x.Building.CityCode == (Common.Common.GetInteger(city) == 0 ? x.Building.CityCode : Common.Common.GetInteger(city))
                    && x.Building.StreetCode == (Common.Common.GetInteger(street) == 0 ? x.Building.StreetCode : Common.Common.GetInteger(street))
                       && x.Building.Number.Contains(string.IsNullOrEmpty(buldingNumber) ? x.Building.Number : buldingNumber)
                      && x.CustomerNumber.CompareTo(string.IsNullOrEmpty(customerNumber) ? x.CustomerNumber : customerNumber) == 0
                       && x.FirstName.Contains(string.IsNullOrEmpty(contactName) ? x.FirstName : contactName)
                       && (string.IsNullOrEmpty(x.Phone1) || x.Phone1.Contains(string.IsNullOrEmpty(phone1) ? x.Phone1 : phone1))
                      && (_Active ? (x.EndDate == null || (x.EndDate != null && x.EndDate >= DateTime.Now)) : (x.EndDate != null && x.EndDate < DateTime.Now))
                  ).Select(c => new { FirstName = c.FirstName ?? string.Empty, id = c.CustomerId, LastName = c.LastName ?? string.Empty, AreaPhone1 = c.AreaPhone1 ?? string.Empty, Phone1 = c.Phone1 ?? string.Empty }).ToList();
                return new JsonResult { Data = custData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }





        public ActionResult _Category()
        {
            return PartialView();
        }
        public JsonResult GetEmployeeHours()
        {
            int factoryId = ISEE.Common.SessionManegment.SessionManagement.FactoryID;
            using (ISEEEntities context = new ISEEEntities())
            {
                var EmpHours = context.FactoryDairyTemplets.Where(s => s.Factory == factoryId).ToList().Select(e => new { Day = ((Days)Enum.ToObject(typeof(Days), e.DayStatus)).ToString(), Start1 = e.Start1 != null ? Convert.ToDateTime(e.Start1.Value.ToString()).ToString("hh:mm tt") : null, End1 = e.Stop1 != null ? Convert.ToDateTime(e.Stop1.Value.ToString()).ToString("hh:mm tt") : null, Start2 = e.Start2 != null ? Convert.ToDateTime(e.Start2.Value.ToString()).ToString("hh:mm tt") : null, End2 = e.Stop2 != null ? Convert.ToDateTime(e.Stop2.Value.ToString()).ToString("hh:mm tt") : null }).ToList();
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
                        factoryDairyTemplet.DayStatus = (int)((Days)Enum.Parse(typeof(Days), item.Day));
                        factoryDairyTemplet.EmployeeId = employeeID;
                        factoryDairyTemplet.Start1 = Convert.ToDateTime(item.Start1).ToShortTimeString() != null ? (new TimeSpan(Int32.Parse(Convert.ToDateTime(item.Start1).ToShortTimeString().Split(':')[0]), Int32.Parse((Convert.ToDateTime(item.Start1).ToShortTimeString().Split(':')[1]).Split(' ')[0]), 0)) : new TimeSpan(0);
                        factoryDairyTemplet.Stop1 = Convert.ToDateTime(item.End1).ToShortTimeString() != null ? (new TimeSpan(Int32.Parse(Convert.ToDateTime(item.End1).ToShortTimeString().Split(':')[0]), Int32.Parse((Convert.ToDateTime(item.End1).ToShortTimeString().Split(':')[1]).Split(' ')[0]), 0)) : new TimeSpan(0);
                        factoryDairyTemplet.Start2 = Convert.ToDateTime(item.Start2).ToShortTimeString() != null ? (new TimeSpan(Int32.Parse(Convert.ToDateTime(item.Start2).ToShortTimeString().Split(':')[0]), Int32.Parse((Convert.ToDateTime(item.Start2).ToShortTimeString().Split(':')[1]).Split(' ')[0]), 0)) : new TimeSpan();
                        factoryDairyTemplet.Stop2 = Convert.ToDateTime(item.End2).ToShortTimeString() != null ? (new TimeSpan(Int32.Parse(Convert.ToDateTime(item.End2).ToShortTimeString().Split(':')[0]), Int32.Parse((Convert.ToDateTime(item.End2).ToShortTimeString().Split(':')[1]).Split(' ')[0]), 0)) : new TimeSpan(0);
                        factoryDairyTemplet.OrderDay = (int)((Days)Enum.Parse(typeof(Days), item.Day));
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
                    emp.StartDay = startDay != "" ? Convert.ToDateTime(startDay).Date : emp.StartDay;
                    emp.EndDay = enddate != "" ? Convert.ToDateTime(enddate).Date : emp.EndDay;
                    emp.PhoneManufactory = Convert.ToInt32(ManufactureChoice);
                    emp.PhoneType = Convert.ToInt32(phoneTypeChoice != "" ? phoneTypeChoice : "1");
                    emp.SecondPhone = phone2;
                    emp.SecondAreaPhone = phone22;
                    emp.MainAreaPhone = phone11;
                    emp.MainPhone = phone1;
                    emp.EmployeeKey = Guid.NewGuid();
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
            int FactoryId = ISEE.Common.SessionManegment.SessionManagement.FactoryID;
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
        public JsonResult insertAddress(int? stateID, int? cityID, int? streetID)
        {
            using (ISEEEntities context = new ISEEEntities())
            {
                int FactoryId = ISEE.Common.SessionManegment.SessionManagement.FactoryID;
                int countryID = 2;
                var result = context.Buildings.Where(x => x.CountryCode == countryID
                    && x.StateCode == (stateID == null ? x.StateCode : stateID)
                    && x.StreetCode == (streetID == null ? x.StreetCode : streetID)
                    && x.CityCode == (cityID == null ? x.CityCode : cityID)).Select(s => new { CountryName = s.Street.City.State.Country.CountryDesc, StateName = s.Street.City.State.StateDesc, CityName = s.Street.City.CityDesc, StreetName = s.Street.StreetDesc, BuldingNumber = s.Number, Lat = s.Lat, Long = s.Long, Number = s.Number }).ToList();
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public ActionResult SaveCustomerForm(CustomerDataModel objCustomerData)
        {
            string errorMessage = string.Empty;
            try
            {
                int FactoryId = ISEE.Common.SessionManegment.SessionManagement.FactoryID;
                var CustomerData = objCustomerData;

                using (ISEEEntities context = new ISEEEntities())
                {
                    Customer customer = new Customer();
                    customer.CreateDate = DateTime.Now;
                    customer.BuildingCode = objCustomerData.BuldingCode;
                    customer.CustomerNumber = objCustomerData.CustomerNumber;
                    customer.Factory = FactoryId;
                    customer.FirstName = objCustomerData.ContactName;
                    customer.Floor = objCustomerData.Floor;
                    customer.Apartment = objCustomerData.Apartment;
                    customer.AreaPhone1 = objCustomerData.PhoneArea1;
                    customer.Phone1 = objCustomerData.Phone1;
                    customer.AreaPhone2 = objCustomerData.PhoneArea2;
                    customer.Phone2 = objCustomerData.Phone2;
                    customer.Fax = objCustomerData.Fax;
                    customer.Mail = objCustomerData.Mail;
                    customer.VisitInterval = objCustomerData.VisitInterval;
                    customer.NextVisit = objCustomerData.NextVisit;
                    customer.VisitTime = objCustomerData.VisitTime;
                    context.Customers.Add(customer);
                    context.SaveChanges();
                    var customerDetails = new { CustomerID = customer.CustomerId, LastName = customer.LastName, FirstName = customer.FirstName, AreaPhone1 = customer.AreaPhone1, Phone1 = customer.Phone1 };
                    return new JsonResult { Data = new { Message = "Success", CustomerDetails = customerDetails }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }

            }
            catch (Exception ex)
            {
                errorMessage = ex.InnerException.Message;
            }
            return new JsonResult { Data = new { Message = "Error", ErrorDetails = errorMessage }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        #region Category Tab
        #region Category Level


        public JsonResult getAll(string id)
        {
            using (ISEEEntities dataContext = new ISEEEntities())
            {
                if (string.IsNullOrEmpty(id))
                {
                    var factoryLevel1list = dataContext.RequsetToFactoryLevel1.Where(d => d.Factory == SessionManegment.SessionManagement.FactoryID).OrderBy(c => c.RequsetOrder).Select(x => new { x.RequestSysIdLevel1, x.RequestDescCodeLevel1, x.RequsetOrder, x.StatusCode, x.Factory }).ToList();
                    return Json(factoryLevel1list, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var factoryLevel1list = dataContext.RequsetToFactoryLevel1.ToList().Where(d => d.Factory == SessionManegment.SessionManagement.FactoryID && d.StatusCode == Common.Common.GetInteger(id)).OrderBy(c => c.RequsetOrder).Select(x => new { x.RequestSysIdLevel1, x.RequestDescCodeLevel1, x.RequsetOrder, x.StatusCode, x.Factory }).ToList();
                    return Json(factoryLevel1list, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public JsonResult GetSecondary(Int32 sysIdLevel1, int? ContactStatus)
        {

            using (ISEEEntities dataContext = new ISEEEntities())
            {
                var factoryLevel2list = dataContext.RequsetToFactoryLevel2.Where(d => d.RequestSysIdLevel1 == sysIdLevel1).Select(x => new { x.RequestDescCodeLevel2, x.RequsetOrder, x.RequestSysIdLevel1, x.RequestSysIdLevel2, x.StatusCode }).ToList();
                if (ContactStatus.HasValue)
                {
                    factoryLevel2list = factoryLevel2list.Where(c => c.StatusCode == ContactStatus).ToList();
                }
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

        public ActionResult _AdminTree()
        {
            //SessionManegment.SessionManagement.FactoryID = 1;
            //SessionManegment.SessionManagement.FactoryDesc = "Demo Company";
            List<TreeView> data = dataContext.TreeViews.Where(tt => tt.FactoryID == SessionManegment.SessionManagement.FactoryID && tt.ParentID == null).ToList();

            var serializer = new JavaScriptSerializer();
            ViewBag.JsonData = serializer.Serialize(CreateJsonTree(data));

            return PartialView();
        }

        public static List<TreeNodeData> CreateJsonTree(List<TreeView> data)
        {
            List<TreeNodeData> treeList = new List<TreeNodeData>();
            if (data.Count == 0)
            {
                treeList.Add(new TreeNodeData() { id = -100, text = SessionManegment.SessionManagement.FactoryDesc, textCss = "customnode", objecttype = "companyNode" });
            }
            TreeNodeData parentTreeNode = new TreeNodeData();
            CreateTreeNodes(data, ref treeList, ref parentTreeNode, false);

            return treeList;
        }

        private static void CreateTreeNodes(List<TreeView> data, ref List<TreeNodeData> treeList, ref TreeNodeData parentTreeNode, bool hasChildren = false)
        {
            TreeNodeData objTreeNodeData;
            foreach (var objTreeView in data)
            {
                if (objTreeView.EmployeeID != null)
                {
                    var emp = dataContext.Employees.Where(x => x.EmployeeId == objTreeView.EmployeeID).FirstOrDefault();
                    objTreeNodeData = new TreeNodeData() { id = objTreeView.ID, text = emp.LastName + " " + emp.FirstName, objectid = objTreeView.EmployeeID, textCss = "employeeTitle", objecttype = "employee", iconUrl = "/images/img/employee_16.png" };
                }
                else if (objTreeView.CustomerID != null)
                {
                    var cust = dataContext.Customers.Where(x => x.CustomerId == objTreeView.CustomerID).FirstOrDefault();
                    objTreeNodeData = new TreeNodeData() { id = objTreeView.ID, text = cust.LastName + " " + cust.FirstName, objectid = objTreeView.CustomerID, textCss = "customerTitle", objecttype = "customer", iconUrl = "/images/img/customer_16.png" };
                }
                else
                    objTreeNodeData = new TreeNodeData() { id = objTreeView.ID, text = objTreeView.Description, textCss = "customnode", objecttype = objTreeView.ParentID == null ? "companyNode" : "branchNode" };
                if (hasChildren)
                {
                    if (parentTreeNode.children == null)
                    {
                        parentTreeNode.children = new List<TreeNodeData>();
                    }
                    parentTreeNode.children.Add(objTreeNodeData);
                }
                else
                {
                    treeList.Add(objTreeNodeData);
                }

                if (objTreeView.TreeView1.Any())
                {
                    CreateTreeNodes(objTreeView.TreeView1.ToList(), ref  treeList, ref objTreeNodeData, true);
                }
            }
        }

        private void DeleteNodes(List<TreeNodeData> treeNodeList)
        {
            List<long> _idsList = new List<long>();
            GetTreeIds(treeNodeList, ref _idsList);
            var deleteNodes = dataContext.TreeViews.Where(c => !_idsList.Contains(c.ID) && c.ID > 0).ToList();
            for (int i = deleteNodes.Count; i > 0; i--)
            {
                dataContext.TreeViews.Remove(deleteNodes[i - 1]);
            }
            dataContext.SaveChanges();
        }

        private void GetTreeIds(List<TreeNodeData> treeNodeList, ref  List<long> _idsList)
        {
            foreach (var item in treeNodeList)
            {
                _idsList.Add(item.id);
                if (item.children != null && item.children.Any())
                {
                    GetTreeIds(item.children, ref _idsList);
                }
            }
        }

        public JsonResult SaveTreeViewData(string treeViewData)
        {
            var treeNodeList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TreeNodeData>>(treeViewData);
            try
            {
                DeleteNodes(treeNodeList);

                SaveProcess(treeNodeList, null);
                dataContext.SaveChanges();

                List<TreeView> data = dataContext.TreeViews.Where(tt => tt.FactoryID == SessionManegment.SessionManagement.FactoryID && tt.ParentID == null).ToList();

                var serializer = new JavaScriptSerializer();
                var jsonString = serializer.Serialize(CreateJsonTree(data));

                return new JsonResult { Data = new { Message = "Success", NewTreeJson = jsonString }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = new { Message = "Error", ErrorDetails = ex.InnerException }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


            }

        }

        private void SaveProcess(List<TreeNodeData> treeNodeList, long? objParentNodeID)
        {
            TreeView objNode;
            foreach (var objTreeNode in treeNodeList)
            {
                objNode = dataContext.TreeViews.FirstOrDefault(c => c.ID == objTreeNode.id);
                if (objNode == null)
                {
                    objNode = new TreeView();
                    objNode.Description = objTreeNode.text;
                    objNode.FactoryID = SessionManegment.SessionManagement.FactoryID;
                    if (!string.IsNullOrEmpty(objTreeNode.objecttype) && objTreeNode.objecttype == "employee" && objTreeNode.objectid != null)
                        objNode.EmployeeID = objTreeNode.objectid;
                    else if (!string.IsNullOrEmpty(objTreeNode.objecttype) && objTreeNode.objecttype == "customer" && objTreeNode.objectid != null)
                        objNode.CustomerID = objTreeNode.objectid;

                    if (objParentNodeID.HasValue)
                    {
                        objNode.ParentID = objParentNodeID;
                    }
                    dataContext.TreeViews.Add(objNode);
                    dataContext.SaveChanges();
                }
                else
                {
                    objNode.Description = objTreeNode.text;
                }
                if (objTreeNode.children != null && objTreeNode.children.Any())
                {
                    SaveProcess(objTreeNode.children, objNode.ID);
                }
            }
        }

        #endregion
        //Customer Tab
        public JsonResult GetStaresByFactoryID()
        {
            using (ISEEEntities context = new ISEEEntities())
            {
                //int FactoryId = ISEE.Common.SessionManegment.SessionManagement.FactoryID;
                //var CountryID = context.FactoryParms.Where(F => F.FactoryId == FactoryId).Select(s => new { CountryID = s.Country }).FirstOrDefault();

                int FactoryId = ISEE.Common.SessionManegment.SessionManagement.FactoryID;
                var CountryID = ISEE.Common.SessionManegment.SessionManagement.Country;


                var StateDec = context.States.Where(c => c.CountryCode == FactoryId && c.CountryCode == CountryID).Select(x => new { CountryCode = x.StateCode, CountryDescEng = x.StateDesc }).ToList();
                return new JsonResult { Data = StateDec, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        public JsonResult GetCitysByState(int stateID)
        {
            using (ISEEEntities context = new ISEEEntities())
            {

                var Cityes = context.Cities.Where(p => p.StateCode == stateID).Select(d => new { CityCode = d.CityCode, CityDesc = d.CityDesc }).ToList();
                return new JsonResult { Data = Cityes, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        public JsonResult GetStreetByCity(int stateID, int cityID)
        {
            using (ISEEEntities context = new ISEEEntities())
            {
                if (cityID != null)
                {
                    var Streets = context.Streets.Where(p => p.CityCode == cityID && p.StateCode == stateID).Select(d => new { StreetCode = d.StreetCode, Streetdesc = d.StreetDesc }).ToList();
                    return new JsonResult { Data = Streets, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public JsonResult GetBuildingsByCity(int streetID, int stateID, int cityID)
        {
            using (ISEEEntities context = new ISEEEntities())
            {
                if (streetID != null)
                {
                    var Buildings = context.Buildings.Where(p => p.StreetCode == streetID && p.StateCode == stateID && p.CityCode == cityID).Select(d => new { BuildingCode = d.BuildingCode, BuildingLat = d.Lat, BuldingLong = d.Long, BuildingNumber = d.Number }).ToList();
                    return new JsonResult { Data = Buildings, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public JsonResult GetEmployeeByEmployeeID(int EmployeeID)
        {
            if (EmployeeID != null)
            {
                using (ISEEEntities context = new ISEEEntities())
                {
                    var result = context.Employees.Where(e => e.EmployeeId == EmployeeID).Select(q => new { employeeID = q.EmployeeId, FirstName = q.FirstName, LastName = q.LastName, MainAreaPhone = q.MainAreaPhone, MainPhone = q.MainPhone }).FirstOrDefault();
                    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


    }
}