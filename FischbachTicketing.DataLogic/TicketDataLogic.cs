using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntLibContrib.Data.Oracle.ManagedDataAccess;
using System.Configuration;
using FischbachTicketing.Common;
using FischbachTicketing.BusinessEntities;
using System.Data;

namespace FischbachTicketing.DataLogic
{
    public class TicketDataLogic : IDisposable
    {
        private OracleDatabase database;

        public TicketDataLogic()
        {
            string connectionString = ConfigurationManager.AppSettings["OracleDB"];
            string masterUserName = ConfigurationManager.AppSettings["MASTERUSERNAME"];
            string masterPassword = ConfigurationManager.AppSettings["MASTERPASSWORD"];

            connectionString = string.Format(connectionString, masterUserName.ToUpper(), masterPassword);

            database = new OracleDatabase(connectionString);
        }

        public int GetQueueTypeID()
        {
            int queueTypeID = 0;
            DataSet dataSet = this.database.ExecuteDataSet(CommandType.Text, string.Format(@"SELECT ID FROM IQMS.CRM_QUEUE_TYPES WHERE NAME='Ticket'"));

            if (dataSet.Tables[0].Rows.Count > 0)
            {
                queueTypeID = Convert.ToInt32(dataSet.Tables[0].Rows[0]["ID"]);                
                dataSet.Dispose();
            }
            else
            {
                dataSet.Dispose();
            }
            return queueTypeID;
        }

        public int GetUrgentSeverityID()
        {
            int queueTypeID = 0;
            DataSet dataSet = this.database.ExecuteDataSet(CommandType.Text, string.Format(@"SELECT * FROM IQMS.CRM_ACTIVITY_SEVERITY WHERE UPPER(TRIM(DESCRIPTION))='URGENT'"));

            if (dataSet.Tables[0].Rows.Count > 0)
            {
                queueTypeID = Convert.ToInt32(dataSet.Tables[0].Rows[0]["ID"]);
                dataSet.Dispose();
            }
            else
            {
                dataSet.Dispose();
            }
            return queueTypeID;
        }

        public int GetQueueID(int queueTypeID, string orderType)
        {
            int queueID = 0;
            string queueName = string.Empty;
            if(orderType.Equals("OI"))
            {
                queueName = "INK ROOM";
            }

            if (orderType.Equals("OS"))
            {
                queueName = "SCREEN ROOM";
            }
            DataSet dataSet = this.database.ExecuteDataSet(CommandType.Text, string.Format(@"SELECT ID FROM IQMS.CRM_QUEUE WHERE QUEUETYPE_ID={0} AND UPPER(TRIM(NAME))='{1}'", queueTypeID, queueName));

            if (dataSet.Tables[0].Rows.Count > 0)
            {
                queueID = Convert.ToInt32(dataSet.Tables[0].Rows[0]["ID"]);
                dataSet.Dispose();
            }
            else
            {
                dataSet.Dispose();
            }
            return queueID;
        }

        public bool InsertTicket(Ticket ticket, out long ticketID)
        {
            long mvarticketID = 0;
            string subject = string.Empty;
            ticketID = 0;            
            ticket.QUEUE_TYPE_ID = GetQueueTypeID();
            ticket.QUEUE_ID = GetQueueID(ticket.QUEUE_TYPE_ID, ticket.ORDER_TYPE);

            if(ticket.URGENT ==true)
            {
                ticket.PRIORITY = GetUrgentSeverityID();
            }
            else
            {
                ticket.PRIORITY = 0;
            }

            if(ticket.NOTES.Length >250)
            {
                subject = ticket.NOTES.Substring(0, 250);
            }
            else
            {
                subject = ticket.NOTES;
            }
            
            try
            {
                string employeeName = ticket.EMPLOYEE_FIRST_NAME + " " + ticket.EMPLOYEE_LAST_NAME;
                System.Data.Common.DbCommand dbCommand = null;
                if (ticket.PRIORITY > 0)
                {
                   dbCommand = database.GetSqlStringCommand(string.Format(@"
                            INSERT INTO IQMS.CRM_ACTIVITY
                            (ID, PR_EMP_ID, EPLANT_ID, USER_ID, TYPE, QUEUE_TYPE_ID, QUEUE_ID, REGARDING, CRM_SEVERITY_ID, PRINCIPLE_SOURCE, PRINCIPLE_SOURCE_ID, PRINCIPLE_SOURCE_DISP, PRINCIPLE_FIRST_NAME, PRINCIPLE_LAST_NAME, ALLDAY, SUBJECT, CUSER1, NUSER1, CUSER2, CREATED, STARTDATE )
                            VALUES
                            (IQMS.S_CRM_ACTIVITY.NEXTVAL, {0}, {1}, '{2}', 4, {3}, {4}, '{5}', {6}, 'PR_EMP', {7}, '{8}', '{9}', '{10}', 'N', '{11}', '{12}', {13},'{14}', TO_DATE('{15}', 'YYYY/MM/DD HH:mi:ss'), TO_DATE('{16}', 'YYYY/MM/DD HH:mi:ss') ) returning id into :l_id", ticket.EMPLOYEE_ID, ticket.EPLANT_ID, ticket.USER_NAME, ticket.QUEUE_TYPE_ID, ticket.QUEUE_ID, ticket.NOTES, ticket.PRIORITY, ticket.EMPLOYEE_ID, employeeName, ticket.EMPLOYEE_FIRST_NAME, ticket.EMPLOYEE_LAST_NAME, subject, ticket.WORKCENTER_DESCRIPTION, ticket.WORKORDER_ID, ticket.ITEMNO, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")));

                }
                else
                {
                    dbCommand = database.GetSqlStringCommand(string.Format(@"
                            INSERT INTO IQMS.CRM_ACTIVITY
                            (ID, PR_EMP_ID, EPLANT_ID, USER_ID, TYPE, QUEUE_TYPE_ID, QUEUE_ID, REGARDING, PRINCIPLE_SOURCE, PRINCIPLE_SOURCE_ID, PRINCIPLE_SOURCE_DISP, PRINCIPLE_FIRST_NAME, PRINCIPLE_LAST_NAME, ALLDAY, SUBJECT, CUSER1, NUSER1, CUSER2, CREATED, STARTDATE )
                            VALUES
                            (IQMS.S_CRM_ACTIVITY.NEXTVAL, {0}, {1}, '{2}', 4, {3}, {4}, '{5}', 'PR_EMP', {6}, '{7}', '{8}', '{9}', 'N', '{10}', '{11}', {12},'{13}', TO_DATE('{14}', 'YYYY/MM/DD HH:mi:ss'), TO_DATE('{15}', 'YYYY/MM/DD HH:mi:ss') ) returning id into :l_id", ticket.EMPLOYEE_ID, ticket.EPLANT_ID, ticket.USER_NAME, ticket.QUEUE_TYPE_ID, ticket.QUEUE_ID, ticket.NOTES, ticket.EMPLOYEE_ID, employeeName, ticket.EMPLOYEE_FIRST_NAME, ticket.EMPLOYEE_LAST_NAME, subject, ticket.WORKCENTER_DESCRIPTION, ticket.WORKORDER_ID, ticket.ITEMNO, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")));

                }

                System.Data.Common.DbParameter parameter = dbCommand.CreateParameter();
                parameter.ParameterName = "l_id";
                parameter.Direction = ParameterDirection.Output;
                parameter.DbType = DbType.Decimal;
                dbCommand.Parameters.Add(parameter);

                var reader = database.ExecuteNonQuery(dbCommand);
                mvarticketID = Convert.ToInt64(dbCommand.Parameters["l_id"].Value.ToString());
                ticket.ID = mvarticketID;
                ticketID = mvarticketID;
                return true;                
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        #region IDisposable Members
        public void Dispose()
        {
            database = null;
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
