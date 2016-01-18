using ClassBooking.Attributes;
using ClassBooking.Authorisation;
using ClassBooking.Database;
using ClassBooking.Models;
using ClassBooking.ViewModels;
using LBG.Insurance.Developer.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ClassBooking.Controllers {

    public class GymClassController : Controller {
        private GymContext db = new GymContext();

        // GET: GymClass
        public ActionResult Index() {
            var classes = db.GymClass.OrderBy(bk => bk.ClassDateTime).ToList().Select(v => v.ToViewModel(true));
            return View(classes);
        }

        public ActionResult CreateWeekClasses(int nWeek = 0) {
            WeekGymClass wgc = new WeekGymClass();
            wgc.SelectedWeek = nWeek;
            wgc.FutureWeeks = new List<SelectListItem>(4);
            DateTime now = DateTime.Now.Date;
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7;
            for(int i = 0; i < 4; i++) {
                SelectListItem weekItem = new SelectListItem();
                weekItem.Value = i.ToString();
                weekItem.Text = now.AddDays(daysUntilMonday + i * 7).ToShortDateString();
                weekItem.Selected = (nWeek == i);
                wgc.FutureWeeks.Add(weekItem);            
            }
            wgc.WeekClasses = new List<ClassDetail>();
            foreach(var clt in db.GymClassTypes.OrderBy(ct=>ct.ClassTime).OrderBy(ct => ct.DayOfTheWeek).ToList()) {
                ClassDetail cl = new ClassDetail();
                cl.Description = clt.Description;
                cl.GymClassTypeId = clt.GymClassTypeId;
                cl.GymClassTypeName = clt.Name;
                cl.MaxCapacity = clt.MaxCapacity;
                cl.MaxWaitList = clt.MaxWaitList;
                int daysUntilClassDay = ((int)clt.DayOfTheWeek - (int)now.DayOfWeek + 7) % 7;
                string[] hms = clt.ClassTime.Split(':');
                cl.ClassDateTime = now.AddDays(daysUntilClassDay + 7 * nWeek).AddHours(int.Parse(hms[0])).AddMinutes(int.Parse(hms[1]));
               
                cl.ClassTime = clt.ClassTime;
                cl.ClassDate = cl.ClassDateTime.ToString("dddd dd MMM");
                cl.IsEditable = true;
                wgc.WeekClasses.Add(cl);
            }
            return View(wgc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult CreateWeekClasses(WeekGymClass weekGymClasses) {
            List<GymClass> newClasses = new List<GymClass>(0);
            foreach(var cl in weekGymClasses.WeekClasses) {
                if (cl.IsEditable) {
                    newClasses.Add(cl.ToModel());
                }
            }
            db.GymClass.AddRange(newClasses);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: GymClass/Details/5
        public ActionResult Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassBookingDetail gymClass = db.GymClass.Find(id).ToBookingViewModel(true);
            if (gymClass == null) {
                return HttpNotFound();
            }
            return View(gymClass);
        }

        // GET: GymClass/Create
        public ActionResult Create() {
            ClassDetail gymClass = new ClassDetail();
            gymClass.ClassDate = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
            gymClass.Types = db.GymClassTypes.OrderBy(ct=>ct.Name).ToList();
            return View(gymClass);
        }

        // POST: GymClass/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult Create([Bind(Include = "GymClassId,ClassTime,ClassDate,GymClassTypeId,Description,MaxCapacity,MaxWaitList")] ClassDetail gymClass) {
            if (ModelState.IsValid) {
                db.GymClass.Add(gymClass.ToModel());
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gymClass);
        }

        // GET: GymClass/Edit/5
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GymClass dbClass = db.GymClass.Find(id);
            ClassBookingDetail gymClass = dbClass.ToBookingViewModel(false);
            if (gymClass == null) {
                return HttpNotFound();
            }
            gymClass.Types = db.GymClassTypes.OrderBy(t=>t.Name).ToList();
            return View(gymClass);
        }

        // POST: GymClass/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult Edit([Bind(Include = "GymClassId,Description,GymClassTypeId,ClassTime,ClassDate,MaxCapacity,MaxWaitList,ClassBookings,ClassWaiting")] ClassBookingDetail gymClass) {
            Tex.Log("Enter GymClassController/Edit");
            try {
                if (ModelState.IsValid) {
                    if (gymClass != null) {
                        Tex.Log("Model Valid - GC ID " + gymClass.GymClassId);
                    }
                    GymClass dbClass = db.GymClass.Find(gymClass.GymClassId);
                    GymClassMapper.CopyGymClass(gymClass.ToModel(), dbClass);                 
                    db.Entry(dbClass).State = EntityState.Modified;

                    if (gymClass.ClassBookings != null && gymClass.ClassBookings.Any()) {
                        foreach (var booking in gymClass.ClassBookings) {
                            db.Entry(booking).State = EntityState.Modified;
                        }
                    }
                    if (gymClass.ClassWaiting != null && gymClass.ClassWaiting.Any()){
                        foreach (var booking in gymClass.ClassWaiting) {
                            db.Entry(booking).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(gymClass);
            } catch (Exception ex) {
                Tex.Dump(ex, "Excetption - GymClassControler/Edit");
                throw;
            }
        }

        // GET: GymClass/Delete/5
        [HttpGet, ActionName("Delete")]
        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult Delete(int id) {
            Tex.Log("Delete called" + id.ToString());
            try {
                GymClass gymClass = db.GymClass.Find(id);
                db.GymClass.Remove(gymClass);
                var bookings = db.MemberClassBookings.Where(cb => cb.GymClassId == id).ToList();
                db.MemberClassBookings.RemoveRange(bookings);
                db.SaveChanges();
                Tex.Log("returning");
                return Json("", JsonRequestBehavior.AllowGet);
            } catch (Exception ex) {
                Tex.Dump(ex, "Exception GymClassController/delete");
                throw;
            }
        }

        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult CancelBooking(int classId, int memberId) {
            if (!HomeController.CancelMemberBooking(classId, memberId, db)) {
                ViewBag.Error = "Your booking could not be cancelled!";
                return View("BookingError");
            }
            return RedirectToAction("Edit", new { id = classId });
        }

        public ActionResult Report() {
            return View(new ClassReport());
        }

        // POST: GymClass/Delete/5
        [HttpPost, ActionName("Report")]
        [ValidateAntiForgeryToken]
        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult Report(ClassReport classReport) {
            DateTime dFrom;
            DateTime dTo;
            DateTime dNow = DateTime.Now;

            bool dateFromOk = DateTime.TryParseExact(classReport.ClassDateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dFrom);
            bool dateToOk = DateTime.TryParseExact(classReport.ClassDateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dTo);
            if (dateFromOk && dateToOk) {
                classReport.TotalAttended = db.MemberClassBookings.Where(bk => bk.Attended && bk.GymClass.ClassDateTime >= dFrom && bk.GymClass.ClassDateTime <= dTo).Count();
                classReport.TotalBookings = db.MemberClassBookings.Where(bk => !bk.Waiting && bk.GymClass.ClassDateTime >= dFrom && bk.GymClass.ClassDateTime <= dTo).Count();
                classReport.TotalWaitingList = db.MemberClassBookings.Where(bk => bk.Waiting && bk.GymClass.ClassDateTime >= dFrom && bk.GymClass.ClassDateTime <= dTo).Count();
                if (dFrom < dNow && dNow < dTo) {
                    dTo = dNow;
                }
                if (dFrom < dNow && dTo <= dNow){
                    classReport.NoShowMembers = NoShowList(dFrom, dTo);
                }
            }
            return View(classReport);
        }
        private List<MemberAttendance> NoShowList(DateTime dFrom, DateTime dTo){
            var gymMemberAttendanceList = new List<MemberAttendance>();
            var noShows = db.MemberClassBookings.Where(bk => !bk.Attended && !bk.Waiting && 
                                                        bk.GymClass.ClassDateTime >= dFrom && 
                                                        bk.GymClass.ClassDateTime <= dTo)
                                                        .OrderBy(bk => bk.GymMemberId).ToList();
            MemberAttendance currentMemberAttendance = new MemberAttendance();
            foreach (var noShow in noShows){
                if (currentMemberAttendance.Member != null && 
                    noShow.GymMemberId == currentMemberAttendance.Member.GymMemberId){
                    currentMemberAttendance.NoShows++;
                }
                else{
                    if (currentMemberAttendance.Member != null){
                        gymMemberAttendanceList.Add(currentMemberAttendance);
                    }
                    currentMemberAttendance = new MemberAttendance();
                    currentMemberAttendance.Member = db.GymMembers.Find(noShow.GymMemberId);
                    currentMemberAttendance.NoShows = 1;
                }
            }
            if (currentMemberAttendance.NoShows > 0){
                gymMemberAttendanceList.Add(currentMemberAttendance);
            }
            return gymMemberAttendanceList.OrderByDescending(ma => ma.NoShows).OrderBy(ma => ma.Member.LastName).ToList();
        }
        // POST: GymClass/Delete/5
        [HttpGet, ActionName("DeleteRange")]
        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult DeleteRange(string classDateFrom, string classDateTo) {
            DateTime dFrom;
            DateTime dTo;
            ClassReport classReport = new ClassReport();
            classReport.ClassDateFrom = classDateFrom;
            classReport.ClassDateTo = classDateTo;
            bool dateFromOk = DateTime.TryParseExact(classDateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dFrom);
            bool dateToOk = DateTime.TryParseExact(classDateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dTo);
            if (dateFromOk && dateToOk) {
                var bookings = db.MemberClassBookings.Where(bk => bk.GymClass.ClassDateTime >= dFrom && bk.GymClass.ClassDateTime <= dTo).ToList();
                db.MemberClassBookings.RemoveRange(bookings);
                var classes = db.GymClass.Where(bk => bk.ClassDateTime >= dFrom && bk.ClassDateTime <= dTo).ToList();
                db.GymClass.RemoveRange(classes);
                db.SaveChanges();
            }
            return RedirectToAction("Report", classReport);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}