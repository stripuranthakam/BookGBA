using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GBA_Application.Models
{
    [ModelMetadataType(typeof(ServicesMetadata))]

    public partial class Services { }
    public class ServicesMetadata
    {
        public int ServiceId { get; set; }

        [StringLength(50, MinimumLength = 2)]
        [Required] 
        public string Name { get; set; }

        [StringLength(150, MinimumLength = 2)]
        [Required] 
        public string Description { get; set; }
        
        [StringLength(35, MinimumLength = 2)]
        [Required]
        [Display(Name = "Time to Complete")]
        public string TimeToComplete { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal Cost { get; set; }
    }
}
