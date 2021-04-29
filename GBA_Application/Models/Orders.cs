using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GBA_Application.Models
{
    public partial class Orders
    {
        public Orders()
        {
            OrderStatuses = new HashSet<OrderStatuses>();
        }

        public int OrderId { get; set; }
        public int AppointmentId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal OrderTotal { get; set; }
        public string OrderDescription { get; set; }
        public bool Completed { get; set; }

        public virtual Appointments Appointment { get; set; }
        public virtual ICollection<OrderStatuses> OrderStatuses { get; set; }
    }
}
