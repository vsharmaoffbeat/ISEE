using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISEE.Controllers
{
    public class ReportsController : Controller
    {
        //
        // GET: /Reports/
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult Reports(string reportName)
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
            if (string.IsNullOrEmpty(reportName))
            {
                reportName = "Empty";
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

            switch (reportName)
            {
                case "EmployeeSms":

                    //  reportViewer.LocalReport.DataSources.Add(new ReportDataSource("dsLocalReport", dataSet.Tables["SampleTable"]));
                    break;
            }

            #endregion

            ViewBag.ReportViewer = reportViewer;
            return View();
        }



    }
}