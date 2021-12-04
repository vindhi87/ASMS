using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Amms.Models
{
    [MetadataType(typeof(VehicleMetadata))]
    public partial class Vehicle
    {

    }
    public class VehicleMetadata
    {
        public int VehicleID { get; set; }

        [Display(Name = "Vehicle Type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Vehicle Type required")]
        public string VehicleType { get; set; }


        [Display(Name = "Vehicle Model")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Vehicle Model required")]
        public string VehicleModel { get; set; }

        [Display(Name = "Vehicle Number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Vehicle Number required")]
        public string VehicleNumber { get; set; }

        [Display(Name = "Year")]
        [RegularExpression(@"^([0-9])*$", ErrorMessage = "Invalid characters found")]
      //  [MaxLength(4, ErrorMessage = "Invalid Year")]
        public int Year { get; set; }
    }
}
