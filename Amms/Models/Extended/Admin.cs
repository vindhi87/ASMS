using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Amms.Models
{
    public class Admin
    {            
        public List<Unit> UnitList { get; set; }
        public Unit Myunit { get; set; }       

        public Dictionary<int, string> UnitDropList { get; set; }

        public List<Category> CategoryList { get; set; }

        public Category Mycategory { get; set; }

        public Dictionary<int, string> CategoryDropList { get; set; }

        public List<Service> ServiceList { get; set; }

        public Service Myservice { get; set; }

        public List<Item> ItemList { get; set; }

        public Item Myitem { get; set; }
       
        public List<User> UserList { get; set; }

        public User Myuser { get; set; }

        public Vehicle Myvehicle { get; set; }

        public List<Vehicle> VehicleList { get; set; }

        public string file { get; set; }

        public List<Appointment> ListAppoinments { get; set; }
        
        public Appointment Myappointment { get; set; }
        public AppointmentService Myappservice { get; set; }

        public List<Profile> Listhistory { get; set; }

        public List<Invoice> Listinvoices { get; set; }
    }
}