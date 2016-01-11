using ClassBooking.Database;
using ClassBooking.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;

namespace ClassBooking.Models {
    [Table("GymClass")]
    public class GymClass {
        public int GymClassId { get; set; }
        [Required]
        [Display(Name = "Class Type")]
        public int GymClassTypeId { get; set; }
        [Display(Name = "Class Type")]
        public virtual GymClassType Type { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Class date time")]
        public DateTime ClassDateTime { get; set; }
        [Required]
        [Display(Name = "Maximum Capacity")]
        public int MaxCapacity { get; set; }
        [Required]
        [Display(Name = "Maximum Wait List")]
        public int MaxWaitList { get; set; }
        public virtual IList<GymClassBooking> Bookings { get; set; }
    }
    public static class GymClassMapper {
        public static void CopyGymClass(GymClass source, GymClass dest) {
            dest.Description = source.Description;
            dest.GymClassTypeId = source.GymClassTypeId;
            dest.MaxCapacity = source.MaxCapacity;
            dest.MaxWaitList = source.MaxWaitList;
            dest.ClassDateTime = source.ClassDateTime;
        }
        public static ClassDetail ToViewModel(this GymClass gymClass, bool isDisplayDateFormat) {
            ClassDetail cd = new ClassDetail();
            if (gymClass == null) {
                return cd;
            }
            cd.Description = gymClass.Description;
            cd.GymClassId = gymClass.GymClassId;
            cd.MaxCapacity = gymClass.MaxCapacity;
            cd.MaxWaitList = gymClass.MaxWaitList;
            cd.GymClassTypeId = gymClass.GymClassTypeId;
            cd.GymClassTypeName = gymClass.Type.Name;
            cd.ClassDateTime = gymClass.ClassDateTime;

            if (isDisplayDateFormat) {
                cd.ClassDate = gymClass.ClassDateTime.ToString("dddd dd MMM");
                cd.ClassTime = gymClass.ClassDateTime.ToString("HH:mm");
            } else {
                cd.ClassTime = gymClass.ClassDateTime.ToString("HH:mm");
                cd.ClassDate = gymClass.ClassDateTime.ToString("dd/MM/yyyy");
            }
            cd.IsEditable = gymClass.ClassDateTime < DateTime.Now;
            cd.Bookings = new List<GymClassBooking>();
            foreach (var b in gymClass.Bookings.OrderBy(bk => bk.GymMember.LastName).ToList()) {
                cd.Bookings.Add(b);
            }
            cd.nBookings = cd.Bookings.Count();
            return cd;
        }
    }
}