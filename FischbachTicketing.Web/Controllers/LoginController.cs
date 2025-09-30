using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FischbachTicketing.BusinessEntities;
using FischbachTicketing.BusinessLogic;
using FischbachTicketing.Common;
using System.Configuration;

namespace FischbachTicketing.Web.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            ViewBag.Title = ConfigurationManager.AppSettings["COMPANYNAME"].ToString() + " :: " + ConfigurationManager.AppSettings["SYSTEMNAME"].ToString();
            ViewBag.CompanyName = ConfigurationManager.AppSettings["COMPANYNAME"].ToString();
            ViewBag.SystemName = ConfigurationManager.AppSettings["SYSTEMNAME"].ToString();
            return View();
        }

        [HttpPost]
        public ActionResult UserLogin(string userName, string password)
        {
            Object jsonData = null;
            User user = null;
            bool authenticate = false;
            
            using (UserBusinessLogic userBL = new UserBusinessLogic())
            {
                authenticate = userBL.Authentication(userName, password);
                if (authenticate == true)
                {
                    user = userBL.GetUserData(userName);                    
                }
            }

            if (user !=null && authenticate == true)
            {
                
                user.Password = password;
                Session["User"] = user;
                jsonData = new
                {
                    status = true,
                    message = "Successfully Login"
                };
            }
            else
            {
                jsonData = new
                {
                    status = false,                    
                    message = "Invalid username or password"
                };
            }
            return Json(jsonData);
        }
    }
}