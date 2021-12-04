using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;
using System.Text;
using System.Web;
//using iTextSharp.text;
//using iTextSharp.text.pdf;
//using iTextSharp.text.html.simpleparser;
using Amms.Models;
using System.Web.UI;
using System.Data.Entity;

namespace Amms.Controllers
{
    public class InvoiceController : Controller
    {
        ServiceController sController;
        ShopController shopController;
        UnitController ucontroller;
       
        // GET: Invoice

        public List<InvoiceDetail> Getallinvoices(int id,out decimal sum)
        {
            List<InvoiceDetail> Listdetails = new List<InvoiceDetail>();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                Listdetails = (from c in dc.InvoiceDetails
                             where c.InvoiceID == id
                             select c).ToList();

                sum = Listdetails.Sum(InvoiceDetail => InvoiceDetail.Total);
            }
            return Listdetails;
        }

        

        public ActionResult Index()
        {
     
            return View("Invoice");
          
        }
               
       

        //-----------------------------------------------Item---------------------------------
        public JsonResult Getbynameitem(string ItemName)
        {
            string itemname = ItemName;
            Item invitem;
            List<Item> selectitem = new List<Item>();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                var myitems = (from c in dc.Items
                               where c.ItemName.Contains(itemname)
                               select c).ToList();

                foreach (var vitem in myitems)
                {
                    invitem = new Item();
                    invitem.ItemID = vitem.ItemID;
                    invitem.ItemName = vitem.ItemName;
                    invitem.ItemDescription = vitem.ItemDescription;
                    invitem.ItemPrice = vitem.ItemPrice;
                    invitem.ItemImage = vitem.ItemImage;
                    invitem.UnitID = vitem.UnitID;                                                                          
                    invitem.CategoryID = vitem.CategoryID;
                    selectitem.Add(invitem);
                }
            }
            
            var itemlist = selectitem;
            return Json(itemlist, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Getbyiditem(string itemid)
        {
            InvDetail invitem  = new InvDetail();
            List<Item> selectitem = new List<Item>();
            Item newitem = new Item();


            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                int id = Convert.ToInt32(itemid);
               
                var myitems = (from c in dc.Items
                               where c.ItemID == id
                               select c).ToList();

                foreach (var vitem in myitems)
                {
                    selectitem = new List<Item>();
                    newitem.ItemID = vitem.ItemID;
                    newitem.ItemName = vitem.ItemName;
                    newitem.ItemPrice = vitem.ItemPrice;
                    newitem.UnitID = vitem.UnitID;

                    selectitem.Add(newitem);
                }
            }
          
            return Json(selectitem, JsonRequestBehavior.AllowGet);
        }


        //---------------------------------------------------Service---------------------------
        public JsonResult Getbynameservice(string ServiceName)
        {
            string servicename = ServiceName;
            Service invservice;
            List<Service> selectservice = new List<Service>();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                var myserv = (from c in dc.Services
                              where c.ServiceName.Contains(servicename)
                              select c).ToList();

                foreach (var serv in myserv)
                {
                    invservice = new Service();
                    invservice.ServiceID = serv.ServiceID;
                    invservice.ServiceName = serv.ServiceName;
                    invservice.ServiceDescription = serv.ServiceDescription;
                    invservice.ServicePrice = serv.ServicePrice;
                    
                    selectservice.Add(invservice);
                }
            }
          
            var servicelist = selectservice;
            return Json(servicelist, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Getbyidservice(string serviceid)
        {
            InvDetail selectedservice = new InvDetail();
            List<Service> selectservice = new List<Service>();
            Service newservice = new Service();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                int id = Convert.ToInt32(serviceid);
               
                var myitems = (from c in dc.Services
                               where c.ServiceID == id
                               select c).ToList();

                foreach (var vitem in myitems)
                {
                    selectservice = new List<Service>();
                    newservice.ServiceID = vitem.ServiceID;
                    newservice.ServiceName = vitem.ServiceName;
                    newservice.ServicePrice = vitem.ServicePrice;
              
                    selectservice.Add(newservice);
                }
            }
        
            return Json(selectservice, JsonRequestBehavior.AllowGet);
        }

        //--------------------------------------------------------Vehicle-------------------------------------

        public JsonResult Getbynamevehicle(string VehicleNumber)
        {
            string vehinumber = VehicleNumber;
            Vehicle vehicle;        
            List<Vehicle> selectvehicle = new List<Vehicle>();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                var myvehicles = (from c in dc.Vehicles
                               where c.VehicleNumber.Contains(vehinumber)
                               select c).ToList();

                foreach (var vitem in myvehicles)
                {
                    vehicle = new Vehicle();
                    vehicle.VehicleID = vitem.VehicleID;
                    vehicle.VehicleNumber = vitem.VehicleNumber;
                    vehicle.VehicleType = vitem.VehicleType;
                    vehicle.UserID = vitem.UserID;

                    selectvehicle.Add(vehicle);
                }
            }

            var vehiclelist = selectvehicle;
            return Json(vehiclelist, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Getbyidvehicle(string VehicleID)
        {
            InvDetail selectedvehicle = new InvDetail();
            List<InvDetail> selectvehicle = new List<InvDetail>();
             Vehicle newvehicle = new Vehicle();
            User user = new User();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                int id = Convert.ToInt32(VehicleID);

                var myvehicles = (from c in dc.Vehicles
                                  where c.VehicleID == id
                                  select c).FirstOrDefault();

                int usrid = Convert.ToInt32(myvehicles.UserID);

                var myuser = (from c in dc.Users
                               where c.UserID == usrid
                               select c).FirstOrDefault();

                newvehicle.VehicleID = myvehicles.VehicleID;
                newvehicle.VehicleNumber = myvehicles.VehicleNumber;
                selectedvehicle.Myvehicle = newvehicle;

                user.FirstName = myuser.FirstName;
                user.UserID = myuser.UserID;
                selectedvehicle.Myuser = user;

                selectvehicle.Add(selectedvehicle);

            }
 
            return Json(selectvehicle, JsonRequestBehavior.AllowGet);
        }

        //----------------------------------------------------------------------------------------------------

   

        [HttpPost]
        public ActionResult Addinvoicelineitem(DateTime AppDate, string AppTime, string invid, string cid, string vehiid, string itemname, decimal itemprice, int itemunit, int quantity, decimal total, string type, int itemid, string pay)
        {
            ucontroller = new UnitController();
            Invoice invoice = new Invoice();
            InvoiceDetail invoicedetail = new InvoiceDetail();
            Service serv = new Service();
            InvDetail invdetail = new InvDetail();
            
            string button = "invoicedetail";
            bool Status = false;
            string message = "";
            
            int invdetid=0;

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                decimal Sum = 0;

                if (type == "i")
                {
                    if (invid == null)
                    {
                        invoice.Total = total;                                            
                        invoice.InvDate = AppDate;
                        invoice.InvTime = AppTime;                    
                        invoice.PayType = "cash";
                        invoice.UserID = Convert.ToInt32(cid);
                        invoice.VehicleID = Convert.ToInt32(vehiid);
                        invoice.Remarks = "invoice";
                        invdetail.Myinvoice = invoice;

                        dc.Invoices.Add(invdetail.Myinvoice);
                        dc.SaveChanges();

                        invdetid = invoice.InvoiceID;

                        invoicedetail.ItemServiceName = itemname;
                        invoicedetail.Price = itemprice;
                        invoicedetail.UnitID = itemunit;
                        invoicedetail.Qty = quantity;
                        invoicedetail.Total = total;
                        invoicedetail.InvoiceID = invdetid;
                        invoicedetail.ItemID = itemid;
                        invoicedetail.Remarks = "i";

                        invdetail.Myinvdetail = invoicedetail;

                        dc.InvoiceDetails.Add(invdetail.Myinvdetail);
                        dc.SaveChanges();

                        Sum = Sum + total;
                    }

                    else
                    {
                        invdetid = Convert.ToInt32(invid);

                        invoicedetail.ItemServiceName = itemname;
                        invoicedetail.Price = itemprice;
                        invoicedetail.UnitID = itemunit;
                        invoicedetail.Qty = quantity;
                        invoicedetail.Total = total;
                        invoicedetail.InvoiceID = invdetid;
                        invoicedetail.ItemID = itemid;
                        
                        invdetail.Myinvdetail = invoicedetail;

                        dc.InvoiceDetails.Add(invdetail.Myinvdetail);
                        dc.SaveChanges();

                        Sum = Sum + total;

                    }
                }

                if (type == "s")
                {
                    if (invid == null)
                    {
                        invoice.Total = total;
                        invoice.InvDate = AppDate;
                        invoice.InvTime = AppTime;
                        invoice.PayType = pay;
                        invoice.UserID = Convert.ToInt32(cid);
                        invoice.VehicleID = Convert.ToInt32(vehiid);
                        invoice.Remarks = "invoice";
                        invdetail.Myinvoice = invoice;

                        dc.Invoices.Add(invdetail.Myinvoice);
                        dc.SaveChanges();

                        invdetid = invoice.InvoiceID;

                        invoicedetail.ItemServiceName = itemname;
                        invoicedetail.Price = itemprice;
                        invoicedetail.UnitID = itemunit;
                        invoicedetail.Qty = quantity;
                        invoicedetail.Total = total;
                        invoicedetail.InvoiceID = invdetid;
                        invoicedetail.ServiceID = itemid;

                        invdetail.Myinvdetail = invoicedetail;

                        dc.InvoiceDetails.Add(invdetail.Myinvdetail);
                        dc.SaveChanges();

                        Sum = Sum + total;
                    }

                    else
                    {
                        invdetid = Convert.ToInt32(invid);

                        invoicedetail.ItemServiceName = itemname;
                        invoicedetail.Price = itemprice;
                        invoicedetail.UnitID = itemunit;
                        invoicedetail.Qty = quantity;
                        invoicedetail.Total = total;
                        invoicedetail.InvoiceID = invdetid;
                        invoicedetail.ServiceID = itemid;
                        invoicedetail.Remarks = "s";

                        invdetail.Myinvdetail = invoicedetail;

                        dc.InvoiceDetails.Add(invdetail.Myinvdetail);
                        dc.SaveChanges();

                        Sum = Sum + total;

                    }
                }
               
                decimal _totalsum;

                invdetail.Listdetail = Getallinvoices(invdetid,out _totalsum);
                invdetail.sum = _totalsum;
                return View("Invoice", invdetail);
            }
        }

     public ActionResult Deleteinvoicelineitem(string id)
        {
            InvDetail invdetail = new InvDetail();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                int invid = Convert.ToInt32(id);

                invdetail.Myinvdetail = (from c in dc.InvoiceDetails
                                where c.DetailID == invid
                                select c).FirstOrDefault();

                dc.InvoiceDetails.Remove(invdetail.Myinvdetail);
                dc.SaveChanges();      
            }
        
            decimal _totalsum;
            invdetail.Listdetail = Getallinvoices(invdetail.Myinvdetail.InvoiceID, out _totalsum);
            invdetail.sum = _totalsum;


            return View("Invoice", invdetail);
        }

        public ActionResult Updateinvoice(string invid)
        {
            InvDetail invdetail = new InvDetail();
            string message;
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                int invoiceid = Convert.ToInt32(invid);

                invdetail.Myinvoice = (from c in dc.Invoices
                               where c.InvoiceID == invoiceid
                               select c).FirstOrDefault();

                dc.Entry(invdetail.Myinvoice).State = EntityState.Modified;
                dc.SaveChanges();

                message = "Invoice Successfully Added";
            }

            ViewBag.Message = message;
            return View("Invoice");
        }

        public ActionResult CancelInvoice()
        {
            return View("Invoice");
        }



        public List<Invoice> Getinvoice()
        {
            List<Invoice> invoicelist = new List<Invoice>();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                Invoice invoice;

                var loadinvoices = dc.Invoices; 
                foreach (var ser in loadinvoices)
                {
                    invoice = new Invoice();
                    invoice.InvoiceID = ser.InvoiceID;
                    invoice.InvDate = ser.InvDate;
                    invoice.InvTime = ser.InvTime;
                    invoice.Total = ser.Total;
                    invoice.UserID = ser.UserID;
                    invoice.VehicleID = ser.VehicleID;
                    invoice.PayType = ser.PayType;
                    invoice.Remarks = ser.Remarks;

                    invoicelist.Add(invoice);
                }
            }
            return invoicelist;
        }
    }

}