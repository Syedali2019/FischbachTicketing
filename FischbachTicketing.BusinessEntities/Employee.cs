using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FischbachTicketing.BusinessEntities
{
    public class Employee
    {
        public long ID { get; set; }
        public string EmployeeNumber { get; set; }
        public string BadgeNumber { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
    }
}
