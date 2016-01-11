using ClassBooking.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ClassBooking.ViewModels {
    public class WeekGymClass {
        public IList<ClassDetail> WeekClasses { get; set; }
        public List<SelectListItem> FutureWeeks { get; set; }
        public int SelectedWeek { get; set; }
    }
}