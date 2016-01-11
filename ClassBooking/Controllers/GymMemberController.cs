using ClassBooking.Attributes;
using ClassBooking.Authorisation;
using ClassBooking.Database;
using ClassBooking.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ClassBooking.Controllers {

    public class GymMemberController : Controller {
        private GymContext db = new GymContext();

        // GET: GymMember
        public ActionResult Index() {
            return View(db.GymMembers.OrderBy(m=>m.LastName).ToList());
        }

        // GET: GymMember/Details/5
        public ActionResult Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GymMember gymMember = db.GymMembers.Find(id);
            if (gymMember == null) {
                return HttpNotFound();
            }
            return View(gymMember);
        }

        // GET: GymMember/Create
        public ActionResult Create() {
            return View();
        }

        // POST: GymMember/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult Create([Bind(Include = "GymMemberId,StaffId,FirstName,LastName,PhoneNumber,EmailAddress, IsAdmin")] GymMember gymMember) {
            if (ModelState.IsValid) {
                db.GymMembers.Add(gymMember);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(gymMember);
        }

        // GET: GymMember/Edit/5
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GymMember gymMember = db.GymMembers.Find(id);
            if (gymMember == null) {
                return HttpNotFound();
            }
            return View(gymMember);
        }

        // POST: GymMember/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult Edit([Bind(Include = "GymMemberId,StaffId,FirstName,LastName,PhoneNumber,EmailAddress,IsAdmin")] GymMember gymMember) {
            if (ModelState.IsValid) {
                db.Entry(gymMember).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gymMember);
        }

        // GET: GymMember/Delete/5
        [HttpGet]
        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult Delete(int id) {
            GymMember gymMember = db.GymMembers.Find(id);
            db.GymMembers.Remove(gymMember);
            var bookings = db.MemberClassBookings.Where(cb => cb.GymMemberId == id).ToList();
            db.MemberClassBookings.RemoveRange(bookings);
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}