using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GBA_Application.Models
{
    public partial class Statuses
    {
        public Statuses()
        {
            OrderStatuses = new HashSet<OrderStatuses>();
        }

        public int StatusId { get; set; }
        public string StatusDescription { get; set; }

        public virtual ICollection<OrderStatuses> OrderStatuses { get; set; }
    }
}
