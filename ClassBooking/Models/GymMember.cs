using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassBooking.Models {
    [Table("GymMember")]
    public class GymMember {
        public int GymMemberId { get; set; }
        [Display(Name = "Staff Id")]
        [Required]
        public string StaffId { get; set; }
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Email address")]
        public string EmailAddress { get; set; }
        [Display(Name = "Is Administrator?")]
        public bool IsAdmin { get; set; }
        [NotMapped]
        public string FullName { get; set; }
        public virtual IList<GymClassBooking> Bookings { get; set; }
    }
}