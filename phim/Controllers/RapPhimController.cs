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
    public class RapPhimController : Controller
    {
        private QuanLyRapPhimEntities db = new QuanLyRapPhimEntities();

        // GET: RapPhim
        public ActionResult Index()
        {
            return View(db.RAP_PHIM.ToList());
        }

        // GET: RapPhim/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RAP_PHIM rAP_PHIM = db.RAP_PHIM.Find(id);
            if (rAP_PHIM == null)
            {
                return HttpNotFound();
            }
            return View(rAP_PHIM);
        }

        // GET: RapPhim/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RapPhim/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDRap,TenRap,DiaChi,SoDienThoai,Email")] RAP_PHIM rAP_PHIM)
        {
            if (ModelState.IsValid)
            {
                db.RAP_PHIM.Add(rAP_PHIM);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rAP_PHIM);
        }

        // GET: RapPhim/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RAP_PHIM rAP_PHIM = db.RAP_PHIM.Find(id);
            if (rAP_PHIM == null)
            {
                return HttpNotFound();
            }
            return View(rAP_PHIM);
        }

        // POST: RapPhim/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDRap,TenRap,DiaChi,SoDienThoai,Email")] RAP_PHIM rAP_PHIM)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rAP_PHIM).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rAP_PHIM);
        }

        // GET: RapPhim/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RAP_PHIM rAP_PHIM = db.RAP_PHIM.Find(id);
            if (rAP_PHIM == null)
            {
                return HttpNotFound();
            }
            return View(rAP_PHIM);
        }

        // POST: RapPhim/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RAP_PHIM rAP_PHIM = db.RAP_PHIM.Find(id);
            db.RAP_PHIM.Remove(rAP_PHIM);
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
