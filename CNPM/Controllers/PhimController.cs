using CNPM;   
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNPM.Models;

namespace CNPM.Controllers
{
    public class PhimController : Controller
    {
        private QuanLyRapPhimEntities db = new QuanLyRapPhimEntities();

        // ========== TRANG PHIM ĐANG CHIẾU ==========
        public ActionResult DangChieu()
        {
            var list = db.PHIMs.Where(x => x.TrangThai == "Đang chiếu").ToList();
            return View(list);
        }

        // ========== TRANG PHIM SẮP CHIẾU ==========
        public ActionResult SapChieu()
        {
            var list = db.PHIMs.Where(x => x.TrangThai == "Sắp chiếu").ToList();
            return View(list);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(PHIM model, HttpPostedFileBase AnhBiaFile)
        {
            if (ModelState.IsValid)
            {
                // 1. Thêm phim vào DB trước để có IDPhim
                db.PHIMs.Add(model);
                db.SaveChanges();

                // 2. Nếu có upload ảnh
                if (AnhBiaFile != null && AnhBiaFile.ContentLength > 0)
                {
                    string extension = Path.GetExtension(AnhBiaFile.FileName);
                    string fileName = "AnhBia_" + model.IDPhim + extension;
                    string path = Path.Combine(Server.MapPath("~/Content/HinhAnh"), fileName);

                    AnhBiaFile.SaveAs(path);

                    // 3. Cập nhật lại tên file ảnh vào DB
                    model.AnhBia = fileName;
                    db.SaveChanges();
                }

                // Trở về trang Sắp chiếu (hoặc Đang chiếu)
                return RedirectToAction("SapChieu");
            }

            return View(model);
        }
        public ActionResult Delete(int id)
        {
            var phim = db.PHIMs.Find(id);

            if (phim == null)
                return HttpNotFound();

            return View(phim);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var phim = db.PHIMs.Find(id);

            if (phim == null)
                return HttpNotFound();

            // ========================
            // XÓA ẢNH BÌA NẾU TỒN TẠI
            // ========================
            if (!string.IsNullOrEmpty(phim.AnhBia))
            {
                string path = Server.MapPath("~/Content/HinhAnh/" + phim.AnhBia);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }

            // Xóa phim trong database
            db.PHIMs.Remove(phim);
            db.SaveChanges();

            return RedirectToAction("SapChieu"); // Hoặc DangChieu tùy bạn
        }
        public ActionResult Edit(int id)
        {
            var phim = db.PHIMs.Find(id);

            if (phim == null)
                return HttpNotFound();

            return View(phim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PHIM model, HttpPostedFileBase AnhBiaFile, HttpPostedFileBase AnhNenFile)
        {
            if (ModelState.IsValid)
            {
                var phim = db.PHIMs.Find(model.IDPhim);

                if (phim == null)
                    return HttpNotFound();

                phim.TenPhim = model.TenPhim;
                phim.DaoDien = model.DaoDien;
                phim.TheLoai = model.TheLoai;
                phim.ThoiLuong = model.ThoiLuong;
                phim.TomTat = model.TomTat;
                phim.TrangThai = model.TrangThai;
                if (AnhBiaFile != null && AnhBiaFile.ContentLength > 0)
                {
                    if (!string.IsNullOrEmpty(phim.AnhBia))
                    {
                        string oldPath = Path.Combine(Server.MapPath("~/Content/HinhAnh"), phim.AnhBia);
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }
                    string extension = Path.GetExtension(AnhBiaFile.FileName);
                    string fileName = "AnhBia_" + phim.IDPhim + extension;
                    string path = Path.Combine(Server.MapPath("~/Content/HinhAnh"), fileName);

                    AnhBiaFile.SaveAs(path);
                    phim.AnhBia = fileName;
                }
                if (AnhNenFile != null && AnhNenFile.ContentLength > 0)
                {
                    if (!string.IsNullOrEmpty(phim.AnhNen))
                    {
                        string oldPath = Path.Combine(Server.MapPath("~/Content/HinhAnh"), phim.AnhNen);
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }
                    string extension = Path.GetExtension(AnhNenFile.FileName);
                    string fileName = "AnhNen_" + phim.IDPhim + extension;
                    string path = Path.Combine(Server.MapPath("~/Content/HinhAnh"), fileName);

                    AnhNenFile.SaveAs(path);
                    phim.AnhNen = fileName;
                }

                db.SaveChanges();
                if (phim.TrangThai == "Sắp chiếu")
                    return RedirectToAction("SapChieu");
                else
                    return RedirectToAction("DangChieu");
            }

            return View(model);
        }
    }
}
