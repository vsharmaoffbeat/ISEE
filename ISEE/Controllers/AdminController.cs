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
            return View();
        }
        public ActionResult _NewCustomer()
        {
            return View();
        }

        public ActionResult _AdminTree()
        {
            List<ISEE.Models.jstreenode> treeList = new List<ISEE.Models.jstreenode>();
            treeList.Add(new ISEE.Models.jstreenode() { id = "ajsontest1", text = "Sample Test Node" });
            treeList.Add(new ISEE.Models.jstreenode() { id = "ajsontest2", text = "Root Node" });
            treeList.Add(new ISEE.Models.jstreenode() { id = "ajsontest3", parent = "ajsontest2", text = "Child Node 1" });
            treeList.Add(new ISEE.Models.jstreenode() { id = "ajsontest4", parent = "ajsontest2", text = "Child Node 2" });
            var serializer = new JavaScriptSerializer();
            ViewBag.JsonData = serializer.Serialize(treeList);
            return PartialView();
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
        public JsonResult GetCustomers(int state, int city, int street, string buldingNumber, string customerNumber, string contactName, string companyName, string phone1)
        //           , int _city, int _street, string _num, string _cusnum, string _FN, string _LN, string _area, string _phone)
        {
            //firstname = string.IsNullOrEmpty(firstname) ? null : firstname;
            //lastname = string.IsNullOrEmpty(lastname) ? null : lastname;
            //phone = string.IsNullOrEmpty(phone) ? null : phone;

            using (ISEEEntities context = new ISEEEntities())
            {

                int factoryId = ISEE.Common.SessionManegment.SessionManagement.FactoryID = 1;
                bool _Active = true;

                var custData = dataCntext.Customers.Include("Building").Include("Building.Street").Include("Building.Street.City").Include("Building.Street.City.State").Where(x => x.Factory == factoryId &&
                    //  (_state != 0 ? x.Building.StateCode == _state : x.Building.StateCode == null) &&
                                                                                                            x.Building.StateCode == (state == 0 ? x.Building.StateCode : state) &&
                                                                                                            x.Building.CityCode == (city == 0 ? x.Building.CityCode : city) &&
                                                                                                            x.Building.StreetCode == (street == 0 ? x.Building.StreetCode : street) &&
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




        public ActionResult _Category()
        {
            return View();
        }
        public bool SaveEmployeeData(string number, string firstName, string lastName, string startDay, string endDay, string phone1, string phone11, string phone2, string phone22, string manufacture, string phoneType)
        {
            using (ISEEEntities context = new ISEEEntities())
            {
                Employee emp = new Employee();
                emp.FirstName = firstName;
                emp.LastName = lastName;
                emp.EmployeeNum = number;

                emp.SysCreatDate = DateTime.Now;
                emp.Factory = 1;
                emp.StartDay = Convert.ToDateTime(startDay).Date;
                emp.EndDay = Convert.ToDateTime(endDay).Date;
                emp.PhoneManufactory = Convert.ToInt32(manufacture);
                emp.PhoneType = Convert.ToInt32(phoneType);
                emp.SecondPhone = phone2;
                emp.SecondAreaPhone = phone22;
                emp.MainAreaPhone = phone11;
                emp.MainPhone = phone1;
                emp.EmployeeKey = new Guid();
                context.Employees.Add(emp);
                context.SaveChanges();
                // var CountryDetail = context.FactoryParms.Select(c => new { FactoryId = c.FactoryId, CountryID = c.Country, Lat = c.Lat, Long = c.Long, Zoom = c.Zoom }).Where(x => x.FactoryId == FactoryId).ToList();
                //ISEEDataModel.Repository.RequsetToFactoryLevel1 objRequsetToFactoryLevel1 = db.RequsetToFactoryLevel1.Where(x => x.RequestSysIdLevel1 == item.RequestSysIdLevel1).FirstOrDefault();
                ////var empQuery = from RequsetToFactoryLevel1 in db.RequsetToFactoryLevel1
                ////              where RequsetToFactoryLevel1.RequestSysIdLevel1 == item.RequestSysIdLevel1
                ////              select RequsetToFactoryLevel1;
                ////ISEEDataModel.Repository.RequsetToFactoryLevel1 objRequsetToFactoryLevel1 = empQuery.Single();
                ////set the new values of the columns (properties), based upon the values entered using the text boxes
                //objRequsetToFactoryLevel1.Factory = item.Factory;
                //objRequsetToFactoryLevel1.RequestDescCodeLevel1 = item.RequestDescCodeLevel1;
                //objRequsetToFactoryLevel1.RequsetOrder = item.RequsetOrder;
                //objRequsetToFactoryLevel1.StatusCode = item.StatusCode;
                //db.SaveChanges();
                //return new JsonResult { Data = CountryDetail, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return true;

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


        public JsonResult getAll()
        {
            using (ISEEEntities dataContext = new ISEEEntities())
            {
                var factoryLevel1list = dataContext.RequsetToFactoryLevel1.Where(d => d.Factory == 1).Select(x => new { x.RequestSysIdLevel1, x.RequestDescCodeLevel1, x.RequsetOrder, x.StatusCode, x.Factory }).OrderBy(x => x.RequestSysIdLevel1).ToList();
                return Json(factoryLevel1list, JsonRequestBehavior.AllowGet);
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