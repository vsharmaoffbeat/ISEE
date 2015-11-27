using ISEE.Common;
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
                    //var companyLogo = ReportParameters["CompanyLogo"].Value;

                    //if (companyLogo != null)
                    //{
                    //    pictureBox2.Value = "http://isee1.blob.core.windows.net/logo/" + companyLogo;
                    //    pictureBox2.Visible = true;

                    //}
                    //else
                    //{ pictureBox1.Visible = true; }

                   // string sql = @"SELECT  RepoetStringLabelPk, MainString, EN, HE, RU, ES, DE FROM  RepoetStringLabel";
                   // string connectionString = ConfigurationManager.ConnectionStrings["ReportsConnectionString"].ConnectionString;


                   // SqlDataAdapter adapter = new SqlDataAdapter(sql, connectionString);
                   // DataTable datatable = new DataTable();
                   // adapter.Fill(datatable);



                   // int factoryID = SessionManagement.FactoryID;
                   //// string commaseparatedValue = ReportParameters["Customers"].Value.ToString();
                   // SqlDataSource sqlDataSource = new SqlDataSource
                   // {
                   //     ConnectionString = connectionString,
                   //     SelectCommand = GetSql(commaseparatedValue)
                   // };
                   // sqlDataSource.Parameters.Add("@Factory", DbType.Int32, factoryID);
                    break;
            }

            #endregion

            ViewBag.ReportViewer = reportViewer;
            return View();
        }

        private string GetSql(string listcustomers)
        {

            string sql =
                @" SELECT Customer.CustomerId, Customer.CustomerNumber, Customer.Factory, Customer.FirstName, Customer.LastName, Customer.Floor, Customer.Apartment, Customer.AreaPhone1,  
                         Customer.Phone1, Customer.AreaPhone2, Customer.Phone2, Customer.AreaCelolar, Customer.Celolar, Customer.AreaFax, Customer.Fax, Customer.Mail, 
                         Customer.VisitInterval, Customer.NextVisit, Country.CountryDesc, City.CityDesc, Street.StreetDesc, Building.Entry, Building.Number
                         FROM            Customer INNER JOIN
                         Building ON Customer.BuildingCode = Building.BuildingCode INNER JOIN
                         Country ON Building.CountryCode = Country.CountryCode INNER JOIN
                         City ON Building.CityCode = City.CityCode INNER JOIN
                         Street ON Building.StreetCode = Street.StreetCode AND City.CityCode = Street.CityCode
                        Where Customer.Factory=@Factory";

            string strParams = string.IsNullOrEmpty(listcustomers) ? string.Empty : " and " + listcustomers;
            return sql + strParams;

        }
     


    }
}