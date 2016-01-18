using ClassBooking.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ClassBooking.ViewModels {
    public class ClassBookingDetail {
        public int GymClassId { get; set; }
        [Required]
        [Display(Name = "Class Type")]
        public int GymClassTypeId { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Type")]
        public string GymClassTypeName { get; set; }
        [Required]
        [Display(Name = "Class date time")]
        public DateTime ClassDateTime { get; set; }
        [Required]
        [Display(Name = "Maximum Capacity")]
        public int MaxCapacity { get; set; }
        [Required]
        [Display(Name = "Maximum Wait List")]
        public int MaxWaitList { get; set; }
        [Required]
        [Display(Name = "Class Time")]
        [RegularExpression("^[0-2][0-9]:[0-5][0-9]$", ErrorMessage = "Please enter the time as hh:mm")]
        public string ClassTime { get; set; }
        [Required]
        [Display(Name = "Class Date")]
        [RegularExpression("^[0-3][0-9]/[0-1][0-9]/\\d{4}$", ErrorMessage = "Please enter the date as dd/mm/yyyy")]
        public string ClassDate { get; set; }
        public IList<GymClassType> Types { get; set; }

        public IList<GymClassBooking> ClassBookings { get; set; }
        public IList<GymClassBooking> ClassWaiting { get; set; }
        [Display(Name = "Booked")]
        public int nBookings { get; set; }

        public bool IsEditable { get; set; }
        public MemberClassStatus MemberStatus { get; set; }
    }
    public static class ClassBookingDetailMapper {
        public static GymClass ToModel(this ClassBookingDetail classDetail) {
            GymClass gymClass = new GymClass();
            gymClass.Description = classDetail.Description;
            gymClass.GymClassId = classDetail.GymClassId;
            gymClass.GymClassTypeId = classDetail.GymClassTypeId;
            gymClass.MaxCapacity = classDetail.MaxCapacity;
            gymClass.MaxWaitList = classDetail.MaxWaitList;
            if(classDetail.ClassDateTime != null && classDetail.ClassDateTime > DateTime.Now) {
                gymClass.ClassDateTime = classDetail.ClassDateTime;
            }
            else if (!String.IsNullOrEmpty(classDetail.ClassDate) && !String.IsNullOrEmpty(classDetail.ClassTime)) {
                DateTime d;

                bool dateOk = DateTime.TryParseExact(classDetail.ClassDate + " " + classDetail.ClassTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
                if (dateOk) {
                    gymClass.ClassDateTime = d;
                }else{
                    gymClass.ClassDateTime = DateTime.Now;
                }
            }
            return gymClass;
        }
    }
}