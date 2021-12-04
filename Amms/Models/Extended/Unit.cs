using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Amms.Models
{
    [MetadataType(typeof(UnitMetadata))]
    public partial class Unit
    {

    }

    public class UnitMetadata
    {       
        public int UnitID { get; set; }
        
        [Display(Name = "Unit Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Unit Name required")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Invalid Character Found")]
        public string UnitName { get; set; }

        [Display(Name = "Unit Description")]
        public string UnitDescription { get; set; }        
    }
}