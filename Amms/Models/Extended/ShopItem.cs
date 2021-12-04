using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amms.Models
{
    public class ShopItem
    {
        public List<Item> ItemList { get; set; }

        public List<Category> CategoryList { get; set; } 

        public List<Unit> UnitList { get; set; }

        public Item Myitem { get; set; }

        public string CartTotal { get; set; }

        public double ShippingTotal { get; set; }

        public Invoice Myinvoice { get; set; }

        public InvoiceDetail Myinvdetail { get; set; }

    }
}