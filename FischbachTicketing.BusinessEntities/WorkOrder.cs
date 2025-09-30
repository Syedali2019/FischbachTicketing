using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FischbachTicketing.BusinessEntities
{
    public class WorkOrder
    {
        public long ID { get; set; }
        public long WORKCENTER_ID { get; set; }
        public long EPLANT_ID { get; set; }
        public string WORKCENTER_NAME { get; set; }
        public string WORKCENTER_DESCRIPTION { get; set; }        
        public string ITEMNO { get; set; }
        public string DESCRIPTION { get; set; }
        public string DESCRIPTION2 { get; set; }
    }
}
