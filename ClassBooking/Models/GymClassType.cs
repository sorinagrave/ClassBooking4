using Newtonsoft.Json;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
namespace ClassBooking.Models {
    [Table("GymClassType")]
    [JsonObject]
    public class GymClassType {
        public int GymClassTypeId { get; set; }
        [Display(Name = "Class Type")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "Class Type Description")]
        public string Description { get; set; }

        [Display(Name = "Maximum capacity")]
        [Required]
        public int MaxCapacity { get; set; }

        [Display(Name = "Maximum wait list")]
        [Required]
        public int MaxWaitList { get; set; }

        [Display(Name = "Day of the week")]
        public DayOfWeek DayOfTheWeek { get; set; }

        [Display(Name = "Class time")]
        [RegularExpression("^[0-2][0-9]:[0-5][0-9]$", ErrorMessage = "Please enter the time as hh:mm")]
        public string ClassTime { get; set; }

        public static SelectList GetWeekDays(DayOfWeek defaultDay) {
            var values = from DayOfWeek e in Enum.GetValues(typeof(DayOfWeek))

                         select new { id = e, Name = e.ToString() };
            return new SelectList(values, "Id", "Name", defaultDay);
        }
    }
}