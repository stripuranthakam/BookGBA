using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GBA_Application.Models
{
    public partial class Services
    {
        public Services()
        {
            Appointments = new HashSet<Appointments>();
        }

        public int ServiceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TimeToComplete { get; set; }
        public decimal? Cost { get; set; }

        public virtual ICollection<Appointments> Appointments { get; set; }
    }
}
