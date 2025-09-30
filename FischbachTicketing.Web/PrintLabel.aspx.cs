using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Drawing;
using FischbachTicketing.BusinessEntities;
using System.Configuration;
using System.Drawing.Printing;

namespace FischbachTicketing.Web
{
    public partial class PrintLabel : System.Web.UI.Page
    {
        private TableLogOnInfo crTableLogoninfo;
        private object crtableLogoninfo;
        private ReportDocument cryRpt = new ReportDocument();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] != null)
            {
                User user = (User)Session["User"];
                ltrlUserFullName.Text = user.UserName;
                if (Page.IsPostBack == false)
                {
                    Page.Title = ConfigurationManager.AppSettings["COMPANYNAME"].ToString() + " :: " + ConfigurationManager.AppSettings["SYSTEMNAME"].ToString();
                    ltrlSystemName.Text = ConfigurationManager.AppSettings["SYSTEMNAME"].ToString();
                }
                GetPrintTicketData();
            }
            else
            {
                Response.Redirect("Login");
            }
        }

        private void GetPrintTicketData()
        {
            try
            {
                Ticket ticket = (Ticket)Session["TICKET"];
                string inkRoomPrinterName = ConfigurationManager.AppSettings["PRINTER_INKROOM"].ToString();
                string screenRoomPrinterName = ConfigurationManager.AppSettings["PRINTER_SCREENROOM"].ToString();

                string reportConnectionString = ConfigurationManager.AppSettings["OracleDB"].ToString();
                string masterUserName = ConfigurationManager.AppSettings["MASTERUSERNAME"];
                string masterPassword = ConfigurationManager.AppSettings["MASTERPASSWORD"];

                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                string[] aConnectionString = reportConnectionString.Split(';');
                crConnectionInfo.ServerName = aConnectionString[0].Replace("DATA SOURCE=", "");
                crConnectionInfo.DatabaseName = "";
                crConnectionInfo.UserID = masterUserName;
                crConnectionInfo.Password = masterPassword;
                crConnectionInfo.IntegratedSecurity = false;

                string reportName = ConfigurationManager.AppSettings["TICKET_REPORT"].ToString();
                cryRpt.Load(Server.MapPath("Reports/" + reportName));

                foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in cryRpt.Database.Tables)
                {
                    crTableLogoninfo = CrTable.LogOnInfo;
                    crTableLogoninfo.ConnectionInfo = crConnectionInfo;
                    CrTable.ApplyLogOnInfo(crTableLogoninfo);
                }

                foreach (ReportDocument subReport in cryRpt.Subreports)
                {
                    foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in subReport.Database.Tables)
                    {
                        crTableLogoninfo = CrTable.LogOnInfo;
                        crTableLogoninfo.ConnectionInfo = crConnectionInfo;
                        CrTable.ApplyLogOnInfo(crTableLogoninfo);
                    }                    
                }

                CrystalDecisions.Shared.ParameterFields parameterFields = new CrystalDecisions.Shared.ParameterFields();
                CrystalDecisions.Shared.ParameterField param1 = new CrystalDecisions.Shared.ParameterField();
                CrystalDecisions.Shared.ParameterDiscreteValue paramValue = new CrystalDecisions.Shared.ParameterDiscreteValue();

                //paramValue.Value = ticket.ID;
                //param1.ParameterFieldName = "CRMACTIVITY_ID";
                //param1.CurrentValues.Add(paramValue);
                //parameterFields.Add(param1);                

                #region Direct Print from Crystal Report
                cryRpt.PrintOptions.PaperOrientation = PaperOrientation.Portrait;
                cryRpt.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;

                CrystalDecisions.Shared.PageMargins objPageMargins;
                objPageMargins = cryRpt.PrintOptions.PageMargins;
                objPageMargins.bottomMargin = 0;
                objPageMargins.leftMargin = 0;
                objPageMargins.rightMargin = 0;
                objPageMargins.topMargin = 0;
                cryRpt.PrintOptions.ApplyPageMargins(objPageMargins);
                PageSettings pageSettings = new PageSettings();
                pageSettings.PaperSize = new System.Drawing.Printing.PaperSize("newPaperSize", 350, 350);
                PrinterSettings printerSettings = new PrinterSettings();

                if(ticket.ORDER_TYPE.Equals("OI"))
                {
                    printerSettings.PrinterName = inkRoomPrinterName;
                }
                else
                {
                    printerSettings.PrinterName = screenRoomPrinterName;
                }

                cryRpt.SetParameterValue("CRM ACTIVITY ID", ticket.ID);

                foreach (ReportDocument subReport in cryRpt.Subreports)
                {
                    cryRpt.SetParameterValue("CRM ACTIVITY ID", ticket.ID);
                }

                cryRpt.PrintToPrinter(printerSettings, pageSettings, false);
                #endregion

            }
            catch (Exception exp)
            {
                
            }
        }
    }
}