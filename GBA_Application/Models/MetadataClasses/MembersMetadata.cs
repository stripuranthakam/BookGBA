using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GBA_Application.Models
{
    [ModelMetadataType(typeof(MembersMetadata))]

    public partial class Members { }

    public class MembersMetadata
    {
        public int MemberId { get; set; }

        [Display(Name = "First Name")]
        [StringLength(50)]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(50)]
        [Required]
        public string LastName { get; set; }

        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
        public string Phone { get; set; }

        [StringLength(20, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        //[Required]
        public DateTime? DateOfBirth { get; set; }

        //[Required]
        [StringLength(70, MinimumLength = 5)]
        public string Address { get; set; }

        //[Required]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        public string Province { get; set; }

        //[Required]
        [StringLength(35, MinimumLength = 2)]
        public string City { get; set; }
        public string Country { get; set; }
    }
}
