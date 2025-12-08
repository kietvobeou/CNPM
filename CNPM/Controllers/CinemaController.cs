using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNPM.Controllers
{
    public class CinemaController : Controller
    {
        QuanLyRapPhimEntities db = new QuanLyRapPhimEntities();
        public ActionResult Index()
        {
            ViewBag.PhimSC = db.PHIMs
             .Where(p => p.TrangThai == "Sắp chiếu")
             .OrderBy(r => Guid.NewGuid())
             .Take(4)
             .ToList();

            ViewBag.PhimDC = db.PHIMs
                .Where(p => p.TrangThai == "Đang chiếu")
                .OrderBy(r => Guid.NewGuid())
                .Take(4)
                .ToList();

            ViewBag.Tin = db.TIN_TUC
                .OrderBy(r => Guid.NewGuid())
                .Take(4)
                .ToList();
            return View();
        }
    }
}