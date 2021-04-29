using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GBA_Application.Models
{
    public partial class Members
    {
        public Members()
        {
            Appointments = new HashSet<Appointments>();
            Vehicles = new HashSet<Vehicles>();
        }

        public int MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string fullName { get { return FirstName + " " + LastName; } }


        //public virtual Vehicles Vehicles { get; set; }
        public virtual ICollection<Appointments> Appointments { get; set; }
        public virtual ICollection<Vehicles> Vehicles { get; set; }
    }
}
