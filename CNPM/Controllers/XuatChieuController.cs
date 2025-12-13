using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNPM.Controllers
{
    public class XuatChieuController : Controller
    {
        private QuanLyRapPhimEntities db = new QuanLyRapPhimEntities();

        // GET: XuatChieu
        public ActionResult Index()
        {
            var xuatChieu = db.XUAT_CHIEU.Include(x => x.PHIM).Include(x => x.PHONG_CHIEU);
            return View(xuatChieu.ToList());
        }

        // GET: XuatChieu/Create
        public ActionResult Create()
        {
            ViewBag.IDPhim = new SelectList(db.PHIMs, "IDPhim", "TenPhim");
            ViewBag.IDPhong = new SelectList(db.PHONG_CHIEU, "IDPhong", "TenPhong");
            return View();
        }

        // POST: XuatChieu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(XUAT_CHIEU xuatChieu)
        {
            if (ModelState.IsValid)
            {
                db.XUAT_CHIEU.Add(xuatChieu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDPhim = new SelectList(db.PHIMs, "IDPhim", "TenPhim", xuatChieu.IDPhim);
            ViewBag.IDPhong = new SelectList(db.PHONG_CHIEU, "IDPhong", "TenPhong", xuatChieu.IDPhong);
            return View(xuatChieu);
        }

        // GET: XuatChieu/Edit/5
        public ActionResult Edit(int id)
        {
            XUAT_CHIEU xuatChieu = db.XUAT_CHIEU.Find(id);
            if (xuatChieu == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDPhim = new SelectList(db.PHIMs, "IDPhim", "TenPhim", xuatChieu.IDPhim);
            ViewBag.IDPhong = new SelectList(db.PHONG_CHIEU, "IDPhong", "TenPhong", xuatChieu.IDPhong);
            return View(xuatChieu);
        }

        // POST: XuatChieu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(XUAT_CHIEU xuatChieu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(xuatChieu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDPhim = new SelectList(db.PHIMs, "IDPhim", "TenPhim", xuatChieu.IDPhim);
            ViewBag.IDPhong = new SelectList(db.PHONG_CHIEU, "IDPhong", "TenPhong", xuatChieu.IDPhong);
            return View(xuatChieu);
        }

        // GET: XuatChieu/Delete/5
        public ActionResult Delete(int id)
        {
            XUAT_CHIEU xuatChieu = db.XUAT_CHIEU.Find(id);
            if (xuatChieu == null)
            {
                return HttpNotFound();
            }
            return View(xuatChieu);
        }

        // POST: XuatChieu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            XUAT_CHIEU xuatChieu = db.XUAT_CHIEU.Find(id);
            if (xuatChieu == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra xem xuất chiếu có đơn đặt vé nào không
            if (xuatChieu.DON_DAT_VE.Any())
            {
                TempData["ErrorMessage"] = "Không thể xóa xuất chiếu vì đã có đơn đặt vé. Vui lòng xóa các đơn đặt vé trước.";
                return RedirectToAction("Delete", new { id = id });
            }

            db.XUAT_CHIEU.Remove(xuatChieu);
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