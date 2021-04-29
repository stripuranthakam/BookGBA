using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GBA_Application.Models
{
    public partial class Appointments
    {
        public Appointments()
        {
            Orders = new HashSet<Orders>();
        }

        public int AppointmentId { get; set; }
        public int? MemberId { get; set; }
        public int? ServiceId { get; set; }
        public int? VehicleId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string Description { get; set; }
        public bool Approved { get; set; }
        public string ApptInfo { get { return AppointmentDate.ToShortDateString() + ", " + AppointmentTime + " - " + Description; } }

        public virtual Members Member { get; set; }
        public virtual Services Service { get; set; }
        public virtual Vehicles Vehicle { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
