using Amms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amms.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index()
        {
            return View();
        }
        

        //---------------retuns list of all categories for required places
        public List<Category> GetAllCategory()     
        {
            List<Category> ListCategory = new List<Category>();
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                Category cat;

                var loadcat = dc.Categories; // load all data from category table to the category list
                foreach (var i in loadcat)
                {
                    cat = new Category();
                    cat.CategoryID = i.CategoryID;
                    cat.CategoryName = i.CategoryName;
                    cat.CategoryDescription = i.CategoryDescription;

                    ListCategory.Add(cat);
                }
            }
            return ListCategory;
        }
    }
}