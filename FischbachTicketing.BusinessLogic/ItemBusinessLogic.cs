using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FischbachTicketing.BusinessEntities;
using FischbachTicketing.DataLogic;

namespace FischbachTicketing.BusinessLogic
{
    public class ItemBusinessLogic : IDisposable
    {
        public List<Item> GetItems(long eplantID)
        {
            using (ItemDataLogic itemDL = new ItemDataLogic())
            {
                return itemDL.GetItems(eplantID);
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
