using CNPM;   
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNPM.Controllers
{
    public class PhimController : Controller
    {
        private QuanLyRapPhimEntities db = new QuanLyRapPhimEntities();

        // ========== TRANG PHIM ĐANG CHIẾU ==========
        public ActionResult DangChieu()
        {
            var list = db.PHIM.Where(x => x.TrangThai == "Đang chiếu").ToList();
            return View(list);
        }

        // ========== TRANG PHIM SẮP CHIẾU ==========
        public ActionResult SapChieu()
        {
            var list = db.PHIM.Where(x => x.TrangThai == "Sắp chiếu").ToList();
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
                db.PHIM.Add(model);
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
            var phim = db.PHIM.Find(id);

            if (phim == null)
                return HttpNotFound();

            return View(phim);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var phim = db.PHIM.Find(id);

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
            db.PHIM.Remove(phim);
            db.SaveChanges();

            return RedirectToAction("SapChieu"); // Hoặc DangChieu tùy bạn
        }

    }
}
