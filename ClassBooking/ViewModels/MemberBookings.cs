using ClassBooking.Models;
using System.Collections.Generic;

namespace ClassBooking.ViewModels {
    public class MemberBookings {
        public IList<ClassDetail> GymClasses { get; set; }
        public GymMember CurrentMember { get; set; }
        public IList<GymMember> AllMembers { get; set; }
    }
}