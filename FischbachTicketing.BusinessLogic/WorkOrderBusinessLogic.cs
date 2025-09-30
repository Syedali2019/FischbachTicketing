using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FischbachTicketing.BusinessEntities;
using FischbachTicketing.DataLogic;

namespace FischbachTicketing.BusinessLogic
{
    public class WorkOrderBusinessLogic : IDisposable
    {
        public WorkOrder GetWorkOrderData(long workOrderID)
        {
            using (WorkOrderDataLogic workOrderDL = new WorkOrderDataLogic())
            {
                return workOrderDL.GetWorkOrderData(workOrderID);
            }
        }

        public List<Item> GetWorkOrderInkItemData(long workOrderID)
        {
            using (WorkOrderDataLogic workOrderDL = new WorkOrderDataLogic())
            {
                return workOrderDL.GetWorkOrderInkItemData(workOrderID);
            }
        }

        #region IDisposable Members
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
