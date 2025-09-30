using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FischbachTicketing.BusinessEntities;
using FischbachTicketing.DataLogic;

namespace FischbachTicketing.BusinessLogic
{
    public class UserBusinessLogic : IDisposable
    {
        public bool Authentication(string userName, string password)
        {
            using (UserDataLogic userDL = new UserDataLogic(userName, password))
            {
                return userDL.AuthenticateUser();
            }
        }

        public User GetUserData(string userName)
        {            
            using (UserDataLogic userDL = new UserDataLogic())
            {
                return userDL.GetUserData(userName);
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
