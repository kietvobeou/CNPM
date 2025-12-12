using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using phim.Models;

namespace phim.Controllers
{
    public class PhongChieuController : Controller
    {
        private QuanLyRapPhimEntities db = new QuanLyRapPhimEntities();

        // GET: PhongChieu
        public ActionResult Index()
        {
            var pHONG_CHIEU = db.PHONG_CHIEU.Include(p => p.RAP_PHIM);
            return View(pHONG_CHIEU.ToList());
        }

        // GET: PhongChieu/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PHONG_CHIEU pHONG_CHIEU = db.PHONG_CHIEU.Find(id);
            if (pHONG_CHIEU == null)
            {
                return HttpNotFound();
            }
            return View(pHONG_CHIEU);
        }

        // GET: PhongChieu/Create
        public ActionResult Create()
        {
            ViewBag.IDRap = new SelectList(db.RAP_PHIM, "IDRap", "TenRap");
            return View();
        }

        // POST: PhongChieu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDPhong,IDRap,TenPhong,SoLuongGhe,LoaiPhong")] PHONG_CHIEU pHONG_CHIEU)
        {
            if (ModelState.IsValid)
            {
                db.PHONG_CHIEU.Add(pHONG_CHIEU);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDRap = new SelectList(db.RAP_PHIM, "IDRap", "TenRap", pHONG_CHIEU.IDRap);
            return View(pHONG_CHIEU);
        }

        // GET: PhongChieu/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PHONG_CHIEU pHONG_CHIEU = db.PHONG_CHIEU.Find(id);
            if (pHONG_CHIEU == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDRap = new SelectList(db.RAP_PHIM, "IDRap", "TenRap", pHONG_CHIEU.IDRap);
            return View(pHONG_CHIEU);
        }

        // POST: PhongChieu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDPhong,IDRap,TenPhong,SoLuongGhe,LoaiPhong")] PHONG_CHIEU pHONG_CHIEU)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pHONG_CHIEU).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDRap = new SelectList(db.RAP_PHIM, "IDRap", "TenRap", pHONG_CHIEU.IDRap);
            return View(pHONG_CHIEU);
        }

        // GET: PhongChieu/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PHONG_CHIEU pHONG_CHIEU = db.PHONG_CHIEU.Find(id);
            if (pHONG_CHIEU == null)
            {
                return HttpNotFound();
            }
            return View(pHONG_CHIEU);
        }

        // POST: PhongChieu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PHONG_CHIEU pHONG_CHIEU = db.PHONG_CHIEU.Find(id);
            db.PHONG_CHIEU.Remove(pHONG_CHIEU);
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
