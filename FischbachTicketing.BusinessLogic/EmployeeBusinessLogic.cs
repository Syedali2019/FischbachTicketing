using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FischbachTicketing.BusinessEntities;
using FischbachTicketing.DataLogic;

namespace FischbachTicketing.BusinessLogic
{
    public class EmployeeBusinessLogic : IDisposable
    {

        public Employee GetEmployee(long employeeID)
        {
            using (EmployeeDataLogic empDL = new EmployeeDataLogic())
            {
                return empDL.GetEmployee(employeeID);
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
