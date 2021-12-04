using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Amms.Models;
using Amms.Models.Extended;

namespace Amms.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            return View("Getreport");
        }

        //-------------Get Customer List-------------------
        public ActionResult customerlist()
        {
            MyDatabaseEntities dc = new MyDatabaseEntities();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "Customer_List.rpt"));
            rd.SetDataSource(dc.Users.Select(c => new
            {
                UserID = c.UserID,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Address = c.Address,
                Mobile = c.Mobile,
                EmailID = c.EmailID,
            }).ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            rd.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
            rd.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
            rd.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/pdf", "Customer_List.pdf");
        }


        //-----------------Summary Chart of Appointments----------------
        public ActionResult appointments()
        {
            MyDatabaseEntities dc = new MyDatabaseEntities();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "CrystalReport3.rpt"));
            rd.SetDataSource(dc.Appointments.Select(c => new
            {
                AppointmentID = c.AppointmentID,
                AppointmentStatus = c.AppointmentStatus,

            }).ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            rd.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
            rd.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
            rd.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/pdf", "Appointmnts_Summary.pdf");
        }



        //public ActionResult Download_PDF()
        //{
        //    MyDatabaseEntities dc = new MyDatabaseEntities();

        //    ReportDocument rd = new ReportDocument();
        //    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "InvoiceDet.rpt"));
        //    List<CtristalReport5> crp5 = new List<CtristalReport5>();

        //    rd.SetDataSource(dc.invoice_details.Select(c => new
        //    { 
        //            InvoiceID = c.InvoiceID,
        //            InvTime = c.InvTime,
        //            InvDate  =c.InvDate,
        //            VehicleNumber = c.VehicleNumber,

        //    }).ToList());

        //    Response.Buffer = false;
        //    Response.ClearContent();
        //    Response.ClearHeaders();

        //    //   rd.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
        //    //  rd.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
        //    // rd.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;

        //    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //    stream.Seek(0, SeekOrigin.Begin);

        //    return File(stream, "application/pdf", "CustomerList.pdf");
        //}


        //-----------------Get Invioce Details---------------
        public ActionResult invoicedetails()
        {
            MyDatabaseEntities dc = new MyDatabaseEntities();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "CrystalReport7.rpt"));
            rd.SetDataSource(dc.invoice_details.Select(c => new
            {
                InvoiceID = c.InvoiceID,
                InvTime = c.InvTime,
                InvDate = c.InvDate,
                VehicleNumber = c.VehicleNumber,

            }).ToList());


            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            //rd.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
            //rd.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
            //rd.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/pdf", "Invoice_Details.pdf");
        }

        //-----------Get Service List------------
        public ActionResult servicelist()
        {
            MyDatabaseEntities dc = new MyDatabaseEntities();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "Service_List.rpt"));
            rd.SetDataSource(dc.Services.Select(c => new
            {
                ServiceID = c.ServiceID,
                ServiceName = c.ServiceName,
                ServiceDescription = c.ServiceDescription,
                ServiceTime = c.ServiceTime,

            }).ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            rd.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
            rd.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
            rd.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/pdf", "Service_List.pdf");
        }


        public ActionResult monthlysales()
        {
            MyDatabaseEntities dc = new MyDatabaseEntities();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "Monthly_Sales.rpt"));
            rd.SetDataSource(dc.Invoices.Select(c => new
            {
                InvoiceID = c.InvoiceID,
                InvDate = c.InvDate,
                Total = c.Total,

            }).ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            rd.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
            rd.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
            rd.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/pdf", "Monthly_Sales.pdf");
        }



        public ActionResult Download_PDF()
        {
            MyDatabaseEntities dc = new MyDatabaseEntities();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "InvoiceDet.rpt"));
            List<CtristalReport5> crp5 = new List<CtristalReport5>();

            rd.SetDataSource(dc.InvoiceDetails.Select(c => new
            {
                Price = c.Price,
                Total = c.Total,


            }).ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            //   rd.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
            //  rd.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
            // rd.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/pdf", "InvoiceDet.pdf");
        }


        public ActionResult PrintInvoice(int id)
        {
            //MyDatabaseEntities dc = new MyDatabaseEntities();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "invoice.rpt"));
            List<CtristalReport5> crp5 = new List<CtristalReport5>();

            List<Invoice> invoice = new List<Invoice>();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                Invoice history = new Invoice();

                var details = (from a in dc.Invoices
                               
                               // join d in dc.Vehicles on a.VehicleID equals d.VehicleID

                               where a.InvoiceID == id
                               select new
                               {
                                   a.InvoiceID,                                   
                                   //   d.VehicleNumber,
                               }).ToList();

                if (details.Count < 1)
                {
                    history = new Invoice();
                    history.InvoiceID = 0;
                    history.FirstName = "N/A";
                    history.VehicleNumber = "N/A";
                    history.InvDate = DateTime.Now;
                    

                    invoice.Add(history);
                }
                foreach (var ser in details)
                {
                    history = new Invoice();
                    history.InvoiceID = ser.InvoiceID;
                    history.FirstName = "Customer";
                    history.VehicleNumber = "N/A";
                    history.InvDate = DateTime.Now;
                    //  history.VehicleNumber = ser.VehicleNumber;


                    invoice.Add(history);
                }
            }

            rd.SetDataSource(invoice);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            //   rd.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
            //  rd.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
            // rd.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/pdf", "InvoiceDet.pdf");
        }
    }

}