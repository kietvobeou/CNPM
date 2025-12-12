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
    public class CinemaController : Controller
    {
        QuanLyRapPhimEntities db = new QuanLyRapPhimEntities();

        // GET: Cinema/Index
        public ActionResult Index()
        {
            ViewBag.PhimSC = db.PHIM
             .Where(p => p.TrangThai == "Sắp chiếu")
             .OrderBy(r => Guid.NewGuid())
             .Take(4)
             .ToList();

            ViewBag.PhimDC = db.PHIM
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
        
       


        // GET: Cinema/ChiTiet/1
        public ActionResult ChiTiet(int id)
        {
            var phim = db.PHIM.FirstOrDefault(s => s.IDPhim == id);
            List<BINH_LUAN> bl = db.BINH_LUAN.Where(s => s.IDPhim == id).ToList();
            ViewBag.bl = bl;
            return View(phim);
        }

        // GET: Cinema/DatVe/1
        public ActionResult DatVe(int id)
        {
            var phim = db.PHIM.Find(id);
            if (phim == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách rạp
            var danhSachRap = db.RAP_PHIM.ToList();

            ViewBag.danhSachRap = danhSachRap;
            ViewBag.chiTietDatVe = new List<CT_DATVE>();

            return View(phim);
        }

        // GET: Cinema/GetSuatChieuByRapAndDate
        [HttpGet]
        public JsonResult GetSuatChieuByRapAndDate(int idRap, int idPhim, string ngayChieu)
        {
            try
            {
                DateTime ngayChieuDate = DateTime.Parse(ngayChieu);

                // Gọi stored procedure
                var suatChieuList = db.Database.SqlQuery<SuatChieuViewModel>(
                    "EXEC sp_GetSuatChieuByRapPhimNgay @IDRap, @IDPhim, @NgayChieu",
                    new SqlParameter("@IDRap", idRap),
                    new SqlParameter("@IDPhim", idPhim),
                    new SqlParameter("@NgayChieu", ngayChieuDate)
                ).ToList();

                return Json(new
                {
                    success = true,
                    data = suatChieuList
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Cinema/GetGheDaDat
        [HttpGet]
        public JsonResult GetGheDaDat(int idXuatChieu)
        {
            try
            {
                // Gọi stored procedure
                var gheDaDat = db.Database.SqlQuery<string>(
                    "EXEC sp_KiemTraGheDaDat @IDXuatChieu",
                    new SqlParameter("@IDXuatChieu", idXuatChieu)
                ).ToList();

                return Json(new
                {
                    success = true,
                    data = gheDaDat
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Cinema/XacNhanDatVe
        [HttpPost]
        public JsonResult XacNhanDatVe(BookingModel model)
        {
            try
            {
                // Lấy ID khách hàng từ session (tạm thời dùng ID 1 để test)
                int idKhachHang = 1; // Test với khách hàng ID 1

                // Hoặc nếu có session:
                // int? idKhachHang = Session["IDKhachHang"] as int?;
                // if (!idKhachHang.HasValue) idKhachHang = 1;

                // Tạo danh sách ghế định dạng "A1:85000,A2:85000"
                string danhSachGhe = string.Join(",",
                    model.danhSachGhe.Select(ghe => $"{ghe}:{GetGiaVe(model.idXuatChieu)}"));

                // Gọi stored procedure đặt vé
                var result = db.Database.SqlQuery<BookingResult>(
                    "EXEC sp_DatVe @IDKhachHang, @IDXuatChieu, @DanhSachGhe",
                    new SqlParameter("@IDKhachHang", idKhachHang),
                    new SqlParameter("@IDXuatChieu", model.idXuatChieu),
                    new SqlParameter("@DanhSachGhe", danhSachGhe)
                ).FirstOrDefault();

                if (result != null && result.IDDonDatVe > 0)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Đặt vé thành công!",
                        redirectUrl = Url.Action("ThanhToan", "Cinema", new
                        {
                            idXuatChieu = model.idXuatChieu,
                            danhSachGhe = string.Join(",", model.danhSachGhe)
                        })
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Đặt vé thất bại!"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Lỗi: " + ex.Message
                });
            }
        }

        // GET: Cinema/ThanhToan
        public ActionResult ThanhToan(int idXuatChieu, string danhSachGhe)
        {
            try
            {
                // Lấy thông tin xuất chiếu kèm các bảng liên quan
                var xuatChieu = db.XUAT_CHIEU
                    .Include("PHIM")
                    .Include("PHONG_CHIEU")
                    .Include("PHONG_CHIEU.RAP_PHIM")
                    .FirstOrDefault(x => x.IDXuatChieu == idXuatChieu);

                if (xuatChieu == null)
                {
                    return HttpNotFound();
                }

                // Parse danh sách ghế
                var gheList = danhSachGhe?.Split(',').ToList() ?? new List<string>();

                // Tính tổng tiền
                decimal tongTien = gheList.Count * xuatChieu.GiaVe;

                // Truyền dữ liệu qua ViewBag
                ViewBag.IdXuatChieu = idXuatChieu;
                ViewBag.DanhSachGhe = danhSachGhe;
                ViewBag.SoLuongVe = gheList.Count;
                ViewBag.TongTien = tongTien;
                ViewBag.GiaVe = xuatChieu.GiaVe;
                ViewBag.GheList = gheList;

                return View(xuatChieu);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // POST: Cinema/XuLyThanhToan
        [HttpPost]
        public JsonResult XuLyThanhToan(int idXuatChieu, string danhSachGhe, string phuongThucThanhToan)
        {
            try
            {
                // Lấy ID khách hàng (tạm thời dùng ID 1 để test)
                int idKhachHang = 1;

                // Parse danh sách ghế
                var gheList = danhSachGhe?.Split(',').ToList() ?? new List<string>();

                // Lấy thông tin xuất chiếu để lấy giá vé
                var xuatChieu = db.XUAT_CHIEU.Find(idXuatChieu);
                if (xuatChieu == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Suất chiếu không tồn tại!"
                    });
                }

                // Tạo danh sách ghế định dạng "A1:85000,A2:85000"
                string danhSachGheFormatted = string.Join(",",
                    gheList.Select(ghe => $"{ghe}:{xuatChieu.GiaVe}"));

                // Gọi stored procedure đặt vé
                var result = db.Database.SqlQuery<BookingResult>(
                    "EXEC sp_DatVe @IDKhachHang, @IDXuatChieu, @DanhSachGhe",
                    new SqlParameter("@IDKhachHang", idKhachHang),
                    new SqlParameter("@IDXuatChieu", idXuatChieu),
                    new SqlParameter("@DanhSachGhe", danhSachGheFormatted)
                ).FirstOrDefault();

                if (result != null && result.IDDonDatVe > 0)
                {
                    // Có thể thêm code lưu phương thức thanh toán vào đây

                    return Json(new
                    {
                        success = true,
                        message = "Thanh toán thành công!",
                        idDonDatVe = result.IDDonDatVe
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Thanh toán thất bại!"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Lỗi: " + ex.Message
                });
            }
        }

        // GET: Cinema/XacNhanThanhCong
        public ActionResult XacNhanThanhCong(int idDonDatVe)
        {
            // Lấy thông tin đơn đặt vé
            var donDatVe = db.DON_DAT_VE
                .Include("XUAT_CHIEU")
                .Include("XUAT_CHIEU.PHIM")
                .FirstOrDefault(d => d.IDDonDatVe == idDonDatVe);

            if (donDatVe == null)
            {
                return HttpNotFound();
            }

            ViewBag.IdDonDatVe = idDonDatVe;
            return View(donDatVe);
        }

        // Helper method
        private decimal GetGiaVe(int idXuatChieu)
        {
            var xuatChieu = db.XUAT_CHIEU.Find(idXuatChieu);
            return xuatChieu?.GiaVe ?? 0;
        }

        // Model classes
        public class BookingModel
        {
            public int idPhim { get; set; }
            public int idXuatChieu { get; set; }
            public string ngayChieu { get; set; }
            public string gioChieu { get; set; }
            public List<string> danhSachGhe { get; set; }
        }

        public class BookingResult
        {
            public int IDDonDatVe { get; set; }
            public decimal TongTien { get; set; }
        }

        public class SuatChieuViewModel
        {
            public int IDXuatChieu { get; set; }
            public int IDPhim { get; set; }
            public int IDPhong { get; set; }
            public DateTime NgayChieu { get; set; }
            public string GioChieu { get; set; }
            public decimal GiaVe { get; set; }
            public string TenPhong { get; set; }
            public int SoLuongGhe { get; set; }
            public string LoaiPhong { get; set; }
            public int IDRap { get; set; }
            public string TenRap { get; set; }
            public string DiaChi { get; set; }
            public string TenPhim { get; set; }
            public int ThoiLuong { get; set; }
        }

        public class PHIM
        {
            public int IDPhim { get; set; }
            public string TenPhim { get; set; }
            public string TrangThai { get; set; }
            // Thêm các thuộc tính khác nếu cần
            public int ThoiLuong { get; set; }
        }
    }
}