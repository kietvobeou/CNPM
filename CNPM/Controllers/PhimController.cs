using System.Linq;
using System.Web.Mvc;
using CNPM;   

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
    }
}
