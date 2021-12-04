using Amms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
//using Amms.Models.Helpers;
using System.IO;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Web.Script.Serialization;


namespace Amms.Controllers
{

    public class AdminController : Controller
    {
        UnitController uController;
        CategoryController cController;
        ServiceController sController;
        ShopController shopController;
        UserController userController;
        VehicleController vController;
        ResevationController rcontroller;
        InvoiceController icontroller;
        //UserHelper hUser;

        //  Admin admin = new Admin();

        string button;
        int updateunitid;
        string image;

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        //Admin page called by _Nav
        public ActionResult Admin()
        {
            return View();
        }


        //--------------------------------------------------User------------------------------------------------------------------//

        // ----------------------------------Add-------------------------

        [HttpGet]
        public ActionResult AddUser()
        {
            Admin user = new Admin();

            button = "add";

            userController = new UserController();
            user.UserList = userController.GetAllUser();

            var list = new SelectList(new[]
            {
                new { ID = "User", Name = "User" },
                new { ID = "Operator", Name = "Operator" },
                new { ID = "Admin", Name = "Admin" },
            },
                "ID", "Name", 1);

            ViewData["list"] = list;

            ViewBag.Button = button;
            return View("Registration", user);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser([Bind(Exclude = "IsEmailVerified,ActivationCode")] Admin user)
        {
            userController = new UserController();

            bool Status = false;
            string message = "";
            //
            // Model Validation

            if (ModelState.IsValid)
            {
                #region //Email is already Exist 
                var isExist = IsEmailExist(user.Myuser.EmailID);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View(user);
                }
                #endregion

                #region Generate Activation Code 
                user.Myuser.ActivationCode = Guid.NewGuid();
                #endregion

                #region  Password Hashing 
                user.Myuser.Password = Crypto.Hash(user.Myuser.Password);
                user.Myuser.ConfirmPassword = Crypto.Hash(user.Myuser.ConfirmPassword);
                #endregion
                user.Myuser.IsEmailVerified = false;
                user.Myuser.UserName = user.Myuser.EmailID;
                user.Myuser.IsEmailVerified = true;

                #region Save to Database
                using (MyDatabaseEntities dc = new MyDatabaseEntities())
                {
                    try
                    {
                        dc.Users.Add(user.Myuser);
                        dc.SaveChanges();

                        //Send Email to User
                        //SendVerificationLinkEmail(user.Myuser.EmailID, user.Myuser.ActivationCode.ToString());
                        message = "Registration successfully done, User ID : " + user.Myuser.EmailID;

                        Status = true;
                    }
                    catch (Exception e)
                    {

                    }
                }
                #endregion
            }
            else
            {
                message = "Invalid Request";
            }


            user.UserList = userController.GetAllUser();

            var list = new SelectList(new[]
            {
                new { ID = "User", Name = "User" },
                new { ID = "Operator", Name = "Operator" },
                new { ID = "Admin", Name = "Admin" },
            },
                "ID", "Name", 1);

            ViewData["list"] = list;

            ModelState.Clear();
            user.Myuser.FirstName = " ";
            user.Myuser.LastName = " ";
            user.Myuser.Address = " ";
            user.Myuser.Mobile = " ";
            user.Myuser.EmailID = " ";


            ViewBag.Message = message;
            ViewBag.Status = Status;
            ViewBag.Button = "add";
            return View("Registration", user);
        }

        //----------------check whether the email is exsting-------------------
        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                var v = dc.Users.Where(a => a.EmailID == emailID).FirstOrDefault();
                return v != null;
            }
        }

        //-------------------------send an email to users email-------------------
        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode)
        {
            var verifyUrl = "/User/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("asmsystme@gmail.com", "Automobile Service Station Management System");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "PassworD1."; // Replace with actual password
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

        //----------------------Load and Update----------------------


        [HttpGet]
        public ActionResult UpdateUser(string id)
        {
            button = "update";

            Admin user = new Admin();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                int userid = Convert.ToInt32(id);
                user.Myuser = (from c in dc.Users
                               where c.UserID == userid
                               select c).FirstOrDefault();
            }

            userController = new UserController();
            user.UserList = userController.GetAllUser();

            var list = new SelectList(new[]
            {
                new { ID = "User", Name = "User" },
                new { ID = "Operator", Name = "Operator" },
                new { ID = "Admin" , Name = "Admin" },
            },
              "ID", "Name", 1);

            ViewData["list"] = list;

            ViewBag.Button = button;

            return View("Registration", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUser(Admin user)
        {
            bool Status = false;
            string message = "";
            bool type = false;

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                dc.Entry(user.Myuser).State = EntityState.Modified;
                dc.SaveChanges();

                message = " User updated successfully";
                Status = true;
            }

            userController = new UserController();
            user.UserList = userController.GetAllUser();

            var list = new SelectList(new[]
          {
                new { ID = "User", Name = "User" },
                new { ID = "Operator", Name = "Operator" },
                new { ID = "Admin" , Name = "Admin" },
            },
              "ID", "Name", 1);

            ViewData["list"] = list;

            ModelState.Clear();
            user.Myuser.FirstName = " ";
            user.Myuser.LastName = " ";
            user.Myuser.Address = " ";
            user.Myuser.Mobile = " ";
            user.Myuser.EmailID = " ";

            ViewBag.Message = message;
            ViewBag.Status = Status;
            ViewBag.Type = type;
            button = "add";
            return View("AddUnit", user);
        }


        //--------------------Delete------------------------
        [HttpGet]
        public ActionResult DeleteUser(string id)
        {
            bool Status = false;
            string message = "";
            bool type = false;
            Admin user = new Admin();

            try
            {
                using (MyDatabaseEntities dc = new MyDatabaseEntities())
                {
                    int userid = Convert.ToInt32(id);
                    user.Myuser = (from c in dc.Users
                                   where c.UserID == userid
                                   select c).FirstOrDefault();
                    dc.Users.Remove(user.Myuser);
                    dc.SaveChanges();

                    message = " User deleted successfully ";
                    Status = true;
                    type = false;
                }
            }

            catch (Exception ex)
            {
                message = "User not deleted";
                Status = true;
                type = true;

            }

            userController = new UserController();
            user.UserList = userController.GetAllUser();

            var list = new SelectList(new[]
          {
                new { ID = "User", Name = "User" },
                new { ID = "Operator", Name = "Operator" },
                new { ID = "Admin", Name = "Admin" },
            },
              "ID", "Name", 1);

            ViewData["list"] = list;

            ModelState.Clear();
            user.Myuser.FirstName = " ";
            user.Myuser.LastName = " ";
            user.Myuser.Address = " ";
            user.Myuser.Mobile = " ";
            user.Myuser.EmailID = " ";

            ViewBag.Message = message;
            ViewBag.Status = Status;
            ViewBag.Type = type;
            return View("Registration", user);
        }



        //-----------------------------------------Items---------------------------------------------------------------------//

        //----------------------Add-------------------------


        [HttpGet]
        public ActionResult AddItem()
        {
            button = "add";

            //Objects of other used controllers
            uController = new UnitController();
            cController = new CategoryController();
            shopController = new ShopController();

            Admin vitem = new Admin();

            try
            {
                vitem.UnitList = uController.GetAllUnit();  //Accessing Unit controller and bind units to the dropdown
                Dictionary<int, string> dropList = new Dictionary<int, string>();

                foreach (var i in vitem.UnitList)
                {
                    dropList.Add(i.UnitID, i.UnitName);
                }

                vitem.CategoryList = cController.GetAllCategory();  //Accessing Category controller and bind units to the dropdown
                Dictionary<int, string> dropList1 = new Dictionary<int, string>();

                foreach (var k in vitem.CategoryList)
                {
                    dropList1.Add(k.CategoryID, k.CategoryName);
                }

                vitem.ItemList = shopController.GetAllItem();
                vitem.UnitDropList = dropList;
                vitem.CategoryDropList = dropList1;
            }

            catch
            {

            }
            ViewBag.Button = button;
            return View("AddItem", vitem);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddItem(Admin vitem, HttpPostedFileBase file) //HttpPostedFile used for image upload
        {
            uController = new UnitController();
            cController = new CategoryController();
            shopController = new ShopController();

            bool Status = false;
            string message = "";

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                if (file != null)
                {
                    string path = Path.Combine(Server.MapPath("~/Content/images/shop/grid"), Path.GetFileName(file.FileName));
                    file.SaveAs(path);

                    vitem.Myitem.ItemImage = Path.GetFileName(file.FileName).ToString();
                }
                try
                {
                    dc.Items.Add(vitem.Myitem);
                    dc.SaveChanges();

                    message = "Item added succesfully";
                    Status = true;
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }

                    }
                    throw;

                }
            }

            ModelState.Clear();
            vitem.Myitem.ItemName = " ";
            vitem.Myitem.ItemDescription = " ";
            vitem.Myitem.ItemPrice = 0;

            //Drop down reset is not working-------------
            vitem.UnitList = uController.GetAllUnit();
            Dictionary<int, string> dropList = new Dictionary<int, string>();
            foreach (var i in vitem.UnitList)
            {
                dropList.Add(i.UnitID, i.UnitName);
            }

            vitem.CategoryList = cController.GetAllCategory();
            Dictionary<int, string> dropList1 = new Dictionary<int, string>();
            foreach (var k in vitem.CategoryList)
            {
                dropList1.Add(k.CategoryID, k.CategoryName);
            }

            vitem.ItemList = shopController.GetAllItem();
            vitem.UnitDropList = dropList;
            vitem.CategoryDropList = dropList1;

            ViewBag.Message = message;
            ViewBag.Status = Status;
            ViewBag.Button = "add";
            return View("Additem", vitem);
        }


        //-------------------Load and Update------------------

        [HttpGet]
        public ActionResult UpdateItem(string id)
        {
            uController = new UnitController();
            cController = new CategoryController();
            shopController = new ShopController();

            button = "update";
            image = "load";

            Admin vitem = new Admin();
            int itemid;

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                itemid = Convert.ToInt32(id);
                vitem.Myitem = (from c in dc.Items
                                where c.ItemID == itemid
                                select c).FirstOrDefault();

                // shopController = new ShopController();

                vitem.UnitList = uController.GetAllUnit();  //Accessing Unit controller and bind units to the dropdown
                Dictionary<int, string> dropList = new Dictionary<int, string>();

                foreach (var i in vitem.UnitList)
                {
                    dropList.Add(i.UnitID, i.UnitName);
                }

                vitem.CategoryList = cController.GetAllCategory();  //Accessing Category controller and bind units to the dropdown
                Dictionary<int, string> dropList1 = new Dictionary<int, string>();

                foreach (var k in vitem.CategoryList)
                {
                    dropList1.Add(k.CategoryID, k.CategoryName);
                }

                vitem.UnitDropList = dropList;
                vitem.CategoryDropList = dropList1;
                vitem.ItemList = shopController.GetAllItem();
                button = "update";
            }
            ViewBag.Button = button;
            ViewBag.Image = image;
            return View("AddItem", vitem);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateItem(Admin vitem, HttpPostedFileBase file)
        {
            button = "update";
            image = "save";

            uController = new UnitController();
            cController = new CategoryController();
            shopController = new ShopController();

            bool Status = false;
            string message = "";
            bool type = false;

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                if (file != null)
                {
                    string path = Path.Combine(Server.MapPath("~/Content/images/shop/grid"), Path.GetFileName(file.FileName));
                    file.SaveAs(path);

                    vitem.Myitem.ItemImage = Path.GetFileName(file.FileName).ToString();
                }

                try
                {

                    dc.Entry(vitem.Myitem).State = EntityState.Modified;
                    dc.SaveChanges();

                    message = " Item updated successfully";
                    Status = true;
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }

            ModelState.Clear();
            vitem.Myitem.ItemName = " ";
            vitem.Myitem.ItemDescription = " ";
            vitem.Myitem.ItemPrice = 0;

            //Drop down reset is not working-------------
            vitem.UnitList = uController.GetAllUnit();
            Dictionary<int, string> dropList = new Dictionary<int, string>();
            foreach (var i in vitem.UnitList)
            {
                dropList.Add(i.UnitID, i.UnitName);
            }

            vitem.CategoryList = cController.GetAllCategory();
            Dictionary<int, string> dropList1 = new Dictionary<int, string>();
            foreach (var k in vitem.CategoryList)
            {
                dropList1.Add(k.CategoryID, k.CategoryName);
            }

            vitem.ItemList = shopController.GetAllItem();
            vitem.UnitDropList = dropList;
            vitem.CategoryDropList = dropList1;

            ViewBag.Message = message;
            ViewBag.Status = Status;
            ViewBag.Type = type;
            ViewBag.Image = image;
            ViewBag.Button = "add";
            return View("Additem", vitem);
        }



        //-----------------Delete---------------------        

        [HttpGet]
        public ActionResult DeleteItem(string id)
        {
            uController = new UnitController();
            cController = new CategoryController();
            shopController = new ShopController();

            bool Status = false;
            string message = "";
            bool type = false;

            Admin vitem = new Admin();
            try
            {
                using (MyDatabaseEntities dc = new MyDatabaseEntities())
                {
                    int itemid = Convert.ToInt32(id);
                    vitem.Myitem = (from c in dc.Items
                                    where c.ItemID == itemid
                                    select c).FirstOrDefault();
                    dc.Items.Remove(vitem.Myitem);
                    dc.SaveChanges();

                    message = " Item deleted successfully ";
                    Status = true;
                    type = false;
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.ToString() == "An error occurred while updating the entries. See the inner exception for details.")
                {
                    message = "This item has a reference";
                    Status = true;
                    type = true;
                }
                else
                {
                    message = "Item not deleted";
                    Status = true;
                    type = true;
                }
            }

            ModelState.Clear();
            vitem.Myitem.ItemName = " ";
            vitem.Myitem.ItemDescription = " ";
            vitem.Myitem.ItemPrice = 0;

            vitem.UnitList = uController.GetAllUnit();
            Dictionary<int, string> dropList = new Dictionary<int, string>();
            foreach (var i in vitem.UnitList)
            {
                dropList.Add(i.UnitID, i.UnitName);
            }

            vitem.CategoryList = cController.GetAllCategory();
            Dictionary<int, string> dropList1 = new Dictionary<int, string>();
            foreach (var k in vitem.CategoryList)
            {
                dropList1.Add(k.CategoryID, k.CategoryName);
            }

            vitem.ItemList = shopController.GetAllItem();
            vitem.UnitDropList = dropList;
            vitem.CategoryDropList = dropList1;

            ViewBag.Message = message;
            ViewBag.Status = Status;
            ViewBag.Type = type;
            ViewBag.Button = "add";

            return View("AddItem", vitem);
        }

        //-----------------------------------------Unit ---------------------------------------------------------------------//

        //------------------Add--------------------------

        [HttpGet]
        public ActionResult AddUnit()
        {
            button = "add";

            Admin unit = new Admin();
            uController = new UnitController();
            unit.UnitList = uController.GetAllUnit();

            ViewBag.Button = button;

            return View("AddUnit", unit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUnit(Admin unit)
        {
            bool Status = false;
            string message = "";

            //if (ModelState.IsValid)
            //{
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                try
                {
                    dc.Units.Add(unit.Myunit);
                    dc.SaveChanges();
                    int id = unit.Myunit.UnitID;

                    message = " Unit added successfully";
                    Status = true;
                }

                catch (DbEntityValidationException e)
                {
                    message = " Unit not added";
                }
            }
            //}

            ViewBag.Message = message;
            ViewBag.Status = Status;
            ViewBag.Button = "add";

            ModelState.Clear();
            unit.Myunit.UnitName = " ";
            unit.Myunit.UnitDescription = " ";

            uController = new UnitController();
            unit.UnitList = uController.GetAllUnit();

            return View("AddUnit", unit);
        }

        //----------------Load and Update-----------------------

        [HttpGet]
        public ActionResult UpdateUnit(string id)
        {
            button = "update";

            Admin unit = new Admin();
            int uid;
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                uid = Convert.ToInt32(id);
                unit.Myunit = (from c in dc.Units
                               where c.UnitID == uid
                               select c).FirstOrDefault();

                uController = new UnitController();
                unit.UnitList = uController.GetAllUnit();
            }
            ViewBag.Button = button;
            //  updateunitid = uid;
            return View("AddUnit", unit);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUnit(Admin unit)
        {
            bool Status = false;
            string message = "";
            bool type = false;
            //Admin admin = new Admin();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                try
                {
                    dc.Entry(unit.Myunit).State = EntityState.Modified;
                    dc.SaveChanges();

                    message = " Unit updated successfully";
                    Status = true;
                }

                catch (DbEntityValidationException e)
                {

                }
            }

            ModelState.Clear();
            unit.Myunit.UnitName = " ";
            unit.Myunit.UnitDescription = " ";

            ViewBag.Message = message;
            ViewBag.Status = Status;
            ViewBag.Type = type;

            ViewBag.Button = "add";

            uController = new UnitController();
            unit.UnitList = uController.GetAllUnit();

            return View("AddUnit", unit);
        }


        //----------------Delete-----------------------

        [HttpGet]
        public ActionResult DeleteUnit(string id)
        {
            bool Status = false;
            string message = "";
            bool type = false;
            //  Unit unit = new Unit();
            Admin unit = new Admin();
            try
            {
                using (MyDatabaseEntities dc = new MyDatabaseEntities())
                {
                    int uid = Convert.ToInt32(id);
                    unit.Myunit = (from c in dc.Units
                                   where c.UnitID == uid
                                   select c).FirstOrDefault();
                    dc.Units.Remove(unit.Myunit);
                    dc.SaveChanges();

                    message = " Unit deleted successfully ";
                    Status = true;
                    type = false;
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.ToString() == "An error occurred while updating the entries. See the inner exception for details.")
                {
                    message = "This unit has a reference";
                    Status = true;
                    type = true;
                }
                else
                {
                    message = "Unit not deleted";
                    Status = true;
                    type = true;
                }
            }

            ModelState.Clear();
            unit.Myunit.UnitName = " ";
            unit.Myunit.UnitDescription = " ";

            ViewBag.Message = message;
            ViewBag.Status = Status;
            ViewBag.Type = type;

            ViewBag.Button = "add";

            uController = new UnitController();
            unit.UnitList = uController.GetAllUnit();

            return View("AddUnit", unit);
        }


        //--------------------------------------------------- Category -----------------------------------------------------//

        //---------------Add-----------------------
        [HttpGet]
        public ActionResult AddCategory()
        {
            button = "add";

            cController = new CategoryController();

            Admin cat = new Admin();
            cat.CategoryList = cController.GetAllCategory();
            ViewBag.Button = button;
            return View("AddCategory", cat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory(Admin cat)
        {
            bool Status = false;
            string message = "";

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                try
                {
                    dc.Categories.Add(cat.Mycategory);
                    dc.SaveChanges();

                    message = " Category added successfully ";
                    Status = true;
                }

                catch (DbEntityValidationException e)
                {

                }
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            ViewBag.Button = "add";

            ModelState.Clear();
            cat.Mycategory.CategoryName = " ";
            cat.Mycategory.CategoryDescription = " ";

            cController = new CategoryController();
            cat.CategoryList = cController.GetAllCategory();

            return View("AddCategory", cat);
        }

        //---------------------Load and Update-----------------

        [HttpGet]
        public ActionResult UpdateCategory(string id)
        {
            button = "update";

            Admin cat = new Admin();
            int catid;
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                catid = Convert.ToInt32(id);
                cat.Mycategory = (from c in dc.Categories
                                  where c.CategoryID == catid
                                  select c).FirstOrDefault();

                cController = new CategoryController();
                cat.CategoryList = cController.GetAllCategory();
            }
            ViewBag.Button = button;
            // updateunitid = uid;
            return View("AddCategory", cat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCategory(Admin cat)
        {

            bool Status = false;
            string message = "";
            bool type = false;

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                try
                {
                    dc.Entry(cat.Mycategory).State = EntityState.Modified;
                    dc.SaveChanges();

                    message = " Category updated successfully";
                    Status = true;
                }

                catch (DbEntityValidationException e)
                {

                }
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            ViewBag.Type = type;
            ViewBag.Button = "add";

            ModelState.Clear();
            cat.Mycategory.CategoryName = " ";
            cat.Mycategory.CategoryDescription = " ";

            cController = new CategoryController();
            cat.CategoryList = cController.GetAllCategory();

            return View("AddCategory", cat);

        }


        //---------------------Delete--------------------------

        [HttpGet]
        public ActionResult DeleteCategory(string id)
        {
            bool Status = false;
            string message = "";
            bool type = false;
            //Category cat = new Category();
            Admin cat = new Admin();

            try
            {
                using (MyDatabaseEntities dc = new MyDatabaseEntities())
                {
                    int catid = Convert.ToInt32(id);
                    cat.Mycategory = (from c in dc.Categories
                                      where c.CategoryID == catid
                                      select c).FirstOrDefault();
                    dc.Categories.Remove(cat.Mycategory);
                    dc.SaveChanges();

                    message = " Category deleted successfully ";
                    Status = true;
                    type = false;
                }
            }

            catch (Exception ex)
            {

                message = "Category not deleted";
                Status = true;
                type = true;
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            ViewBag.Type = type;
            ViewBag.Button = "add";

            ModelState.Clear();
            cat.Mycategory.CategoryName = " ";
            cat.Mycategory.CategoryDescription = " ";

            cController = new CategoryController();
            cat.CategoryList = cController.GetAllCategory();

            return View("AddCategory", cat);
        }

        //------------------------------------------------Service -----------------------------------------------------------//

        //-----------------------Add-------------------    

        [HttpGet]
        public ActionResult AddService()
        {
            button = "add";

            sController = new ServiceController();

            Admin serv = new Admin();
            try
            {
                serv.ServiceList = sController.GetAllService();
            }
            catch
            {

            }
            ViewBag.Button = button;
            return View("AddService", serv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddService(Admin serv, HttpPostedFileBase file)
        {
            button = "add";
            image = "save";

            sController = new ServiceController();

            bool Status = false;
            string message = "";
            bool type = false;

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                if (file != null)
                {
                    string path = Path.Combine(Server.MapPath("~/Content/images/service"), Path.GetFileName(file.FileName));
                    file.SaveAs(path);

                    serv.Myservice.ServiceImage = Path.GetFileName(file.FileName).ToString();
                }

                try
                {
                    dc.Services.Add(serv.Myservice);
                    dc.SaveChanges();
                    message = " Service added successfully ";
                    Status = true;
                }

                catch (DbEntityValidationException e)
                {
                    if (e.Message == "Parameter value  is out of range.")
                    {
                        message = "Invalid Caharacters Found";
                    }
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }

            ModelState.Clear();
            serv.Myservice.ServiceImage = " ";
            serv.Myservice.ServiceName = " ";
            serv.Myservice.ServiceDescription = " ";
            serv.Myservice.ServicePrice = 0;
            serv.Myservice.ServiceTime = " ";

            ViewBag.Message = message;
            ViewBag.Status = Status;
            ViewBag.Button = "add";

            serv.ServiceList = sController.GetAllService();

            return View("AddService", serv);
        }


        //-----------------------Load and Update---------------

        [HttpGet]
        public ActionResult UpdateService(string id)
        {
            button = "update";
            image = "load";
            Admin serv = new Admin();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                int sid = Convert.ToInt32(id);
                serv.Myservice = (from c in dc.Services
                                  where c.ServiceID == sid
                                  select c).FirstOrDefault();

                sController = new ServiceController();
                try
                {
                    serv.ServiceList = sController.GetAllService();
                }
                catch
                {

                }
            }
            ViewBag.Button = button;
            ViewBag.Image = image;
            return View("AddService", serv);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateService(Admin serv, HttpPostedFileBase file)
        {
            bool Status = false;
            string message = "";
            bool type = false;

            sController = new ServiceController();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                if (file != null)
                {
                    string path = Path.Combine(Server.MapPath("~/Content/images/service"), Path.GetFileName(file.FileName));
                    file.SaveAs(path);

                    serv.Myservice.ServiceImage = Path.GetFileName(file.FileName).ToString();
                }
                try
                {
                    dc.Entry(serv.Myservice).State = EntityState.Modified;
                    dc.SaveChanges();

                    message = " Service updated successfully";
                    Status = true;
                }

                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }

            }

            ModelState.Clear();
            serv.Myservice.ServiceName = " ";
            serv.Myservice.ServiceDescription = " ";
            serv.Myservice.ServicePrice = 0;
            serv.Myservice.ServiceTime = " ";

            ViewBag.Message = message;
            ViewBag.Status = Status;
            ViewBag.Type = type;
            ViewBag.Button = "add";

            serv.ServiceList = sController.GetAllService();
            return View("AddService", serv);
        }

        //-----------------------Delete-----------------------

        [HttpGet]
        public ActionResult DeleteService(string id)
        {
            bool Status = false;
            string message = "";
            bool type = false;
            Admin serv = new Models.Admin();

            try
            {
                using (MyDatabaseEntities dc = new MyDatabaseEntities())
                {
                    int sid = Convert.ToInt32(id);
                    serv.Myservice = (from c in dc.Services
                                      where c.ServiceID == sid
                                      select c).FirstOrDefault();
                    dc.Services.Remove(serv.Myservice);
                    dc.SaveChanges();

                    message = " Service deleted successfully ";
                    Status = true;
                    type = false;
                }
            }

            catch (Exception ex)
            {

                message = "Service not deleted";
                Status = true;
                type = true;
            }

            ModelState.Clear();
            serv.Myservice.ServiceName = " ";
            serv.Myservice.ServicePrice = 0;
            serv.Myservice.ServiceDescription = " ";
            serv.Myservice.ServiceTime = " ";
            serv.Myservice.ServiceImage = " ";

            sController = new ServiceController();
            serv.ServiceList = sController.GetAllService();

            ViewBag.Message = message;
            ViewBag.Status = Status;
            ViewBag.Type = type;
            ViewBag.Button = "add";
            return View("AddService", serv);
        }

        [HttpGet]
        public ActionResult Stock()
        {
            return View();
        }

        //-----------------------------------------Profile--------------------------------------------------

        //----------------------------------Load user Details and add vehicle------------------------------

        [HttpGet]

        public ActionResult Profile(string id)
        {
            button = "add";
            vController = new VehicleController();
            User users = new User();
            Admin user = new Admin();

            int uid = Convert.ToInt32(id);

            if (id == null)
            {
                uid = 1;
            }
            else
            {

            }

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                users = (from c in dc.Users
                         where c.UserID == uid
                         select c).FirstOrDefault();

                user.Myuser = users;
                user.VehicleList = vController.GetAllVehicle();
            }

            ViewBag.Button = button;
            return View("Profile", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Profile(Admin vehicle)
        {
            bool Status = false;
            string message = "";
            bool type = false;

            User user = new User();
            vController = new VehicleController();
            int uid = Convert.ToInt32(vehicle.Myuser.UserID);

            try
            {
                using (MyDatabaseEntities dc = new MyDatabaseEntities())
                {
                    vehicle.Myvehicle.UserID = uid;

                    dc.Vehicles.Add(vehicle.Myvehicle);
                    dc.SaveChanges();

                    message = " Vehicle added successfully";
                    Status = true;

                    user = (from c in dc.Users
                            where c.UserID == uid
                            select c).FirstOrDefault();
                }
            }

            catch (Exception ex)
            {
                message = "Vehicle not added";
                Status = true;
                type = true;
            }

            ModelState.Clear();
            vehicle.Myvehicle.VehicleModel = " ";
            vehicle.Myvehicle.VehicleNumber = " ";
            vehicle.Myvehicle.Year = 0;
            vehicle.Myvehicle.VehicleType = " ";

            vehicle.Myuser = user;
            vehicle.VehicleList = vController.GetAllVehicle();

            ViewBag.Message = message;
            ViewBag.Status = Status;
            ViewBag.Button = "add";
            return View("Profile", vehicle);

        }

        //---------Load and Update------------------

        [HttpGet]
        public ActionResult UpdateVehicle(string id)
        {
            button = "update";

            Admin vehicle = new Admin();
            User user = new User();
            vController = new VehicleController();

            int uid;
            int vehicleid;

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                vehicleid = Convert.ToInt32(id);

                vehicle.Myvehicle = (from c in dc.Vehicles
                                     where c.VehicleID == vehicleid
                                     select c).FirstOrDefault();

                uid = Convert.ToInt32(vehicle.Myvehicle.UserID);

                user = (from c in dc.Users
                        where c.UserID == uid
                        select c).FirstOrDefault();
            }

            vehicle.Myuser = user;
            vehicle.VehicleList = vController.GetAllVehicle();
            ViewBag.Button = button;
            return View("Profile", vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateVehicle(Admin vehicle)
        {
            bool Status = false;
            string message = "";
            bool type = false;
            button = "add";
            vController = new VehicleController();
            User user = new User();

            int uid;

            try
            {
                using (MyDatabaseEntities dc = new MyDatabaseEntities())
                {
                    uid = Convert.ToInt32(vehicle.Myuser.UserID);
                    vehicle.Myvehicle.UserID = uid;

                    dc.Entry(vehicle.Myvehicle).State = EntityState.Modified;
                    dc.SaveChanges();

                    user = (from c in dc.Users
                            where c.UserID == uid
                            select c).FirstOrDefault();

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

            vehicle.Myuser = user;
            vehicle.VehicleList = vController.GetAllVehicle();

            ViewBag.Message = message;
            ViewBag.Status = Status;
            ViewBag.Type = type;
            ViewBag.Button = button;
            return View("Profile", vehicle);

        }

        //---------Delete ------------------------

        [HttpGet]
        public ActionResult DeleteVehicle(string id)
        {
            bool Status = false;
            string message = "";
            bool type = false;

            vController = new VehicleController();

            User user = new User();
            Admin vehicle = new Admin();
            int uid;

            try
            {
                using (MyDatabaseEntities dc = new MyDatabaseEntities())
                {
                    int vehicleid = Convert.ToInt32(id);

                    vehicle.Myvehicle = (from c in dc.Vehicles
                                         where c.VehicleID == vehicleid
                                         select c).FirstOrDefault();

                    uid = Convert.ToInt32(vehicle.Myvehicle.UserID);

                    dc.Vehicles.Remove(vehicle.Myvehicle);
                    dc.SaveChanges();

                    user = (from c in dc.Users
                            where c.UserID == uid
                            select c).FirstOrDefault();

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

            vehicle.Myuser = user;
            vehicle.VehicleList = vController.GetAllVehicle();

            ViewBag.Message = message;
            ViewBag.Status = Status;
            ViewBag.Type = type;
            ViewBag.Button = "add";
            return View("Profile", vehicle);
        }


        // -------------------------------Reservations--------------------------------

        //-------Load all reservations--------------
        [HttpGet]
        public ActionResult GetAllReservations()
        {
            Admin app = new Admin();
            rcontroller = new ResevationController();

            app.ListAppoinments = rcontroller.GetAllAppointments();
            return View("Appointments", app);
        }


        //------------View available time slots by admin

        [HttpGet]
        public ActionResult ViewAvailableTimeSlots()
        {
            return View("ViewReservations");
        }


        //----------update pending -> confirmed----------------------------
        [HttpPost]
        public ActionResult UpdateReservation(string id)
        {
            bool confirm = false;
            Admin app = new Admin();
            int appid = Convert.ToInt32(id);
            rcontroller = new ResevationController();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                app.Myappointment = (from c in dc.Appointments
                                     where c.AppointmentID == appid
                                     select c).FirstOrDefault();

                app.Myappointment.AppointmentStatus = "Confirmed";

                int uid = app.Myappointment.UserID;


                dc.Entry(app.Myappointment).State = EntityState.Modified;
                dc.SaveChanges();

                confirm = true;

                if (confirm == true)
                {
                    ConfirmReservation(uid, appid);
                }
            }
            confirm = false;
            app.ListAppoinments = rcontroller.GetAllAppointments();
            return View("Appointments", app);
        }

        //----------cancel/delete reservation----------------

        [HttpGet]
        public ActionResult DeleteReservation(string id)
        {
            string message;
            bool type = false;
            Admin app = new Admin();
            int appid = Convert.ToInt32(id);
            rcontroller = new ResevationController();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                app.Myappointment = (from c in dc.Appointments
                                     where c.AppointmentID == appid
                                     select c).FirstOrDefault();

                dc.Appointments.Remove(app.Myappointment);
                dc.SaveChanges();
                message = "Reservation Deleted Sucesfully";
                type = true;
            }
            app.ListAppoinments = rcontroller.GetAllAppointments();

            ViewBag.Message = message;
            ViewBag.Type = type;
            return View("Appointments", app);
        }

        //--------------------------List all pending reservations---------------------
        public ActionResult PendingReservations()
        {
            Admin app = new Admin();
            Appointment appointment;
            List<Appointment> ListApp = new List<Appointment>();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                var loadservice = (from c in dc.Appointments
                                   where c.AppointmentStatus == "Pending"
                                   select c).ToList();
                foreach (var ser in loadservice)
                {
                    appointment = new Appointment();
                    appointment.AppointmentID = ser.AppointmentID;
                    appointment.AppointmentDate = ser.AppointmentDate;
                    appointment.AppointmentStartTime = ser.AppointmentStartTime;
                    appointment.AppointmentEndTime = ser.AppointmentEndTime;
                    appointment.AppointmentStatus = ser.AppointmentStatus;

                    ListApp.Add(appointment);
                }
            }
            app.ListAppoinments = ListApp;

            return View("Appointments", app);
        }

        //------------------List all confirmed reservations----------------------
        public ActionResult ConfirmedReservations()
        {
            Admin app = new Admin();
            Appointment appointment;
            List<Appointment> ListApp = new List<Appointment>();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                var loadservice = (from c in dc.Appointments
                                   where c.AppointmentStatus == "Confirmed"
                                   select c);
                foreach (var ser in loadservice)
                {
                    appointment = new Appointment();
                    appointment.AppointmentID = ser.AppointmentID;
                    appointment.AppointmentDate = ser.AppointmentDate;
                    appointment.AppointmentStartTime = ser.AppointmentStartTime;
                    appointment.AppointmentEndTime = ser.AppointmentEndTime;
                    appointment.AppointmentStatus = ser.AppointmentStatus;

                    ListApp.Add(appointment);
                }
            }
            app.ListAppoinments = ListApp;

            return View("Appointments", app);
        }

        //-----------------Send confirmation mail to customers---------------------------------------

        [NonAction]
        public void ConfirmReservation(int uid, int appid)
        {
            User user = new User();
            Appointment app = new Appointment();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                user.EmailID = (from c in dc.Users
                                where c.UserID == uid
                                select c.EmailID).FirstOrDefault();

                app.AppointmentDate = (from c in dc.Appointments
                                       where c.AppointmentID == appid
                                       select c.AppointmentDate).FirstOrDefault();

                app.AppointmentStartTime = (from c in dc.Appointments
                                            where c.AppointmentID == appid
                                            select c.AppointmentStartTime).FirstOrDefault();
            }

            var verifyUrl = "/User/VerifyAccount/";
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("automobileservicesms@gmail.com", "Automobile Service Station Management System"); // System email address
            var toEmail = new MailAddress(user.EmailID);

            var fromEmailPassword = "PassworD1."; // Can replace this with actual password

            string subject = "Online Reservation ";

            string body = "<br/>Your Reservation on " + app.AppointmentDate.ToShortDateString() + "  " + app.AppointmentStartTime.ToString() + " " + "is Placed Successfully. Please be on time. Thank you";

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


        //-----------------------Invoices-------------------

        //-------------------View all invoices by admin-----------------------
        public ActionResult ViewAllInvoices()
        {
            Admin app = new Admin();
            icontroller = new InvoiceController();

            app.Listinvoices = icontroller.Getinvoice();

            return View("AdminInvoices", app);//hadanna
        }

    }
}
