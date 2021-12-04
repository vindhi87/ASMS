using Amms.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Amms.Controllers
{
    public class UserController : Controller
    {
        VehicleController vController;
        ServiceController scontroller;
        ResevationController rController;

        string button;
        int updatevehicleid;

      //  public object rcontroller { get; private set; }

        //--------------------------------user registration---------------------------------
        //Registration Action
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        //Registration POST action 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified,ActivationCode")] User user)
        {
            bool Status = false;
            string message = "";
            
            // Model Validation 
            if (ModelState.IsValid)
            {
                #region //Email is already Exist 
                var isExist = IsEmailExist(user.EmailID);  //checking for email already exist. below implemented
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View(user);
                }
                #endregion

                #region Generate Activation Code 
                user.ActivationCode = Guid.NewGuid();
                #endregion

                #region  Password Hashing 
                user.Password = Crypto.Hash(user.Password); //password hashing
                user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword); //confirm password hashing 
                #endregion

                user.IsEmailVerified = false;
                user.UserName = user.EmailID;
                user.UserGroup = "User";
                
                #region Save to Database
                using (MyDatabaseEntities dc = new MyDatabaseEntities())
                {
                    dc.Users.Add(user);                   
                    dc.SaveChanges();

                    //Send Email to User to activate the link
                    SendVerificationLinkEmail(user.EmailID, user.ActivationCode.ToString());
                    message = "Successfully registered. Account activation link " +
                        " has been sent to your email address: " + user.EmailID;
                    Status = true;
                }
                #endregion
            }
            else
            {
                message = "Invalid Request";
            }

            ModelState.Clear();
            user.FirstName = " ";
            user.LastName = " ";
            user.Address = " ";
            user.Mobile = " ";
            user.EmailID = " ";
            user.Password = " ";
            user.ConfirmPassword = " ";
            
            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(user);
        }

        //Verify Account  
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;                    

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                dc.Configuration.ValidateOnSaveEnabled = false; // This line I have added here to avoid confirm password does not match issue on save changes
                var v = dc.Users.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerified = true;
                    dc.SaveChanges();
                    Status = true;                   
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }

            ViewBag.Status = Status;
            return View();
        }

        //-------------------User Login---------------------------
        //Login 
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        //Login POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string ReturnUrl = "")
        {
            string message = "";
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                var v = dc.Users.Where(a => a.EmailID == login.EmailID).FirstOrDefault();
               
                if (v != null)
                {
                    if (!v.IsEmailVerified)
                    {
                        ViewBag.Message = "Please verify your email first";
                        return View();
                    }

                    if (string.Compare(Crypto.Hash(login.Password), v.Password) == 0)
                    {
                        int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
                        var ticket = new FormsAuthenticationTicket(login.EmailID, login.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);

                        //------------------Assigning user details to session variables to be used in profile and reservation pages------------
                        //------- Session Variable -------------
                        Session["UserID"] = v.UserID;
                        Session["UserName"] = v.UserName;
                        Session["FirstName"] = v.FirstName;
                        Session["LastName"] = v.LastName;
                        Session["Address"] = v.Address;
                        Session["Mobile"] = v.Mobile;                  
                        Session["Email"] = v.EmailID;
                        Session["UserGroup"] = v.UserGroup;
                        //------- End Session Variable -------------

                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("GetDashboard", "Dashboard"); //Successfull login home page displayed
                        }
                    }
                    else
                    {
                        message = "Invalid credential provided";
                    }
                }
                else
                {
                    message = "Invalid credential provided";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        //Logout and clearing session variables  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
           
            //------- Session Variable -------------
            Session["UserName"] = null;
            Session["FirstName"] = null;
            Session["LastName"] = null;
            Session["Address"] = null;
            Session["Mobile"] = null;
            Session["Email"] = null;
            Session["count"] = null;
            Session["cart"] = null;

            //------- End Session Variable -------------

            return RedirectToAction("GetDashboard", "Dashboard");
        }


        // Checking for email already exist in registration
        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {                
                var v = dc.Users.Where(a => a.EmailID == emailID).FirstOrDefault();
                return v != null;
            }
        }

        //-------------Sending verification email to the custoemr asking to activate the account------------------------------
        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode)
        {
            var verifyUrl = "/User/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("automobileservicesms@gmail.com", "Automobile Service Station Management System"); // System email address
            var toEmail = new MailAddress(emailID);

            var fromEmailPassword = "PassworD1."; // Can replace this with actual password

            string subject = "Your account is successfully created!";

            string body = "<br/><br/>User account is successfuly created." +
                " Please click on the below link to verify your account" +
                " <br/><br/><a href='" + link + "'>" + link + "</a> ";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword),               
                Port = 587,                
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);                                 
        }


        //------------------------------Vehicles------------------------------

            //--------------------------Add------------------------------------

        [HttpGet]
        public ActionResult Profile()
        {
            button = "add";
        
            vController = new VehicleController();
            scontroller = new ServiceController();
            rController = new ResevationController();

            Newuser vehicle = new Newuser();
           
            vehicle.UserID = Convert.ToInt32(Session["UserID"]);
            vehicle.UserName = Session["UserName"].ToString();
            vehicle.FirstName = Session["FirstName"].ToString();
            vehicle.LastName = Session["LastName"].ToString();
            vehicle.Address = Session["Address"].ToString();
            vehicle.Mobile= Session["Mobile"].ToString();
            vehicle.EmailID = Session["Email"].ToString();            
        
            ViewBag.Button = button;
            vehicle.VehicleList = vController.GetAllVehicle();

            vehicle.Listhistory = scontroller.GetHistory(vehicle.UserID);

            ViewBag.Button = button;
            return View("Profile", vehicle);           
        }    
       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Profile(Newuser vehicle)
        {
            button = "add";           

            bool Status = false;
            string message = "";
            bool type = false;

            vController = new VehicleController();
            scontroller = new ServiceController();
            List<Vehicle> listvehicle = new List<Vehicle>();
            

            try
            {                
                vehicle.Myvehicle.UserID = Convert.ToInt32(Session["UserID"]);

                using (MyDatabaseEntities dc = new MyDatabaseEntities())
                {
                     var vehicleid = (from c in dc.Vehicles
                                   where c.UserID == vehicle.Myvehicle.UserID
                                   select c.VehicleNumber).ToList();


                    foreach (var i  in vehicleid) 
                    {
                        if (i == vehicle.Myvehicle.VehicleNumber)
                        {
                            message = " Vehicle already in the system";
                            Status = true;
                        }
                        
                    }

                    if (Status == false)
                    {
                        dc.Vehicles.Add(vehicle.Myvehicle);
                        dc.SaveChanges();

                        message = " Vehicle added successfully";
                        Status = true;
                    }

                }
                
            }

            catch(Exception ex)
            {
                message = "Vehicle not added";
                Status = true;
                type = true;
            }

            vehicle.UserID  = Convert.ToInt32(Session["UserID"]);
            vehicle.UserName = Session["UserName"].ToString();
            vehicle.FirstName = Session["FirstName"].ToString();
            vehicle.LastName = Session["LastName"].ToString();
            vehicle.Address = Session["Address"].ToString();
            vehicle.Mobile = Session["Mobile"].ToString();
            vehicle.EmailID = Session["Email"].ToString();
          
            vehicle.VehicleList = vController.GetAllVehicle();

            vehicle.Listhistory = scontroller.GetHistory(vehicle.UserID); 
        

            ModelState.Clear();
            vehicle.Myvehicle.VehicleModel = " ";
            vehicle.Myvehicle.VehicleNumber = " ";
            vehicle.Myvehicle.Year = 0;
            vehicle.Myvehicle.VehicleType = " ";        

            ViewBag.Button = button;
            ViewBag.Message = message;
            ViewBag.Status = Status;
           
            return View("Profile", vehicle);

        }

        //-----------------------------Load and Update------------------

        [HttpGet]
        public ActionResult UpdateVehicle(string id)
        {
             button = "update";

            Newuser vehicle = new Newuser();
            vController = new VehicleController();
            scontroller = new ServiceController();

            int vehicleid;
            int uid; 

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                vehicleid = Convert.ToInt32(id);

                vehicle.UserID = Convert.ToInt32(Session["UserID"]);

                vehicle.Myvehicle = (from c in dc.Vehicles
                           where c.VehicleID == vehicleid
                           select c).FirstOrDefault();           
               
            }

            vehicle.UserID = Convert.ToInt32(Session["UserID"]);
            vehicle.UserName = Session["UserName"].ToString();
            vehicle.FirstName = Session["FirstName"].ToString();
            vehicle.LastName = Session["LastName"].ToString();
            vehicle.Address = Session["Address"].ToString();
            vehicle.Mobile = Session["Mobile"].ToString();
            vehicle.EmailID = Session["Email"].ToString();
            

            vehicle.VehicleList = vController.GetAllVehicle();

            ViewBag.Button = button;
            updatevehicleid = vehicleid;
            return View("Profile", vehicle);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateVehicle(Newuser vehicle)
        {
            bool Status = false;
            string message = "";
            bool type = false;
            button = "add";
            vController = new VehicleController();
            scontroller = new ServiceController();

            try
            {                
                using (MyDatabaseEntities dc = new MyDatabaseEntities())
                {
                    dc.Entry(vehicle.Myvehicle).State = EntityState.Modified;
                    dc.SaveChanges();

                    message = " Vehicle updated successfully";
                    Status = true;
                }
            }

            catch (Exception ex)
            {
                message = "Vehicle not updated";
                Status = true;
                type = true;
            }

            ModelState.Clear();
            vehicle.Myvehicle.VehicleModel = " ";
            vehicle.Myvehicle.VehicleNumber = " ";
            vehicle.Myvehicle.Year = 0;
            vehicle.Myvehicle.VehicleType = " ";

            vehicle.UserID = Convert.ToInt32(Session["UserID"]);
            vehicle.UserName = Session["UserName"].ToString();
            vehicle.FirstName = Session["FirstName"].ToString();
            vehicle.LastName = Session["LastName"].ToString();
            vehicle.Address = Session["Address"].ToString();
            vehicle.Mobile = Session["Mobile"].ToString();
            vehicle.EmailID = Session["Email"].ToString();           

            vehicle.VehicleList = vController.GetAllVehicle();

            vehicle.Listhistory = scontroller.GetHistory(vehicle.UserID);


            ViewBag.Message = message;
            ViewBag.Status = Status;
            ViewBag.Type = type;
            ViewBag.Button = button;
            return View("Profile", vehicle);

        }

        //-------------------------Delete ------------------------

        [HttpGet]
        public ActionResult DeleteVehicle(string id)
        {
            bool Status = false;
            string message = "";
            bool type = false;
           
            vController = new VehicleController();
            scontroller = new ServiceController();
            Newuser vehicle = new Newuser();

            try
            {
                using (MyDatabaseEntities dc = new MyDatabaseEntities())
                {
                    int vehicleid = Convert.ToInt32(id);

                    vehicle.Myvehicle = (from c in dc.Vehicles
                           where c.VehicleID == vehicleid
                           select c).FirstOrDefault();

                    dc.Vehicles.Remove(vehicle.Myvehicle);
                    dc.SaveChanges();

                    message = " Vehicle deleted successfully ";
                    Status = true;
                    type = false;
                }
            }

            catch (Exception ex)
            {
                message = "Vehicle not deleted";
                Status = true;
                type = true;
            }


            ModelState.Clear();
            vehicle.Myvehicle.VehicleModel = " ";
            vehicle.Myvehicle.VehicleNumber = " ";
            vehicle.Myvehicle.Year = 0;
            vehicle.Myvehicle.VehicleType = " ";

            vehicle.UserID = Convert.ToInt32(Session["UserID"]);
            vehicle.UserName = Session["UserName"].ToString();
            vehicle.FirstName = Session["FirstName"].ToString();
            vehicle.LastName = Session["LastName"].ToString();
            vehicle.Address = Session["Address"].ToString();
            vehicle.Mobile = Session["Mobile"].ToString();
            vehicle.EmailID = Session["Email"].ToString();           

            vehicle.VehicleList = vController.GetAllVehicle();

            vehicle.Listhistory = scontroller.GetHistory(vehicle.UserID);

            ViewBag.Message = message;
            ViewBag.Status = Status;
            ViewBag.Type = type;
            ViewBag.Button  = "add";
            return View("Profile", vehicle);
        }

        //-----------------returns list of all user detais for required places-----------------------------
        public List<User> GetAllUser()
        {
            List<User> UserList = new List<User>();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                User user;
                var loaduser = (from c in dc.Users
                                orderby c.FirstName ascending
                                orderby c.UserGroup descending
                                select c).ToList();
                foreach (var i in loaduser)
                {
                    user = new User();
                    user.UserID = i.UserID;
                    user.FirstName = i.FirstName;
                    user.LastName = i.LastName;
                    user.EmailID = i.EmailID;
                    user.Address = i.Address;
                    user.Mobile = i.Mobile;
                    user.UserGroup = i.UserGroup;
                    user.UserName = i.UserName;

                    UserList.Add(user);
                }
            }
            return UserList;
        }
    }
}