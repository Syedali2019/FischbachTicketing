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
    public class EmployeeDataLogic : IDisposable
    {
        private OracleDatabase database;
        private readonly Mapper<Employee> mapper;

        public EmployeeDataLogic()
        {
            string connectionString = ConfigurationManager.AppSettings["OracleDB"];
            string masterUserName = ConfigurationManager.AppSettings["MASTERUSERNAME"];
            string masterPassword = ConfigurationManager.AppSettings["MASTERPASSWORD"];

            connectionString = string.Format(connectionString, masterUserName.ToUpper(), masterPassword);

            database = new OracleDatabase(connectionString);
            mapper = new Mapper<Employee>(MapEmployee);
        }

        public Employee GetEmployee(long employeeID)
        {
            var reader = database.ExecuteReader(CommandType.Text, string.Format("SELECT ID, EMPNO,BADGENO, FIRST_NAME, LAST_NAME FROM IQMS.PR_EMP WHERE ID={0}", employeeID));
            Employee employee = mapper.MapSingle(reader);
            if (reader.IsClosed == false)
            {
                reader.Close();
                reader.Dispose();
            }
            return employee;
        }

        private static Employee MapEmployee(IDataReader reader)
        {
            var employee = new Employee
            {
                ID = reader["ID"] == DBNull.Value ? 0 : Convert.ToInt64(reader["ID"]),
                EmployeeNumber = reader["EMPNO"] == DBNull.Value ? string.Empty : Convert.ToString(reader["EMPNO"]),
                BadgeNumber = reader["BADGENO"] == DBNull.Value ? string.Empty : Convert.ToString(reader["BADGENO"]),
                FirstName = reader["FIRST_NAME"] == DBNull.Value ? string.Empty : Convert.ToString(reader["FIRST_NAME"]),
                LastName = reader["LAST_NAME"] == DBNull.Value ? string.Empty : Convert.ToString(reader["LAST_NAME"])                
            };
            return employee;
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
