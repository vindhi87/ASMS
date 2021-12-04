using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Amms.Models
{
    [MetadataType(typeof(ServiceMetadata))]
    public partial class Service
    {
         public bool IsActive { get; set; }
    }

    public class ServiceMetadata
    {
        public int ServiceID { get; set; }

        [Display(Name = "Service Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Service Name required")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Invalid Character Found")]
        public string ServiceName { get; set; }

        [Display(Name = "Service Description")]
        public string ServiceDescription { get; set; }

        [Display(Name = "Service Price")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Service Price required")]
        [DisplayFormat(DataFormatString = "{0:n}", ApplyFormatInEditMode = true)]
        public decimal ServicePrice { get; set; }

        [Display(Name = "Service Time")]
        public string ServiceTime { get; set; }

        [Display(Name = "Service Image")]
        public string ServiceImage { get; set; }
    }
}
