using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GBA_Application.Models
{
    public partial class Vehicles
    {
        public Vehicles()
        {
            Appointments = new HashSet<Appointments>();
        }

        public int VehicleId { get; set; }
        public string Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string LicensePlate { get; set; }
        public int MemberId { get; set; }

        public string fullVehicleName { get { return this.Make + " " + this.Model + " " + this.Year; } }

        public virtual Members Member { get; set; }
        public virtual ICollection<Appointments> Appointments { get; set; }
    }
}
