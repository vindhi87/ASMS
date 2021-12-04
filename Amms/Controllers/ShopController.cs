using Amms.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amms.Controllers
{
    public class ShopController : Controller
    {
      
        int x = 0;
        // GET: Shop
        public ActionResult ShopProd()
        {
            ShopItem shopItem = new ShopItem();
            List<Category> cat = GetAllCategory();
            shopItem.CategoryList = cat;

            List<Item> items = GetAllItem();
            shopItem.ItemList = items;

            return View(shopItem);
        }

        public ActionResult ShopCart()
        {
            return View();
        }

        //------------------return list of categories-----------------
        public List<Category> GetAllCategory()
        {
            List<Category> ListCat = new List<Category>();
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                Category shopcat;                
                var cats = dc.Categories;// load all data from category table 
                foreach (var vcat in cats)
                {
                    shopcat = new Category();
                    shopcat.CategoryID = vcat.CategoryID;
                    shopcat.CategoryName = vcat.CategoryName;
                    shopcat.CategoryDescription = vcat.CategoryDescription;

                    ListCat.Add(shopcat);
                }
            }
            return ListCat;
        }

        //-----------------return list of items------------------
        public List<Item> GetAllItem()
        {
            List<Item> ListItem = new List<Item>();
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                Item shopitem;
                
                var items = dc.Items;
                foreach (var vitem in items)
                {
                    shopitem = new Item();
                    shopitem.ItemID = vitem.ItemID;
                    shopitem.ItemName = vitem.ItemName;
                    shopitem.ItemDescription = vitem.ItemDescription;
                    shopitem.ItemPrice = vitem.ItemPrice;
                    shopitem.ItemImage = vitem.ItemImage;
                    shopitem.UnitID = vitem.UnitID;
                    shopitem.CategoryID = vitem.CategoryID;
                    ListItem.Add(shopitem);
                }
            }
            return ListItem;
        }


        [HttpGet]
        public List<Item> GetSelectItem(int catid = 0)
        {
            List<Item> ListItem = new List<Item>();
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                Item shopitem;
              
                var items = dc.Items.ToList();          
                if (catid != 0)
                {
                    items = items.Where(a => a.CategoryID == catid).ToList();
                }

                foreach (var vitem in items)
                {
                    shopitem = new Item();
                    shopitem.ItemID = vitem.ItemID;
                    shopitem.ItemName = vitem.ItemName;
                    shopitem.ItemDescription = vitem.ItemDescription;
                    shopitem.ItemPrice = vitem.ItemPrice;
                    shopitem.ItemImage = vitem.ItemImage.ToString();

                    ListItem.Add(shopitem);
                }
            }
            return ListItem;
        }

        //-------------------filter items according to categories----------------
        public ActionResult ShoptoCategory(int catid)
        {
            ShopItem shopItem = new ShopItem();
            List<Category> cat = GetAllCategory();
            shopItem.CategoryList = cat;

            List<Item> items1 = GetSelectItem(catid);
            shopItem.ItemList = items1;

            return View("ShopProd", shopItem);
        }


        //--------------------Add items to the cart with increament--------------------

        List<Item> li = new List<Item>();
        public ActionResult AddToCart(string id)
        {
            bool itemexist = false;
            Item it = new Item();
            MyDatabaseEntities dc = new MyDatabaseEntities();
            ShopItem sitem = new ShopItem();

            int itemid = Convert.ToInt32(id);

            it = (from c in dc.Items
                  where c.ItemID == itemid
                  select c).FirstOrDefault();

            if (Session["cart"] == null)
            {
                li.Add(it);
                li[0].qty = 1;
                li[0].Total = Convert.ToDecimal(li[0].ItemPrice);
                Session["cart"] = li;
                ViewBag.cart = li.Count();

                Session["count"] = 1;
            }
            else
            {
                li = (List<Item>)Session["cart"];

                for (var i = 0; i < li.Count(); i++)
                {
                    if (li[i].ItemID == itemid)
                    {
                        li[i].qty = li[i].qty + 1;
                        li[i].Total = Convert.ToDecimal(li[i].qty * li[i].ItemPrice);
                        itemexist = true;
                    }

                }
                    if (!itemexist)
                    {
                        it.qty = 1;
                        it.Total = it.ItemPrice;
                        li.Add(it);
                        Session["count"] = Convert.ToInt32(Session["count"]) + 1;
                    }
                
                Session["cart"] = li;
                Session["carttot"] = sitem.CartTotal;
                ViewBag.cart = li.Count();
            }

            return RedirectToAction("ShopProd", "Shop");

        }


        //----------------------Pass values to DB when proceed to payment clicked------------------
        [HttpPost]
        public ActionResult Addtodb(string total)
        {

            Invoice invoice = new Invoice();
            InvoiceDetail invoicedetail = new InvoiceDetail();

            ShopItem shopcart = new ShopItem();

            List<Item> lidb = new List<Item>();
            int invdetid;

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {

                invoice.Total = Convert.ToDecimal(total);
                invoice.InvDate = DateTime.UtcNow;
                invoice.InvTime = "11:30:00";
                invoice.PayType = "card";
                invoice.UserID = Convert.ToInt32(Session["UserID"]);
                invoice.VehicleID = 1;
                invoice.Remarks = "shopping cart";

                shopcart.Myinvoice = invoice;

                dc.Invoices.Add(shopcart.Myinvoice);
                dc.SaveChanges();

                invdetid = invoice.InvoiceID;
                lidb = (List<Item>)Session["cart"];

                foreach (var i in lidb)
                {
                    invoicedetail.ItemServiceName = i.ItemName;
                    invoicedetail.Price = Convert.ToDecimal(i.ItemPrice);
                    invoicedetail.UnitID = Convert.ToInt32(i.UnitID);
                    invoicedetail.Qty = i.qty;
                    invoicedetail.Total = i.Total;
                    invoicedetail.InvoiceID = invdetid;
                    invoicedetail.ItemID = i.ItemID;

                    shopcart.Myinvdetail = invoicedetail;

                    dc.InvoiceDetails.Add(shopcart.Myinvdetail);
                    dc.SaveChanges();                    
                }
            }
            return RedirectToAction("ShopProd", "Shop");
        }


        //---------------------Load the shopping cart after add to cart button clicked--------------
        public ActionResult LoadCart()
        {
            string message;
            ShopItem it = new ShopItem();
            try
            {
                it.ItemList = (List<Item>)Session["cart"];
                it.CartTotal = it.ItemList.Sum(i => i.ItemPrice).ToString();               
            }
            catch (Exception e)
            {
                message = "Cart is empty";
            }

            return View("ShopCart", it);
        }
 

    }
}
