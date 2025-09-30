using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FischbachTicketing.BusinessEntities;
using FischbachTicketing.BusinessLogic;
using FischbachTicketing.Common;
using System.Configuration;
using Newtonsoft.Json;

namespace FischbachTicketing.Web.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        public ActionResult Index()
        {
            Object jsonData = null;
            ViewBag.Title = ConfigurationManager.AppSettings["COMPANYNAME"].ToString() + " :: " + ConfigurationManager.AppSettings["SYSTEMNAME"].ToString();
            ViewBag.CompanyName = ConfigurationManager.AppSettings["COMPANYNAME"].ToString();
            ViewBag.SystemName = ConfigurationManager.AppSettings["SYSTEMNAME"].ToString();

            if (Session["User"] != null)
            {
                User user = (User)Session["User"];               
                ViewBag.UserFullName = user.UserName;
                ViewBag.EplantName = user.EplantName;
                ViewBag.SessionID = Session.SessionID;
                ViewBag.EmployeeID = user.EmployeeID;
                return View();
            }
            else
            {
                jsonData = new
                {
                    status = false,
                    message = string.Empty
                };
                return Json(jsonData);
            }
        }

        public ActionResult GetEmployeeID(Int32 employeeID)
        {
            Object jsonData = null;
            if (Session["User"] != null)
            {
                User user = (User)Session["User"];
                Employee employee = null;
                using (EmployeeBusinessLogic employeeBL = new EmployeeBusinessLogic())
                {
                    employee = employeeBL.GetEmployee(user.EmployeeID);
                }

                if(employee != null)
                {
                    Session["Employee"] = employee;
                    jsonData = new
                    {
                        status = true,
                        message = string.Empty,
                        employee = employee
                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    jsonData = new
                    {
                        status = false,
                        message = string.Format("No employee is associated with this user {0}", user.UserName)
                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                jsonData = new
                {   
                    status = false,
                    isRedirect = true,
                    url = "/Login/Index"
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetWorkOrderData(Int32 workOrderID)
        {
            Object jsonData = null;
            if (Session["User"] != null)
            {
                User user = (User)Session["User"];
                WorkOrder workOrder = null;
                using (WorkOrderBusinessLogic workOrderBL = new WorkOrderBusinessLogic())
                {
                    workOrder = workOrderBL.GetWorkOrderData(workOrderID);
                    if(workOrder!=null)
                    {
                        user.EplantID = workOrder.EPLANT_ID;
                    }                   
                }

                if (workOrder != null)
                {                    
                    jsonData = new
                    {
                        status = true,
                        message = string.Empty,
                        workOrder = workOrder
                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    jsonData = new
                    {
                        status = false,
                        message = string.Format("WorkOrder # {0} was either not found or has not been scheduled", workOrderID)
                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                jsonData = new
                {
                    status = false,
                    isRedirect = true,
                    url = "/Login/Index"
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetWorkOrderItemData(string sidx, string sord, int page, int rows)
        {
            if (Session["User"] != null)
            {
                using (WorkOrderBusinessLogic workOrderBL = new WorkOrderBusinessLogic())
                {
                    User user = (User)Session["User"];
                    List<Item> itemList = null;

                    itemList = workOrderBL.GetWorkOrderInkItemData(Convert.ToInt32(RouteData.Values["id"]));

                    var Results = itemList;
                    int pageIndex = Convert.ToInt32(page) - 1;
                    int pageSize = rows;
                    int totalRecords = Results.Count();

                    var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
                    if (sord.ToUpper() == "DESC")
                    {
                        Results = Results.OrderByDescending(s => s.ID).ToList();
                        Results = Results.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                    }
                    else
                    {
                        Results = Results.OrderBy(s => s.ID).ToList();
                        Results = Results.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                    }
                    var jsonData = new
                    {
                        total = totalPages,
                        page,
                        records = totalRecords,
                        rows = Results
                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult GetItems(string sidx, string sord, int page, int rows, bool _search, string filters)
        {
            if (Session["User"] != null)
            {
                using (ItemBusinessLogic itemBL = new ItemBusinessLogic())
                {
                    User user = (User)Session["User"];
                    List<Item> itemList = null;

                    itemList = itemBL.GetItems(Convert.ToInt32(user.EplantID));

                    if (_search)
                    {
                        SearchFilter searchFilter = null;
                        if (filters != null)
                        {
                            searchFilter = JsonConvert.DeserializeObject<SearchFilter>(filters);
                        }

                        foreach (Rule rule in searchFilter.rules)
                        {

                            if (rule.field.Equals("ITEMNO"))
                            {
                                itemList = itemList.Where(msearch => msearch.ITEMNO.StartsWith(rule.data)).ToList();
                            }

                            if (rule.field.Equals("DESCRIPTION"))
                            {
                                itemList = itemList.Where(msearch => msearch.DESCRIPTION.StartsWith(rule.data)).ToList();
                            }                           
                        }
                    }

                    var Results = itemList;
                    int pageIndex = Convert.ToInt32(page) - 1;
                    int pageSize = rows;
                    int totalRecords = Results.Count();

                    var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
                    if (sord.ToUpper() == "DESC")
                    {
                        Results = Results.OrderByDescending(s => s.ID).ToList();
                        Results = Results.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                    }
                    else
                    {
                        Results = Results.OrderBy(s => s.ID).ToList();
                        Results = Results.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                    }
                    var jsonData = new
                    {
                        total = totalPages,
                        page,
                        records = totalRecords,
                        rows = Results
                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult SaveTicket(Ticket ticket)
        {
            if (Session["User"] != null)
            {
                Object jsonData = null;
                
                User user = (User)Session["User"];
                Employee emp = (Employee)Session["Employee"];
                long ticketID = 0;
                ticket.USER_NAME = user.UserName;
                ticket.EMPLOYEE_ID = emp.ID;
                ticket.EMPLOYEE_FIRST_NAME = emp.FirstName;
                ticket.EMPLOYEE_LAST_NAME = emp.LastName;
                ticket.EMPLOYEE_MIDDLE_NAME = emp.MiddleName;
                ticket.EPLANT_ID = user.EplantID;
                using (TicketBusinessLogic ticketBL = new TicketBusinessLogic())
                {
                    ticketBL.InsertTicket(ticket, out ticketID);
                    Session.Add("TICKET", ticket);
                }

                if(ticketID > 0)
                {
                    jsonData = new
                    {
                        status = true,
                        ticketID = ticketID,
                        message = "CRM Activity successfully save."
                    };
                }
                else
                {
                    jsonData = new
                    {
                        status = false,
                        ticketID = 0,
                        message = "Couldn't save the CRM Activity."
                    };
                }                

                return Json(jsonData);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Logout()
        {
            Session.Remove("User");
            Session.Remove("TICKET");
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public JsonResult KeepSessionAlive()
        {
            // You may do stuff here
            return new JsonResult { Data = "success" };
        }
    }
}