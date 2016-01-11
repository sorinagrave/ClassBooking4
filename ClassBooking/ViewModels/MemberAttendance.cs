using ClassBooking.Models;

namespace ClassBooking.ViewModels {
    public class MemberAttendance {
        public GymMember Member { get; set; }
        public int NoShows { get; set; }
    }
}