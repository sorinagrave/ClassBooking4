using ClassBooking.Attributes;
using ClassBooking.Authorisation;
using ClassBooking.Database;
using ClassBooking.Models;
using ClassBooking.ViewModels;
using LBG.Insurance.Developer.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ClassBooking.Controllers {

    public class HomeController : Controller {
        private GymContext db = new GymContext();

        // GET: GymClassTypes
        public ActionResult Index(int memberId = 0) {
            Tex.VerboseLog("Home/Index", memberId.ToString());
            try {
                MemberBookings gmb = new MemberBookings();
                DateTime oneHourFromNow = DateTime.Now.AddHours(1);
                var allFutureClasses = db.GymClass.Where(cl => cl.ClassDateTime > oneHourFromNow).OrderBy(bk => bk.ClassDateTime).ToList();
                GymMember gm;
                if (memberId == 0) {
                    gm = GetCurrentGymMember();
                } else {
                    gm = db.GymMembers.Find(memberId);
                }
                var allBookableClasses = new List<ClassDetail>();
                foreach (GymClass cl in allFutureClasses) {

                    var bookableClass = cl.ToViewModel(true);
                    if (gm != null) {
                        var memberBooking = db.MemberClassBookings.Where(bk => bk.GymClassId == cl.GymClassId && bk.GymMemberId == gm.GymMemberId).FirstOrDefault();
                        if (memberBooking != null) {
                            bookableClass.MemberStatus = memberBooking.Waiting ? MemberClassStatus.BookedWaiting : MemberClassStatus.BookedClass;
                        } else {
                            int nNonWaitBookings = db.MemberClassBookings.Where(bk => bk.GymClassId == cl.GymClassId && !bk.Waiting).Count();
                            if (nNonWaitBookings < cl.MaxCapacity) {
                                bookableClass.MemberStatus = MemberClassStatus.EligibleClass;
                            } else {
                                bookableClass.MemberStatus = (bookableClass.nBookings < cl.MaxCapacity + cl.MaxWaitList ? MemberClassStatus.EligibleWaiting : MemberClassStatus.None);
                            }
                        }
                    }
                    allBookableClasses.Add(bookableClass);
                }

                gmb.GymClasses = allBookableClasses;
                gmb.CurrentMember = gm;
                gmb.AllMembers = db.GymMembers.OrderBy(m=>m.LastName).ToList();
                foreach (var memb in gmb.AllMembers) {
                    memb.FullName = String.Format("{0} {1}", memb.FirstName, memb.LastName);
                }
                return View(gmb);
            } catch (Exception ex) {
                Tex.Dump(ex, "Home Index Excetpion");
                throw;  
            }
        }

        [ClassBookingAuthorise(Level = AuthorisationLevel.Member)]
        public ActionResult Book(int classId) {
            GymMember gm = GetCurrentGymMember();
            if (gm == null) {
                ViewBag.Error = "Member not found";
                return View("BookingError");
            }

            return BookClassMember(classId, gm.GymMemberId);
        }

        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult BookMember(int classId, int memberId) {
            return BookClassMember(classId, memberId);
        }

        private ActionResult BookClassMember(int classId, int memberId) {
            GymClass cl = db.GymClass.Find(classId);
            if (cl == null) {
                ViewBag.Error = "Class not found";
                return View("BookingError");
            }
            int nMemberClassBookings = db.MemberClassBookings.Where(cb => cb.GymClassId == classId && cb.GymMemberId == memberId).Count();
            if(nMemberClassBookings > 0) {
                ViewBag.Error = "You are already booked on this class!";
                return View("BookingError");
            }
            int nAllBooked = db.MemberClassBookings.Where(bk => bk.GymClassId == cl.GymClassId).Count();
            int nBooked = db.MemberClassBookings.Where(bk => bk.GymClassId == cl.GymClassId && !bk.Waiting).Count();
            if (nAllBooked < cl.MaxCapacity + cl.MaxWaitList) {
                GymClassBooking booking = new GymClassBooking();
                booking.GymClassId = classId;
                booking.GymMemberId = memberId;
                booking.Waiting = nBooked >= cl.MaxCapacity;
                db.Entry(booking).State = EntityState.Added;
                db.SaveChanges();
            } else {
                ViewBag.Error = "Class is fully booked!";
                return View("BookingError");
            }
            return RedirectToAction("Index", new { memberId = memberId });
        }

        [ClassBookingAuthorise(Level = AuthorisationLevel.Member)]
        public ActionResult Cancel(int classId) {
            GymMember gm = GetCurrentGymMember();
            if (gm == null) {
                ViewBag.Error = "Member not found";
                return View("BookingError");
            }
            if (!CancelMemberBooking(classId, gm.GymMemberId, db)) {
                ViewBag.Error = "Booking could not be found!";
                return View("BookingError");
            }
            return RedirectToAction("Index");
        }

        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult CancelMember(int classId, int memberId) {
            if (!CancelMemberBooking(classId, memberId, db)) {
                ViewBag.Error = "Booking could not be cancelled!";
                return View("BookingError");
            }
            return RedirectToAction("Index", new { memberId = memberId });
        }

        public static bool CancelMemberBooking(int classId, int memberId, GymContext db) {
            GymClass cl = db.GymClass.Find(classId);
            if (cl == null) {
                return false;
            }
            var booking = db.MemberClassBookings.Where(bk => bk.GymClassId == cl.GymClassId && bk.GymMemberId == memberId).FirstOrDefault();
            if (booking == null) {
                return false;
            }
            db.Entry(booking).State = EntityState.Deleted;
            var nBookings = db.MemberClassBookings.Where(cls => cls.GymClassId == classId && !cls.Waiting).Count();
            // Promote the next one from the waiting list
            GymClassBooking promotedBooking = null;
            if (!booking.Waiting && nBookings <= cl.MaxCapacity){
                promotedBooking = db.MemberClassBookings.Where(bk => bk.GymClassId == cl.GymClassId && bk.Waiting == true).FirstOrDefault();
            }       
            if (promotedBooking != null) {
                promotedBooking.Waiting = false;
                db.Entry(promotedBooking).State = EntityState.Modified;
            }
            db.SaveChanges();
            return true;
        }

        private GymMember GetCurrentGymMember() {
            var nameParts = User.Identity.Name.Split('\\');
            if (nameParts.Length < 1) return null;
            string sName = nameParts[1];
            return db.GymMembers.SingleOrDefault(m => m.StaffId == sName);
        }
    }
}