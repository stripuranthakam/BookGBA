using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GBA_Application.Models
{
    [ModelMetadataType(typeof(VehiclesMetadata))]

    public partial class Vehicles { }
    public class VehiclesMetadata
    {
        public int VehicleId { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Year must be exactly 4 digits")]
        public string Year { get; set; }
        
        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string Make { get; set; }
        
        [StringLength(60, MinimumLength = 2)]
        [Required]
        public string Model { get; set; }
        
        [StringLength(8, MinimumLength = 8)]
        [Display(Name = "License Plate")]
        public string LicensePlate { get; set; }
        public int MemberId { get; set; }
    }
}
