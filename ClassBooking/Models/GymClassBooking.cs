
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassBooking.Models
{
    [Table("GymClassBooking")]
    public class GymClassBooking
    {
        public int GymClassBookingId { get; set; }
        public int GymClassId { get; set; }
        public int GymMemberId { get; set; }
        public virtual GymClass GymClass { get; set; }
        public virtual GymMember GymMember { get; set; }
        public bool Attended { get; set; }
        public bool Waiting { get; set; }
    }
}