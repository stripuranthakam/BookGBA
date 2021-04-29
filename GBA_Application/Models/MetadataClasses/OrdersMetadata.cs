using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GBA_Application.Models
{
    [ModelMetadataType(typeof(OrdersMetadata))]

    public partial class Orders { }
    public class OrdersMetadata
    {
        public int OrderId { get; set; }
        
        [Display(Name = "Appointment")]
        public int AppointmentId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }
        
        [Display(Name = "Order Total")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal OrderTotal { get; set; }
        
        [Display(Name = "Order Description")]
        public string OrderDescription { get; set; }
        public bool Completed { get; set; }
    }
}
