using Amms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amms.Controllers
{
    public class InquiryController : Controller
    {
        //--------------load inquiry page----------------
        [HttpGet]
        public ActionResult Inquiry()
        {
            Inquiry inquiry = new Inquiry();
           
            return View("Inquiry", inquiry);
        }

        //--------------record inquiry--------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Inquiry(Inquiry inquiry)
        {
            bool Status = false;
            string message = "";

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                dc.Inquiries.Add(inquiry);
                dc.SaveChanges();

                ModelState.Clear();
                inquiry.Name = " ";
                inquiry.Message = " ";
                inquiry.Email = "";

                message = "Your Inquiry is successfully recorded. We will respond to you as soon as possible";
                Status = true;
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View("Inquiry", inquiry);
        }
    }
}