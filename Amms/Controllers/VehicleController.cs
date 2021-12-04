using Amms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amms.Controllers
{
    public class VehicleController : Controller
    {
        public ActionResult Vehicle()
        {
            return View();
        }

        //----------------return list of vehicles for required places-------------------
        public List<Vehicle> GetAllVehicle()
        {
            List<Vehicle> ListVehicle = new List<Vehicle>();
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                Vehicle vehicle;
              
                var loadvehicle = dc.Vehicles; // load all data from Service table 
                foreach (var v in loadvehicle)
                {
                    vehicle = new Vehicle();
                    vehicle.VehicleID = v.VehicleID;
                    vehicle.VehicleModel = v.VehicleModel;
                    vehicle.VehicleNumber = v.VehicleNumber;
                    vehicle.VehicleType = v.VehicleType;
                    vehicle.Year = v.Year;
                    vehicle.UserID = v.UserID;
                    ListVehicle.Add(vehicle);
                }
            }
            return ListVehicle;
        }

        public List<Vehicle> Getselectedvehicle()
        {
            List<Vehicle> ListVehicle = new List<Vehicle>();
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                Vehicle vehicle;

                var loadvehicle = dc.Vehicles; // load all data from Service table 
                foreach (var v in loadvehicle)
                {
                    vehicle = new Vehicle();
                    vehicle.VehicleID = v.VehicleID;
                    vehicle.VehicleModel = v.VehicleModel;
                    vehicle.VehicleNumber = v.VehicleNumber;
                    vehicle.VehicleType = v.VehicleType;
                    vehicle.Year = v.Year;
                    vehicle.UserID = v.UserID;
                    ListVehicle.Add(vehicle);
                }
            }
            return ListVehicle;
        }
    }
}