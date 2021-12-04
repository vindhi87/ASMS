using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amms.Models
{
    [MetadataType(typeof(ItemMetadata))]
    public partial class Item
    {
    }
    
    public class ItemMetadata
    {
        public int ItemID { get; set; }

        [Display(Name = "Item Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Item Name required")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Invalid Character Found")]
        public string ItemName { get; set; }

        [Display(Name = "Item Description")]
        public string ItemDescription { get; set; }
                     
        [Display(Name = "Item Price")]
        public decimal ItemPrice { get; set; }

        [Range(1, Int32.MaxValue, ErrorMessage = "Must Select A Value")]
        [Required(ErrorMessage = "Unit is required")]       
        public int UnitID { get; set; }

        [Display(Name = "Category")]
        public int CategoryID { get; set; }

        [Display(Name = "Item Image")]
        public string ItemImage { get; set; }
    }
}