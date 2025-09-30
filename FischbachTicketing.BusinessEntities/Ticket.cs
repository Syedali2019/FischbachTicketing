using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FischbachTicketing.BusinessEntities
{
    public class Ticket
    {
        public string ORDER_TYPE { get; set; }
        public long ID { get; set; }
        public long WORKORDER_ID { get; set; }
        public long EPLANT_ID { get; set; }
        public long EMPLOYEE_ID { get; set; }
        public string WORKCENTER_DESCRIPTION { get; set; }
        public string USER_NAME { get; set; }
        public string NOTES { get; set; }
        public string EMPLOYEE_FIRST_NAME { get; set; }
        public string EMPLOYEE_MIDDLE_NAME { get; set; }
        public string EMPLOYEE_LAST_NAME { get; set; }
        public int QUEUE_ID { get; set; }
        public int QUEUE_TYPE_ID { get; set; }
        public int PRIORITY { get; set; }
        public bool URGENT { get; set; }
        public string ITEMNO { get; set; }
    }
}
