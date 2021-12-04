using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amms.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult GetDashboard()
        {
            return View("Dashboard");
        }
    }
}