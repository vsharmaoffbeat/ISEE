using ISEE.Common;
using ISEE.Models;
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
    public class AdminController : Controller
    {
        ISEEFactory _facory = new ISEEFactory();
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
                int factoryId = SessionManagement.FactoryID;
                bool _Active = true;
                var empData = dataContext.Employees.Where(x => x.Factory == factoryId
                                                         && x.FirstName.Contains(firstname == null ? x.FirstName : firstname)
                                                         && (string.IsNullOrEmpty(lastname) || x.LastName.Contains(lastname))
                                                         && x.EmployeeNum.Contains(phone == null ? x.EmployeeNum : phone)
                                                         && (_Active == true ? (x.EndDay == null || (x.EndDay != null && x.EndDay >= DateTime.Now)) : (x.EndDay != null && x.EndDay < DateTime.Now))).OrderBy(x => x.EmployeeNum).Select(x => new { FirstName = x.FirstName, LastName = x.LastName, MainPhone = x.MainPhone, id = x.EmployeeId, x.MainAreaPhone }).ToList();
                return new JsonResult { Data = empData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        public JsonResult GetCustomersNew(int state, int city, int street, string buldingNumber, string customerNumber, string contactName, string companyName, string phoneArea, string phone1)
        {
            int factoryId = SessionManagement.FactoryID;
            bool _Active = true;
            var custData = _facory.GetCustomersNew(factoryId, state, city, street, buldingNumber, customerNumber, companyName, contactName, phoneArea, phone1, _Active).Select(c => new
                       {
                           FirstName = c.FirstName ?? string.Empty,
                           id = c.CustomerId,
                           LastName = c.LastName ?? string.Empty,
                           AreaPhone1 = c.AreaPhone1 ?? string.Empty,
                           Phone1 = c.Phone1 ?? string.Empty
                       }).ToList();
            return new JsonResult { Data = custData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }





        public ActionResult _Category()
        {
            return PartialView();
        }
        public JsonResult GetEmployeeHours()
        {
            int factoryId = SessionManagement.FactoryID;
            using (ISEEEntities context = new ISEEEntities())
            {
                var EmpHours = context.FactoryDairyTemplets.Where(s => s.Factory == factoryId).ToList().Select(e => new { Day = ((Days)Enum.ToObject(typeof(Days), e.DayStatus)).ToString(), Start1 = e.Start1 != null ? Convert.ToDateTime(e.Start1.Value.ToString()).ToString("hh:mm tt") : null, End1 = e.Stop1 != null ? Convert.ToDateTime(e.Stop1.Value.ToString()).ToString("hh:mm tt") : null, Start2 = e.Start2 != null ? Convert.ToDateTime(e.Start2.Value.ToString()).ToString("hh:mm tt") : null, End2 = e.Stop2 != null ? Convert.ToDateTime(e.Stop2.Value.ToString()).ToString("hh:mm tt") : null }).ToList();
                return new JsonResult { Data = EmpHours, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        public bool SaveEmployeeHours(string objhours, int employeeID)
        {
            int factoryId = SessionManagement.FactoryID;
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
                            factoryDairyTemplet.Start1 = Common.Common.GetTimeSpan(item.Start1);
                            factoryDairyTemplet.Stop1 = Common.Common.GetTimeSpan(item.End1)  ;
                            factoryDairyTemplet.Start2 = Common.Common.GetTimeSpan(item.Start2) ;
                            factoryDairyTemplet.Stop2 = Common.Common.GetTimeSpan(item.End2)  ;
                        factoryDairyTemplet.OrderDay = (int)((Days)Enum.Parse(typeof(Days), item.Day));
                        context.EmployeeDiaryTemplates.Add(factoryDairyTemplet);
                        context.SaveChanges();
                    }
                    return true;
                }

            }
            return false;
        }

        public int SaveEmployeeData(string number, string firstName, string lastName, string startDay, string enddate, string phone1, string phone11, string phone2, string phone22, string ManufactureChoice, string phoneTypeChoice, string mail)
        {
            int factoryId = SessionManagement.FactoryID;
            int EmployeeID = 0;
            using (ISEEEntities context = new ISEEEntities())
            {
                try
                {
                    Employee emp = new Employee();
                    emp.FirstName =Common.Common.GetNullableValues(firstName);
                    emp.LastName = Common.Common.GetNullableValues(lastName);
                    emp.EmployeeNum =Common.Common.GetNullableValues( number);
                    emp.SysCreatDate = DateTime.Now;
                    emp.Factory = factoryId;
                    if (string.IsNullOrEmpty(startDay))
                        emp.StartDay = DateTime.Now;
                    else
                    emp.StartDay = startDay != "" ? Convert.ToDateTime(startDay).Date : emp.StartDay;
                    emp.EndDay = enddate != "" ? Convert.ToDateTime(enddate).Date : emp.EndDay;
                    emp.PhoneManufactory = Convert.ToInt32(ManufactureChoice);
                    emp.PhoneType = Convert.ToInt32(phoneTypeChoice != "" ? phoneTypeChoice : "1");
                    emp.SecondPhone = Common.Common.GetNullableValues(phone22);
                    emp.SecondAreaPhone = Common.Common.GetNullableValues(phone2);
                    emp.MainAreaPhone = Common.Common.GetNullableValues(phone1);
                    emp.Mail = Common.Common.GetNullableValues(mail);
                    emp.MainPhone = Common.Common.GetNullableValues(phone11);
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

    

        public ActionResult SaveCustomerForm(CustomerDataModel objCustomerData)
        {
            string errorMessage = string.Empty;
            try
            {
                int FactoryId = SessionManagement.FactoryID;
                var CustomerData = objCustomerData;

                using (ISEEEntities context = new ISEEEntities())
                {
                    Customer customer = new Customer();
                    customer.CreateDate = DateTime.Now;
                    customer.BuildingCode = objCustomerData.BuldingCode;
                    customer.CustomerNumber = objCustomerData.CustomerNumber;
                    customer.Factory = FactoryId;
                    customer.FirstName = objCustomerData.CompanyName;
                    customer.LastName = objCustomerData.ContactName;
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
                    var factoryLevel1list = dataContext.RequsetToFactoryLevel1.Where(d => d.Factory == SessionManagement.FactoryID).OrderBy(c => c.RequsetOrder).Select(x => new { x.RequestSysIdLevel1, x.RequestDescCodeLevel1, x.RequsetOrder, x.StatusCode, x.Factory }).ToList();
                    return Json(factoryLevel1list, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var factoryLevel1list = dataContext.RequsetToFactoryLevel1.ToList().Where(d => d.Factory == SessionManagement.FactoryID && d.StatusCode == Common.Common.GetInteger(id)).OrderBy(c => c.RequsetOrder).Select(x => new { x.RequestSysIdLevel1, x.RequestDescCodeLevel1, x.RequsetOrder, x.StatusCode, x.Factory }).ToList();
                    return Json(factoryLevel1list, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public JsonResult GetSecondary(Int32 sysIdLevel1, int? ContactStatus)
        {

            using (ISEEEntities dataContext = new ISEEEntities())
            {
                var factoryLevel2list = dataContext.RequsetToFactoryLevel2.Where(d => d.RequestSysIdLevel1 == sysIdLevel1).Select(x => new { x.RequestDescCodeLevel2, x.RequsetOrder, x.RequestSysIdLevel1, x.RequestSysIdLevel2, x.StatusCode }).ToList();
                //if (ContactStatus.HasValue)
                //{
                //    factoryLevel2list = factoryLevel2list.Where(c => c.StatusCode == ContactStatus).ToList();
                //}
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
            List<TreeView> data = dataContext.TreeViews.Where(tt => tt.FactoryID == SessionManagement.FactoryID && tt.ParentID == null).ToList();

            var serializer = new JavaScriptSerializer();
            ViewBag.JsonData = serializer.Serialize(CreateJsonTree(data));

            return PartialView();
        }

        public static List<TreeNodeData> CreateJsonTree(List<TreeView> data)
        {
            List<TreeNodeData> treeList = new List<TreeNodeData>();
            if (data.Count == 0)
            {
                treeList.Add(new TreeNodeData() { id = -100, text = SessionManagement.FactoryDesc, textCss = "customnode", objecttype = "companyNode" });
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

        private void DeleteNodes(List<TreeNodeData> treeNodeList, int factoryId)
        {
            List<long> _idsList = new List<long>();
            GetTreeIds(treeNodeList, ref _idsList);
            var deleteNodes = dataContext.TreeViews.Where(c => !_idsList.Contains(c.ID) && c.ID > 0 && c.FactoryID == factoryId).ToList();
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
                DeleteNodes(treeNodeList, SessionManagement.FactoryID);

                SaveProcess(treeNodeList, null);
                dataContext.SaveChanges();

                List<TreeView> data = dataContext.TreeViews.Where(tt => tt.FactoryID == SessionManagement.FactoryID && tt.ParentID == null).ToList();

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
                    objNode.FactoryID = SessionManagement.FactoryID;
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
    

        public JsonResult GetEmployeeByEmployeeID(int EmployeeID)
        {
            using (ISEEEntities context = new ISEEEntities())
            {
                var result = context.Employees.Where(e => e.EmployeeId == EmployeeID).Select(q => new { employeeID = q.EmployeeId, FirstName = q.FirstName, LastName = q.LastName, MainAreaPhone = q.MainAreaPhone, MainPhone = q.MainPhone }).FirstOrDefault();
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        //To do Data
        public JsonResult GetAddressBuildingCode(int state, string citydesc, int city, int street, string streetdesc, string number, double Lat, double Long, string entry, string zipcode)
        {
            int country = SessionManagement.Country;
            var buildingCode = _facory.GetAddressBuildingCode(country, state, citydesc, city, street, streetdesc, number, Lat, Long, entry, zipcode);
            return new JsonResult { Data = new { BuildingCode = buildingCode }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
    }
}