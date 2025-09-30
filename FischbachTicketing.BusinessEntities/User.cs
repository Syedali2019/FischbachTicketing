using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FischbachTicketing.BusinessEntities
{
    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public long EmployeeID { get; set; }
        public long EplantID { get; set; }
        public string EplantName { get; set; }
    }
}
