using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNPM.Controllers
{
    public class RapPhimController : Controller
    {
        private QuanLyRapPhimEntities db = new QuanLyRapPhimEntities();

        // GET: RapPhim
        public ActionResult Index()
        {
            return View(db.RAP_PHIM.ToList());
        }

        // GET: RapPhim/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RapPhim/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RAP_PHIM rapPhim)
        {
            if (ModelState.IsValid)
            {
                db.RAP_PHIM.Add(rapPhim);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rapPhim);
        }

        // GET: RapPhim/Edit/5
        public ActionResult Edit(int id)
        {
            RAP_PHIM rapPhim = db.RAP_PHIM.Find(id);
            if (rapPhim == null)
            {
                return HttpNotFound();
            }
            return View(rapPhim);
        }

        // POST: RapPhim/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RAP_PHIM rapPhim)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rapPhim).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rapPhim);
        }

        // GET: RapPhim/Delete/5
        public ActionResult Delete(int id)
        {
            RAP_PHIM rapPhim = db.RAP_PHIM.Find(id);
            if (rapPhim == null)
            {
                return HttpNotFound();
            }
            return View(rapPhim);
        }

        // POST: RapPhim/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RAP_PHIM rapPhim = db.RAP_PHIM.Find(id);
            if (rapPhim == null)
            {
                return HttpNotFound();
            }
            if (rapPhim.PHONG_CHIEU.Any())
            {
                TempData["ErrorMessage"] = "Không thể xóa rạp vì còn phòng chiếu. Vui lòng xóa các phòng chiếu trước.";
                return RedirectToAction("Delete", new { id = id });
            }

            db.RAP_PHIM.Remove(rapPhim);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}