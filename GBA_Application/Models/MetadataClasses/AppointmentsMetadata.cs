using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GBA_Application.Models
{
    [ModelMetadataType(typeof(AppointmentsMetadata))]

    public partial class Appointments { }
    public class AppointmentsMetadata
    {
        public int AppointmentId { get; set; }
        
        [Display(Name = "Member")]
        public int MemberId { get; set; }
        
        [Display(Name = "Service")]
        public int ServiceId { get; set; }
        
        [Display(Name = "Vehicle")]
        public int VehicleId { get; set; }
        
        [Required]
        [Display(Name = "Appointment Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime AppointmentDate { get; set; }
        
        [Display(Name = "Appointment Time")]
        [DataType(DataType.Time)]
        public TimeSpan AppointmentTime { get; set; }
        public string Description { get; set; }
        public bool Approved { get; set; }
    }
}
