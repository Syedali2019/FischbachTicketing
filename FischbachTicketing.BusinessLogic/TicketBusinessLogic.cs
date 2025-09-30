using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FischbachTicketing.BusinessEntities;
using FischbachTicketing.DataLogic;

namespace FischbachTicketing.BusinessLogic
{
    public class TicketBusinessLogic : IDisposable
    {
        public bool InsertTicket(Ticket ticket, out long ticketID)
        {
            using (TicketDataLogic ticketDL = new TicketDataLogic())
            {
                return ticketDL.InsertTicket(ticket, out ticketID);
            }
        }

        public int GetQueueTypeID()
        {
            using (TicketDataLogic ticketDL = new TicketDataLogic())
            {
                return ticketDL.GetQueueTypeID();
            }
        }

        public int GetQueueID(int queueTypeID, string orderType)
        {
            using (TicketDataLogic ticketDL = new TicketDataLogic())
            {
                return ticketDL.GetQueueID(queueTypeID, orderType);
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
