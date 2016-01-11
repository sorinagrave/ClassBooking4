using ClassBooking.Attributes;
using ClassBooking.Authorisation;
using ClassBooking.Database;
using ClassBooking.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ClassBooking.Controllers {

    public class GymClassTypesController : Controller {
        private GymContext db = new GymContext();

        // GET: GymClassTypes
        public ActionResult Index() {
            return View(db.GymClassTypes.OrderBy(clt=>clt.DayOfTheWeek).ToList());
        }

        // GET: GymClassTypes/Details/5
        public ActionResult Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GymClassType gymClassType = db.GymClassTypes.Find(id);
            if (gymClassType == null) {
                return HttpNotFound();
            }
            return View(gymClassType);
        }

        // GET: GymClassTypes/Create
        public ActionResult Create() {
            return View();
        }

        [HttpGet]
        public ActionResult GymClassTypeDetails(int? classTypeId) {
            if (classTypeId != null) {
                return Json(db.GymClassTypes.Find(classTypeId), JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        // POST: GymClassTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult Create([Bind(Include = "GymClassTypeId,Name,Description,MaxCapacity,MaxWaitList,DayOfTheWeek,ClassTime")] GymClassType gymClassType) {
            if (ModelState.IsValid) {
                db.GymClassTypes.Add(gymClassType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(gymClassType);
        }

        // GET: GymClassTypes/Edit/5
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GymClassType gymClassType = db.GymClassTypes.Find(id);
            if (gymClassType == null) {
                return HttpNotFound();
            }
            return View(gymClassType);
        }

        // POST: GymClassTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult Edit([Bind(Include = "GymClassTypeId,Name,Description,MaxCapacity,MaxWaitList,DayOfTheWeek,ClassTime")] GymClassType gymClassType) {
            if (ModelState.IsValid) {
                db.Entry(gymClassType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gymClassType);
        }

        // GET: GymClassTypes/Delete/5
        [HttpGet, ActionName("Delete")]
        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult Delete(int id) {
            GymClassType gymClassType = db.GymClassTypes.Find(id);
            db.GymClassTypes.Remove(gymClassType);
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