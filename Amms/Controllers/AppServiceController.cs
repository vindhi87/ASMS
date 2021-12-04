using Amms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amms.Controllers
{
    public class AppServiceController : Controller
    {
        // GET: AppService
        public ActionResult Index()
        {
            return View();
        }


        public List<AppointmentService> GetApp()
        {
            List<AppointmentService> ListService = new List<AppointmentService>();

            using (MyDatabaseEntities db = new MyDatabaseEntities())
            {
                List<Invoice> employees = db.Invoices.ToList();
                List<InvoiceDetail> departments = db.InvoiceDetails.ToList();


                var employeeRecord = from e in employees
                                     join d in departments on e.InvoiceID equals d.InvoiceID into table1
                                     from d in table1.ToList()
                                     select new Profile
                                     {
                                         employee = e,
                                         department = d,

                                     };

            }
            return ListService;

        }
    }
}

