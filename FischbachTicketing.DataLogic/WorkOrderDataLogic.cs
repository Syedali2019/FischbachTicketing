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
    public class WorkOrderDataLogic : IDisposable
    {
        private OracleDatabase database;
        private readonly Mapper<WorkOrder> mapper;
        private readonly Mapper<Item> mapperWOItem;

        public WorkOrderDataLogic()
        {
            string connectionString = ConfigurationManager.AppSettings["OracleDB"];
            string masterUserName = ConfigurationManager.AppSettings["MASTERUSERNAME"];
            string masterPassword = ConfigurationManager.AppSettings["MASTERPASSWORD"];

            connectionString = string.Format(connectionString, masterUserName.ToUpper(), masterPassword);

            database = new OracleDatabase(connectionString);
            mapper = new Mapper<WorkOrder>(MapWorkOrder);
            mapperWOItem = new Mapper<Item>(MapWOrkOrderItem); 
        }
        public WorkOrder GetWorkOrderData(long workOrderID)
        {
            var reader = database.ExecuteReader(CommandType.Text, string.Format(@"SELECT W.ID, CS.WORK_CENTER_ID, WC.EQNO AS WORKCENTER_NAME, WC.CNTR_DESC AS WORKCENTER_DESCRIPTION, W.EPLANT_ID, '' AS ITEMNO, '' AS DESCRIP, '' AS DESCRIP2 FROM IQMS.WORKORDER W INNER JOIN IQMS.CNTR_SCHED CS ON CS.WORKORDER_ID=W.ID INNER JOIN IQMS.WORK_CENTER WC ON CS.WORK_CENTER_ID=WC.ID WHERE CS.WORKORDER_ID={0}", workOrderID));

            WorkOrder workOrder = mapper.MapSingle(reader);
            if(workOrder!=null)
            {
                Item item = GetWorkOrderItemData(workOrderID);
                workOrder.ITEMNO = item.ITEMNO;
                workOrder.DESCRIPTION = item.DESCRIPTION;
                workOrder.DESCRIPTION2 = item.DESCRIPTION2;
            }

            if (reader.IsClosed == false)
            {
                reader.Close();
                reader.Dispose();
            }
            return workOrder;
        }

        public Item GetWorkOrderItemData(long workOrderID)
        {
            var reader = database.ExecuteReader(CommandType.Text, string.Format(@"SELECT
	                                                                                    A.ID,
	                                                                                    A.ITEMNO,
	                                                                                    A.DESCRIP,
	                                                                                    A.DESCRIP2,
	                                                                                    WO.EPLANT_ID
                                                                                    FROM 
	                                                                                    IQMS.WORKORDER WO INNER JOIN 
	                                                                                    IQMS.PARTNO P ON WO.STANDARD_ID = P.STANDARD_ID   
	                                                                                    INNER JOIN IQMS.ARINVT A ON P.ARINVT_ID = A.ID
	                                                                                    INNER JOIN IQMS.STANDARD S ON WO.STANDARD_ID = S.ID
                                                                                    WHERE 
	                                                                                    WO.ID = {0}", workOrderID));

            Item item = mapperWOItem.MapSingle(reader);


            if (reader.IsClosed == false)
            {
                reader.Close();
                reader.Dispose();
            }
            return item;
        }

        public List<Item> GetWorkOrderInkItemData(long workOrderID)
        {
            var reader = database.ExecuteReader(CommandType.Text, string.Format(@"SELECT 
                                                                                            A.ID, 
                                                                                            A.ITEMNO, 
                                                                                            A.DESCRIP, 
                                                                                            A.DESCRIP2,
                                                                                            A.EPLANT_ID
                                                                                        FROM 
	                                                                                        IQMS.WORKORDER WO INNER JOIN IQMS.PARTNO P ON P.STANDARD_ID=WO.STANDARD_ID
	                                                                                        INNER JOIN IQMS.PTOPER PO ON PO.PARTNO_ID=P.ID
	                                                                                        INNER JOIN IQMS.SNDOP SN ON PO.SNDOP_ID=SN.ID
	                                                                                        INNER JOIN IQMS.OPMAT OP ON SN.ID=OP.SNDOP_ID
	                                                                                        INNER JOIN IQMS.ARINVT A ON OP.ARINVT_ID=A.ID
                                                                                        WHERE WO.ID={0} AND CLASS='IN'", workOrderID));

            IEnumerable<Item> lists = mapperWOItem.MapList(reader);
            List<Item> itemList = lists as List<Item>;

            if (reader.IsClosed == false)
            {
                reader.Close();
                reader.Dispose();
            }
            return itemList;
        }

        private static WorkOrder MapWorkOrder(IDataReader reader)
        {
            var workOrder = new WorkOrder
            {
                ID = reader["ID"] == DBNull.Value ? 0 : Convert.ToInt64(reader["ID"]),
                WORKCENTER_ID = reader["WORK_CENTER_ID"] == DBNull.Value ? 0 : Convert.ToInt64(reader["WORK_CENTER_ID"]),
                EPLANT_ID = reader["EPLANT_ID"] == DBNull.Value ? 0 : Convert.ToInt64(reader["EPLANT_ID"]),
                WORKCENTER_NAME = reader["WORKCENTER_NAME"] == DBNull.Value ? string.Empty : Convert.ToString(reader["WORKCENTER_NAME"]),
                WORKCENTER_DESCRIPTION = reader["WORKCENTER_DESCRIPTION"] == DBNull.Value ? string.Empty : Convert.ToString(reader["WORKCENTER_DESCRIPTION"]),
                ITEMNO = reader["ITEMNO"] == DBNull.Value ? string.Empty : Convert.ToString(reader["ITEMNO"]),
                DESCRIPTION = reader["DESCRIP"] == DBNull.Value ? string.Empty : Convert.ToString(reader["DESCRIP"]),
                DESCRIPTION2 = reader["DESCRIP2"] == DBNull.Value ? string.Empty : Convert.ToString(reader["DESCRIP2"]),
            };
            return workOrder;
        }

        private static Item MapWOrkOrderItem(IDataReader reader)
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
