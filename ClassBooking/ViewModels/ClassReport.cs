using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClassBooking.ViewModels
{
    public class ClassReport
    {
        [Required]
        [Display(Name = "Date From (dd/mm/yyyy)")]
        public string ClassDateFrom { get; set; }
        [Required]
        [Display(Name = "Date To (dd/mm/yyyy)")]
        public string ClassDateTo { get; set; }
        [Display(Name = "Total bookings")]
        public int TotalBookings { get; set; }
        [Display(Name = "Total on waiting list")]
        public int TotalWaitingList { get; set; }
        [Display(Name = "Total attended")]
        public int TotalAttended { get; set; }
        public IList<MemberAttendance> NoShowMembers { get; set; }
    }
}