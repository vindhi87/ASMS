using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amms.Models
{
    public class InvDetail
    {
        //public int ServiceID { get; set; }
        //public string ServiceName { get; set; }
        //public string ServiceDescription { get; set; }
        //public Nullable<decimal> ServicePrice { get; set; }
        //public string ServiceTime { get; set; }
        //public string ServiceImage { get; set; }

        public Vehicle Myvehicle { get; set; }
        public User Myuser { get; set; }

        public List<Service> ServiceList { get; set; }

        public Service Myservice { get; set; }

        public Dictionary<int, string> ServiceDropList { get; set; }

        public List<Item> ItemList { get; set; }

        public Item Myitem { get; set; }

        public Dictionary<int, string> ItemDropList { get; set; }

        public List<SelectListItem> selectservice { get; set; }

        public Invoice Myinvoice { get; set; }

        public InvoiceDetail Myinvdetail { get; set; }
        public List<InvoiceDetail> Listdetail { get; set; }
        public List<Invoice> Listinvoice { get; set; }

        public List<Unit> UnitLIst { get; set; }

        public string UnitName { get; set; }

         public decimal sum { get; set; }

    }
}