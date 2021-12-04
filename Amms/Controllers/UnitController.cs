using Amms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amms.Controllers
{
    public class UnitController : Controller
    {
        // GET: Unit
        public ActionResult Index()
        {
            return View();
        }


        //----------------return list of units for required places-------------------
        public List<Unit> GetAllUnit()
        {
            List<Unit> ListUnit = new List<Unit>();

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                Unit unit1;
                
                var loadunit = dc.Units; // load all data from unit table to the unit list
                foreach (var i in loadunit)
                {
                    unit1 = new Unit();
                    unit1.UnitID = i.UnitID;
                    unit1.UnitName = i.UnitName;
                    unit1.UnitDescription = i.UnitDescription;

                    ListUnit.Add(unit1);                    
                }
            }
            return ListUnit;
        }
    }
}