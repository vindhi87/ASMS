using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Amms.Models
{
    public class Profile
    {
       public int InvoiceID { get; set; }
        public DateTime InvDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string VehicleNumber { get; set; }
        public string ServiceName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime AppointmentStartTime { get; set; }
        public int AppointmentID { get; set; }

    }
}