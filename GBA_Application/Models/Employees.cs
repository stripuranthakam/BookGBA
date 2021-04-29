using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GBA_Application.Models
{
    public partial class Employees
    {
        public Employees()
        {
            OrderStatuses = new HashSet<OrderStatuses>();
        }

        public int EmployeeId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<OrderStatuses> OrderStatuses { get; set; }
    }
}
