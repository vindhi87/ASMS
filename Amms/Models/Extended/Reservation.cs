using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amms.Models
{

    public class Reservation
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string EmailID { get; set; }

        public User MyUser { get; set; }
        public List<User> UserList { get; set; }
        public Appointment Myappointment { get; set; }

        public AppointmentService MyappService { get; set; }

        public DateTime AppointmentDate { get; set; }

        public List<Service> ServList { get; set; }

        public Service Myservice { get; set; }

        //   public List<UserAppointment> AppointList { get; set; }
        public Vehicle Myvehicle { get; set; }
        public int SelectedVehicle { get; set; }
        public List<Vehicle> VehicleList { get; set; }
        public Dictionary<int, string> VehicleDropList { get; set; }

    }

}