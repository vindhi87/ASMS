using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amms.Models
{
    public class Newuser
    {
        public int UserID {get;set;}
               
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string EmailID { get; set; }
        public string Password { get; set; }
        public bool IsEmailVerified { get; set; }
        public System.Guid ActivationCode { get; set; }
        public string UserGroup { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public string ConfirmPassword { get; set; }
        public List<User> UserList { get; set; }
        public User MyUser { get; set; }
        public List<Vehicle> VehicleList { get; set; }
        public Vehicle Myvehicle { get; set; } 
        
        public Profile Myprofile { get; set; }
        public Appointment Myappointment { get; set; }

        public List<Profile> Listhistory { get; set; }

        public List<Appointment> ListReservations { get; set; }

        public string vehiclenumber { get; set; }
    }
}