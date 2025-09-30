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
    public class ItemDataLogic : IDisposable
    {
        private OracleDatabase database;
        private readonly Mapper<Item> mapper;

        public ItemDataLogic()
        {
            string connectionString = ConfigurationManager.AppSettings["OracleDB"];
            string masterUserName = ConfigurationManager.AppSettings["MASTERUSERNAME"];
            string masterPassword = ConfigurationManager.AppSettings["MASTERPASSWORD"];

            connectionString = string.Format(connectionString, masterUserName.ToUpper(), masterPassword);

            database = new OracleDatabase(connectionString);
            mapper = new Mapper<Item>(MapItem);
        }

        public List<Item> GetItems(long eplantID)
        {
            var reader = database.ExecuteReader(CommandType.Text, string.Format(@"SELECT ID, ITEMNO, DESCRIP, DESCRIP2, CLASS FROM IQMS.ARINVT WHERE CLASS='IN' AND EPLANT_ID={0} ORDER BY ITEMNO", eplantID));

            IEnumerable<Item> lists = mapper.MapList(reader);
            List<Item> itemList = lists as List<Item>;

            if (reader.IsClosed == false)
            {
                reader.Close();
                reader.Dispose();
            }
            return itemList;
        }

        private static Item MapItem(IDataReader reader)
        {
            var item = new Item
            {
                ID = reader["ID"] == DBNull.Value ? 0 : Convert.ToInt64(reader["ID"]),
                ITEMNO = reader["ITEMNO"] == DBNull.Value ? string.Empty : Convert.ToString(reader["ITEMNO"]),
                DESCRIPTION = reader["DESCRIP"] == DBNull.Value ? string.Empty : Convert.ToString(reader["DESCRIP"]),
                DESCRIPTION2 = reader["DESCRIP2"] == DBNull.Value ? string.Empty : Convert.ToString(reader["DESCRIP2"]),
            };
            return item;
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
