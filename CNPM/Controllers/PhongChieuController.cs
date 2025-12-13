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
    public class PhongChieuController : Controller
    {
        private QuanLyRapPhimEntities db = new QuanLyRapPhimEntities();

        // GET: PhongChieu
        public ActionResult Index()
        {
            var phongChieu = db.PHONG_CHIEU.Include(p => p.RAP_PHIM);
            return View(phongChieu.ToList());
        }

        // GET: PhongChieu/Create
        public ActionResult Create()
        {
            ViewBag.IDRap = new SelectList(db.RAP_PHIM, "IDRap", "TenRap");
            return View();
        }

        // POST: PhongChieu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PHONG_CHIEU phongChieu)
        {
            if (ModelState.IsValid)
            {
                db.PHONG_CHIEU.Add(phongChieu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDRap = new SelectList(db.RAP_PHIM, "IDRap", "TenRap", phongChieu.IDRap);
            return View(phongChieu);
        }

        // GET: PhongChieu/Edit/5
        public ActionResult Edit(int id)
        {
            PHONG_CHIEU phongChieu = db.PHONG_CHIEU.Find(id);
            if (phongChieu == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDRap = new SelectList(db.RAP_PHIM, "IDRap", "TenRap", phongChieu.IDRap);
            return View(phongChieu);
        }

        // POST: PhongChieu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PHONG_CHIEU phongChieu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(phongChieu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDRap = new SelectList(db.RAP_PHIM, "IDRap", "TenRap", phongChieu.IDRap);
            return View(phongChieu);
        }

        // GET: PhongChieu/Delete/5
        public ActionResult Delete(int id)
        {
            PHONG_CHIEU phongChieu = db.PHONG_CHIEU.Find(id);
            if (phongChieu == null)
            {
                return HttpNotFound();
            }
            return View(phongChieu);
        }

        // POST: PhongChieu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PHONG_CHIEU phongChieu = db.PHONG_CHIEU.Find(id);
            if (phongChieu == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra xem phòng có suất chiếu nào không
            if (phongChieu.XUAT_CHIEU.Any())
            {
                TempData["ErrorMessage"] = "Không thể xóa phòng vì còn suất chiếu. Vui lòng xóa các suất chiếu trước.";
                return RedirectToAction("Delete", new { id = id });
            }

            db.PHONG_CHIEU.Remove(phongChieu);
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