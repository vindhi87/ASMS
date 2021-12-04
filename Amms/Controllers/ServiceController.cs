using Amms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amms.Controllers
{
    public class ServiceController : Controller
    {
        // GET: Service
        public ActionResult Index()
        {           
            return View();
           
        }

        //-----------------load service page nav bar---------------------
        public ActionResult Service()
        {
            ViewBag.Message = "Your service page.";
            Reservation app = new Reservation();
            List<Service> ServiceList = GetAllService();
            app.ServList = ServiceList;

            return View(app);
        }


        //----------------return list of services for required places-------------------
        public List<Service> GetAllService()
        {
            List<Service> ListService = new List<Service>();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                Service serv;

                var loadservice = dc.Services; // load all data from Service table 
                foreach (var ser in loadservice)
                {
                    serv = new Service();
                    serv.ServiceID = ser.ServiceID;
                    serv.ServiceName = ser.ServiceName;
                    serv.ServiceDescription = ser.ServiceDescription;
                    serv.ServicePrice = ser.ServicePrice;
                    serv.ServiceTime = ser.ServiceTime;
                    serv.ServiceImage = ser.ServiceImage;
                    
                    ListService.Add(serv);
                }
            }
            return ListService;
        }

        //------------------return list of vehicle service history details for custoemr profile-------------
        public List<Profile> GetHistory(int id)
        {
            List<Profile> Servhistory = new List<Profile>();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                Profile history;
               
                var details = (from a in dc.Invoices
                               join b in dc.InvoiceDetails on a.InvoiceID equals b.InvoiceID
                               join c in dc.Users on a.UserID equals c.UserID
                               join d in dc.Vehicles on a.VehicleID equals d.VehicleID
                               join e in dc.Services on b.ServiceID equals e.ServiceID
                               where c.UserID == id
                               where b.Remarks == "s"
                               select new
                               {
                                   a.InvoiceID,
                                   a.InvDate,
                                   c.FirstName,
                                   c.LastName,
                                   d.VehicleNumber,
                                   e.ServiceName
                               }).ToList();
                             
                foreach (var ser in details)
                {
                    history = new Profile();
                    history.InvoiceID = ser.InvoiceID;
                    history.InvDate = ser.InvDate;
                    history.FirstName = ser.FirstName;
                    history.LastName = ser.LastName;
                    history.FirstName = history.FirstName + " " + history.LastName;
                    history.VehicleNumber = ser.VehicleNumber;
                    history.ServiceName = ser.ServiceName;

                    Servhistory.Add(history);
                }               
            }
            return Servhistory;
        }
    }
}
