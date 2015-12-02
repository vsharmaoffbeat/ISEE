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
                case "EmployeeSms":
                    //        var companyLogo = ReportParameters["CompanyLogo"].Value;

                    //if (companyLogo != null)
                    //{
                    //    pictureBox2.Value = "http://isee1.blob.core.windows.net/logo/" + companyLogo;
                    //    pictureBox2.Visible = true;

                    //}
                    //else
                    //{ pictureBox1.Visible = true; }

                    //string sql = @"SELECT  RepoetStringLabelPk, MainString, EN, HE, RU, ES, DE FROM  RepoetStringLabel";
                    //string connectionString = ConfigurationManager.ConnectionStrings["ReportLibrary.Properties.Settings.iSEE_report"].ConnectionString;
                    //var value = ReportParameters["lg"].Value;
                    //var lang = "EN";//default lang

                    //if (value != null)
                    //{
                    //    lang = value.ToString();
                    //}

                    //SqlDataAdapter adapter = new SqlDataAdapter(sql, connectionString);
                    //DataTable datatable = new DataTable();
                    //adapter.Fill(datatable);

                    //ReportItemBase[] allTextBoxes = this.Items.Find(typeof(Telerik.Reporting.TextBox), true);
                    //foreach (Telerik.Reporting.TextBox textBox in allTextBoxes)
                    //{

                    //    DataRow[] result = datatable.Select("MainString  = '" + textBox.Name + "'");
                    //    if (result.Length > 0) textBox.Value = result[0][lang].ToString();

                    //}

                    //string commaseparatedValue = ReportParameters["EmployeeList"].Value.ToString();
                    //string FactoryGuid = (string)ReportParameters["Factory"].Value;

                    //SqlDataSource sqlDataSource = new SqlDataSource
                    //{
                    //    ConnectionString = connectionString,
                    //    SelectCommand = GetSql(commaseparatedValue)
                    //};
                    //sqlDataSource.Parameters.Add("@Factory", DbType.Int32, GetFactoryId(FactoryGuid));
                    //sqlDataSource.Parameters.Add("@FromDate", DbType.DateTime, ReportParameters["FromDate"].Value);
                    //sqlDataSource.Parameters.Add("@ToDate", DbType.DateTime, ReportParameters["ToDate"].Value);



                    //  reportViewer.LocalReport.DataSources.Add(new ReportDataSource("dsLocalReport", dataSet.Tables["SampleTable"]));
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

        public class ClsEmployee
        {
            public int Id { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string CustomerNumber { get; set; }
        }


    }

    public class ReportSearchParams
    {
        public string ReportName { get; set; }
        public string FilterSearch { get; set; }
    }
}