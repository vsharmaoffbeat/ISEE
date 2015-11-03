using ISEEDataModel.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        public ActionResult Admin()
        {
            if (ISEE.Common.SessionManegment.SessionManagement.FactoryID == 0)
                return RedirectToAction("login", "login");
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
            return View();
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
                var factoryLevel2list = dataContext.RequsetToFactoryLevel2.Where(d => d.RequestSysIdLevel1 == sysIdLevel1).Select(x => new { x.RequestDescCodeLevel2, x.RequsetOrder, x.RequestSysIdLevel1, x.RequestSysIdLevel2 ,x.StatusCode}).ToList();
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


    }
}