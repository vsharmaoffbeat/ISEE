using ISEE.Common;
using ISEEDataModel.Repository;
using ISEEREGION.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISEEREGION.Controllers
{
    public class DataController : Controller
    {
        //
        // GET: /Data/
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult UserLogin(LoginViewModel data)
        {
            using (ISEEEntities context = new ISEEEntities())
            {
                var query = from f in context.Factories.ToList()
                            join fp in context.FactoryParms on
                             f.FactoryId equals fp.FactoryId
                            join fd in context.FactoryDistances on
                            f.FactoryId equals fd.FactoryId
                            join country in context.Countries on
                            fp.Country equals country.CountryCode

                            where (String.Compare(f.UserName, data.UserName, StringComparison.OrdinalIgnoreCase) == 0) &&
                            (String.CompareOrdinal(f.Password, data.Password) == 0)

                            select new
                            {
                                FactoryGuid = (Guid)f.FactoryKey,
                                FactoryDesc = f.FactoryDesc,
                                FactoryID = f.FactoryId,
                                Country = fp.Country,
                                CountryDesc = country.CountryDesc,
                                StopEmployeeTime = (int)fp.StopEmployeeTime,
                                SlipTime = fp.SplitTime,
                                Lat = (double)fp.Lat,
                                Long = (double)fp.Long,
                                Zoom = (int)fp.Zoom,
                                MapProvider = (int)fp.MapProvider,
                                SmsProvider = (int)fp.SmsProvider,
                                PhoneAreaCode = fp.PhoneAreaCode,
                                CompanyLogo = fp.CompanyLogo,
                                CurrentGmt = country.CurrentGmt,
                                MapSearchModule = fd.MapSearchModule,
                                RadiusSearch = fd.RadiusSearch,
                                TopEmployeeSearch = fd.TopEmployeeSearch,
                                ZoomSearchLevel = fd.ZoomSearchLevel,
                                CalendarShowRadius = fd.CalendarShowRadius,
                                CalenderShowZoomLevel = fd.CalenderShowZoomLevel
                            };





                var loginData = query.ToList()[0];
                SessionManegment.SessionManagement.FactoryID = loginData.FactoryID;

                SessionManegment.SessionManagement.FactoryDesc = loginData.FactoryDesc;
                SessionManegment.SessionManagement.CountryDesc = loginData.CountryDesc;
                SessionManegment.SessionManagement.FactoryID = loginData.FactoryID;
                SessionManegment.SessionManagement.Country = loginData.Country;
                //SessionManegment.SessionManagement. = loginData.;
                //SessionManegment.SessionManagement. = loginData.;
                //SessionManegment.SessionManagement. = loginData.;
                //SessionManegment.SessionManagement. = loginData.;
                //SessionManegment.SessionManagement. = loginData.;


            }


            //using (ISEEEntities context = new ISEEEntities())
            //{
            //    var user = context.Factories.Where(d => d.UserName == data.UserName && d.Password == data.Password).Select(x => x.FactoryId).FirstOrDefault();
            //}
            return new JsonResult { Data = 1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


    }
}
