using ISEE.Common;
using ISEE.Reports;
using ISEE.Reports.ReportDataSetTableAdapters;
using ISEEDataModel.Repository;
using ISEEDataModel.Repository.Services;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ISEE.Controllers
{
    public class ReportsController : Controller
    {
        ISEEFactory _facory = new ISEEFactory();

        //
        // GET: /Reports/
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Reports()
        {
            try
            {

                ReportViewer reportViewer = LoadReport(null);

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception)
            {

                throw;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Reports(ReportSearchParams reportSearchModel)
        {
            try
            {

                ReportViewer reportViewer = LoadReport(reportSearchModel);

                ViewBag.ReportViewer = reportViewer;
            }
            catch (Exception)
            {

                throw;
            }
            return View();
        }

        private ReportViewer LoadReport(ReportSearchParams reportSearchModel)
        {
            #region Server Report
            //ReportViewer reportViewer = new ReportViewer();
            //reportViewer.ProcessingMode = ProcessingMode.Remote;
            //reportViewer.SizeToReportContent = true;
            //reportViewer.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            //reportViewer.Height = System.Web.UI.WebControls.Unit.Percentage(100); 
            //reportViewer.ServerReport.ReportPath = string.Format(ColorTrend.Common.GetAppKeyValue("ReportsPath"), reportName);
            //reportViewer.ServerReport.ReportServerUrl = new Uri(ColorTrend.Common.GetAppKeyValue("ReportServerName"));
            //reportViewer.ServerReport.ReportServerCredentials = new CustomReportCredentials(ColorTrend.Common.GetAppKeyValue("ReportServerUser"), ColorTrend.Common.GetAppKeyValue("ReportServerPassword"), "");

            #endregion
            string reportName = "";
            if (reportSearchModel == null || string.IsNullOrEmpty(reportSearchModel.ReportName))
            {
                reportName = "Empty";
            }
            else
            {
                reportName = reportSearchModel.ReportName;
            }
            #region Local Report
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            reportViewer.Height = System.Web.UI.WebControls.Unit.Percentage(100);

            //TODO: To use our report path
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + string.Format(@"Reports\{0}.rdlc", reportName);
            //TODO: To get datasource as per report;
            reportViewer.LocalReport.DataSources.Clear();
            var value = SessionManagement.CurrentLang;
            var lang = "EN";//default lang

            if (value != null)
            {
                lang = value.ToString();
            }

            //TODO: Globalisation Of Reports

            switch (reportName)
            {
                case "EmployeePresence":
                    ReportDataSet dsEP = new ReportDataSet();
                    usp_GetEmployeePresence1TableAdapter daEP = new usp_GetEmployeePresence1TableAdapter();
                    dsEP.EnforceConstraints = false;
                    daEP.Fill(dsEP.usp_GetEmployeePresence1, reportSearchModel.StartDate, SessionManagement.FactoryID, reportSearchModel.EndDate, reportSearchModel.FilterSearch);
                    List<ReportParameter> parmE = new List<ReportParameter>();
                    DataTable dtEP = dsEP.Tables["usp_GetEmployeePresence1"];
                    ReportDataSource rptDataSourceEP = new ReportDataSource("GetEmployeePresence", dtEP);
                    reportViewer.LocalReport.DataSources.Add(rptDataSourceEP);
                    break;

                case "EmployeeSms":
                    ReportDataSet dsES = new ReportDataSet();
                    usp_GetEmployeeSMSTableAdapter daES = new usp_GetEmployeeSMSTableAdapter();
                    dsES.EnforceConstraints = false;
                    daES.Fill(dsES.usp_GetEmployeeSMS, reportSearchModel.StartDate, SessionManagement.FactoryID, reportSearchModel.EndDate, reportSearchModel.FilterSearch);
                    List<ReportParameter> parmES = new List<ReportParameter>();
                    DataTable dtES = dsES.Tables["usp_GetEmployeeSMS"];
                    ReportDataSource rptDataSourceES = new ReportDataSource("Getsms", dtES);
                    reportViewer.LocalReport.DataSources.Add(rptDataSourceES);
                    break;

                case "ListCustomers":
                    ReportDataSet ds = new ReportDataSet();
                    usp_GetCustomersTableAdapter da = new usp_GetCustomersTableAdapter();
                    da.Fill(ds.usp_GetCustomers, SessionManagement.FactoryID, reportSearchModel.FilterSearch);
                    List<ReportParameter> parm = new List<ReportParameter>();
                    DataTable dt = ds.Tables["usp_GetCustomers"];
                    ReportDataSource rptDataSource = new ReportDataSource("GetCustomers", dt);
                    reportViewer.LocalReport.DataSources.Add(rptDataSource);
                    break;
            }
            reportViewer.LocalReport.Refresh();
            #endregion
            return reportViewer;
        }


        public ActionResult GetCustomers(ReportFilterCriteria model)
        {
            try
            {
                List<ClsEmployee> resultList = new List<ClsEmployee>();
                switch (model.FilterType)
                {
                    case "1":
                        _facory.GetEmployees(SessionManagement.FactoryID, model.LastName, model.FirstName, model.CustomerNumber, 0, 0, model.Active).ToList().ForEach(x => resultList.Add(new ClsEmployee { FirstName = x.FirstName, Id = x.EmployeeId, LastName = x.LastName, CustomerNumber = x.EmployeeNum }));
                        break;
                    default:
                        _facory.GetCustomersNew(SessionManagement.FactoryID, 0, 0, 0, null, model.CustomerNumber, model.FirstName, model.LastName, null, null, model.Active).ToList().ForEach(x => resultList.Add(new ClsEmployee { FirstName = x.FirstName, Id = x.CustomerId, LastName = x.LastName, CustomerNumber = x.CustomerNumber }));
                        break;
                }
                return new JsonResult { Data = new { IsSuccess = true, Customers = resultList }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception)
            {

                return new JsonResult { Data = new { IsSuccess = false, ErrorMessageText = "An Error has been occured...", ErrorMessageBoxTitle = "Message" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }


        public ActionResult GetEmployees(ReportFilterCriteria model)
        {
            try
            {
                List<ClsEmployeeSMS> resultList = new List<ClsEmployeeSMS>();
                switch (model.FilterType)
                {
                    case "1":
                        _facory.GetEmployees(SessionManagement.FactoryID, model.LastName, model.FirstName, model.EmployeeNumber, 0, 0, model.Active).ToList().ForEach(x => resultList.Add(new ClsEmployeeSMS { FirstName = x.FirstName, Id = x.EmployeeId, LastName = x.LastName, EmployeeNumber = x.EmployeeNum }));
                        break;
                    default:
                        _facory.GetEmployees(SessionManagement.FactoryID, model.LastName, model.FirstName, model.EmployeeNumber, 0, 0, model.Active).ToList().ForEach(x => resultList.Add(new ClsEmployeeSMS { FirstName = x.FirstName, Id = x.EmployeeId, LastName = x.LastName, EmployeeNumber = x.EmployeeNum }));
                        break;
                }
                return new JsonResult { Data = new { IsSuccess = true, Employees = resultList }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception)
            {

                return new JsonResult { Data = new { IsSuccess = false, ErrorMessageText = "An Error has been occured...", ErrorMessageBoxTitle = "Message" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public class ClsEmployee
        {
            public int Id { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string CustomerNumber { get; set; }
        }
        public class ClsEmployeeSMS
        {
            public int Id { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string EmployeeNumber { get; set; }
        }


    }

    public class ReportSearchParams
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ReportName { get; set; }
        public string FilterSearch { get; set; }
    }
}