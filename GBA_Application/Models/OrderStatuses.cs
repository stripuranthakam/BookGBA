using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GBA_Application.Models
{
    public partial class OrderStatuses
    {
        public int OrderStatusesId { get; set; }
        public int? OrderId { get; set; }
        public int? StatusId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime EstimatedFinishDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }

        public virtual Employees Employee { get; set; }
        public virtual Orders Order { get; set; }
        public virtual Statuses Status { get; set; }
    }
}
