using Amms.Models;
//using Amms.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Amms.Controllers
{
    public class ResevationController : Controller
    {
        //UserHelper hUser;
        ServiceController sController;
        VehicleController vController;
    
        //-------------------Load user's reservation page------------------
        // GET: Resevation
        [HttpGet]
        public ActionResult GetReservation()
        {           
            sController = new ServiceController();
            vController = new VehicleController();

            Reservation app = new Reservation();
                app.UserID = Convert.ToInt32(Session["UserID"]);
                app.FirstName = Session["FirstName"].ToString();
                app.LastName = Session["LastName"].ToString();
                app.Mobile = Session["Mobile"].ToString();
                app.EmailID = Session["Email"].ToString();

                app.VehicleList = vController.GetAllVehicle();
                Dictionary<int, string> dropList = new Dictionary<int, string>();
                foreach (var i in app.VehicleList)
                {
                    if (i.UserID == app.UserID)
                    {
                        dropList.Add(i.VehicleID, i.VehicleNumber);
                    }
                }
                app.VehicleDropList = dropList;

                app.ServList = sController.GetAllService();

            return View("Reservation", app);
        }

        //---------------save reservations placed by user to db-----------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetReservation(Reservation app, DateTime appdate, String apptime)
        {
            sController = new ServiceController();
            vController = new VehicleController();
            Appointment appobj = new Appointment();
            AppointmentService appserobj;

            bool Status = false;
            string message = "";
            int id = Convert.ToInt32(Session["UserID"]);

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                try
                {
                    appobj.UserID = id;
                    appobj.AppointmentDate = appdate;
                    appobj.AppointmentStartTime = apptime + ":" + "00";

                    DateTime Time = DateTime.ParseExact(apptime, "HH:mm", CultureInfo.InvariantCulture);
                    int hrs = Time.Hour;
                    string minutes = (Time.Minute).ToString();
                    hrs = hrs + 2;
                    string Hrs = hrs.ToString();

                    string time = Hrs + ":" + minutes + ":" + "00";

                    appobj.AppointmentEndTime = time;
                    appobj.AppointmentStatus = "Pending";
                    appobj.VehicleID = 1;
                    app.Myappointment = appobj;

                    dc.Appointments.Add(app.Myappointment);
                    dc.SaveChanges();

                    foreach (var i in app.ServList)
                    {
                        if (i.IsActive == true)
                        {
                            appserobj = new AppointmentService();
                            appserobj.AppointmentID = app.Myappointment.AppointmentID;
                            appserobj.UserID = app.Myappointment.UserID;
                            appserobj.VehicleID = app.Myappointment.VehicleID;

                            appserobj.ServiceID = i.ServiceID;
                            app.MyappService = appserobj;

                            dc.AppointmentServices.Add(app.MyappService);
                            dc.SaveChanges();
                        }
                    }

                    message = "Your Reservation has successfully placed";
                    Status = true;
                }
                catch(Exception e)
                {
                    message = "Error!";
                }
            }

            app.VehicleList = vController.GetAllVehicle();
            Dictionary<int, string> dropList = new Dictionary<int, string>();
            foreach (var i in app.VehicleList)
            {
                if (i.UserID == app.UserID)
                {
                    dropList.Add(i.VehicleID, i.VehicleNumber);
                }
            }
            app.VehicleDropList = dropList;
          
            app.ServList = sController.GetAllService();

            app.UserID = Convert.ToInt32(Session["UserID"]);
            app.FirstName = Session["FirstName"].ToString();
            app.LastName = Session["LastName"].ToString();
            app.Mobile = Session["Mobile"].ToString();
            app.EmailID = Session["Email"].ToString();

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View("Reservation", app);
        }


          //---------------Load admin's reservation page----------------------

        [HttpGet]
        public ActionResult GetReservationAdmin(int id)
        {
            sController = new ServiceController();
            vController = new VehicleController();
            User user = new User();

            Reservation app = new Reservation();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
                {
                    user = (from c in dc.Users
                            where c.UserID == id
                            select c).FirstOrDefault();
                }
                app.UserID = user.UserID;
                app.FirstName = user.FirstName;
                app.LastName = user.LastName;
                app.Mobile = user.Mobile;
                app.EmailID = user.EmailID;
            

            app.VehicleList = vController.GetAllVehicle();
            Dictionary<int, string> dropList = new Dictionary<int, string>();
            foreach (var i in app.VehicleList)
            {
                if (i.UserID == app.UserID)
                {
                    dropList.Add(i.VehicleID, i.VehicleNumber);
                }
            }
            app.VehicleDropList = dropList;

            app.ServList = sController.GetAllService();

            return View("Reservation", app);
        }

        //-----------------save reservations placed by admin to db

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetReservationAdmin(Reservation app, DateTime appdate, String apptime)
        {
            sController = new ServiceController();
            vController = new VehicleController();
            Appointment appobj = new Appointment();
            AppointmentService appserobj;
            User user = new User();

            bool Status = false;
            string message = "";
            int id = Convert.ToInt32(Session["UserID"]);

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                appobj.UserID = id;
                appobj.AppointmentDate = appdate;
                appobj.AppointmentStartTime = apptime + ":" + "00";

                DateTime Time = DateTime.ParseExact(apptime, "HH:mm", CultureInfo.InvariantCulture);
                int hrs = Time.Hour;
                string minutes = (Time.Minute).ToString();
                hrs = hrs + 2;
                string Hrs = hrs.ToString();

                string time = Hrs + ":" + minutes + ":" + "00";

                appobj.AppointmentEndTime = time;
                appobj.AppointmentStatus = "Pending";
                appobj.VehicleID = 1;
                app.Myappointment = appobj;

                dc.Appointments.Add(app.Myappointment);
                dc.SaveChanges();

                foreach (var i in app.ServList)
                {
                    if (i.IsActive == true)
                    {
                        appserobj = new AppointmentService();
                        appserobj.AppointmentID = app.Myappointment.AppointmentID;
                        appserobj.UserID = app.Myappointment.UserID;
                        appserobj.VehicleID = app.Myappointment.VehicleID;

                        appserobj.ServiceID = i.ServiceID;
                        app.MyappService = appserobj;

                        dc.AppointmentServices.Add(app.MyappService);
                        dc.SaveChanges();
                    }
                }

                message = "success ";
                Status = true;

                     user = (from c in dc.Users
                        where c.UserID == id
                        select c).FirstOrDefault();
            
                         app.UserID = user.UserID;
                         app.FirstName = user.FirstName;
                         app.LastName = user.LastName;
                         app.Mobile = user.Mobile;
                         app.EmailID = user.EmailID;

                app.VehicleList = vController.GetAllVehicle();
            }

          
            Dictionary<int, string> dropList = new Dictionary<int, string>();
            foreach (var i in app.VehicleList)
            {
                if (i.UserID == app.UserID)
                {
                    dropList.Add(i.VehicleID, i.VehicleNumber);
                }
            }
            app.VehicleDropList = dropList;

            app.ServList = sController.GetAllService();


            ViewBag.Message = message;
            ViewBag.Status = Status;
            //return RedirectToAction("AddUser", "Admin");
            return View("Registration", app);
        }

        //-------------------------List all reservation details for admins reservation panel-------------
        public List<Appointment> GetAllAppointments()
        {
            List<Appointment> ListApp = new List<Appointment>();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                Appointment app;
                var loadservice = dc.Appointments; // load all data from Service table 
                foreach (var ser in loadservice)
                {
                    app = new Appointment();
                    app.AppointmentID = ser.AppointmentID;
                    app.AppointmentDate = ser.AppointmentDate;
                    app.AppointmentStartTime = ser.AppointmentStartTime;
                    app.AppointmentEndTime = ser.AppointmentEndTime;
                    app.AppointmentStatus = ser.AppointmentStatus;

                    ListApp.Add(app);
                }
            }
            return ListApp;
        }

    }
}
