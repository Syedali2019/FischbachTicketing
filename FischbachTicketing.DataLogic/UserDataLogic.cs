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
    public class UserDataLogic : IDisposable
    {
        private OracleDatabase database;
        private readonly Mapper<User> mapper;

        public UserDataLogic()
        {
            string connectionString = ConfigurationManager.AppSettings["OracleDB"];
            string masterUserName = ConfigurationManager.AppSettings["MASTERUSERNAME"];
            string masterPassword = ConfigurationManager.AppSettings["MASTERPASSWORD"];

            connectionString = string.Format(connectionString, masterUserName.ToUpper(), masterPassword);

            database = new OracleDatabase(connectionString);
            mapper = new Mapper<User>(MapUser);
        }

        public UserDataLogic(string userName, string password)
        {
            string connectionString = ConfigurationManager.AppSettings["OracleDB"];
            connectionString = string.Format(connectionString, userName.ToUpper(), password);

            database = new OracleDatabase(connectionString);
            mapper = new Mapper<User>(MapUser);
        }

        public bool AuthenticateUser()
        {
            try
            {
                DataSet dataSet = this.database.ExecuteDataSet(CommandType.Text, string.Format(@"SELECT TO_CHAR (SYSDATE, 'MM-DD-YYYY HH24:MI:SS') SYS_DATE FROM DUAL"));
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    dataSet.Dispose();
                    return true;
                }
                else
                {
                    dataSet.Dispose();
                    return false;
                }
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public User GetUserData(string userName)
        {
            try
            {
                var reader = database.ExecuteReader(CommandType.Text, string.Format(@"
                                                                                SELECT 
                                                                                    SG.USER_NAME, 
                                                                                    E.ID AS EPLANT_ID, 
                                                                                    E.NAME AS EPLANT_NAME,
                                                                                    SG.PR_EMP_ID
                                                                                FROM 
	                                                                                IQMS.S_USER_GENERAL SG INNER JOIN 
                                                                                    IQMS.EPLANT E ON SG.EPLANT_ID=E.ID
                                                                                WHERE 
                                                                                    SG.USER_NAME='{0}'", userName.ToUpper()));
                User user = mapper.MapSingle(reader);
                if (reader.IsClosed == false)
                {
                    reader.Close();
                    reader.Dispose();
                }
                return user;
            }
            catch (Exception exp)
            {
                return null;
            }
        }

        private static User MapUser(IDataReader reader)
        {
            var user = new User
            {
                UserName = reader["USER_NAME"] == DBNull.Value ? string.Empty : Convert.ToString(reader["USER_NAME"]),
                EplantID = reader["EPLANT_ID"] == DBNull.Value ? 0 : Convert.ToInt64(reader["EPLANT_ID"]),
                EplantName = reader["EPLANT_NAME"] == DBNull.Value ? string.Empty : Convert.ToString(reader["EPLANT_NAME"]),
                EmployeeID = reader["PR_EMP_ID"] == DBNull.Value ? -1 : Convert.ToInt64(reader["PR_EMP_ID"]),                
            };
            return user;
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
