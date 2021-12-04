using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amms.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        //load about us in navbar
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        //load shop in navbar
        public ActionResult Shop()
        {
            return View();
        }

        //load gallry in navbar
        public ActionResult Gallery()
        {
            return View();
        }

        //load calender
        public ActionResult Calendar()
        {
            return View();
        }

    }
}