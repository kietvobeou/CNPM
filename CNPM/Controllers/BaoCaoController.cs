using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Web.Mvc;

namespace CNPM.Controllers
{
    public class BaoCaoController : Controller
    {
        private QuanLyRapPhimEntities db = new QuanLyRapPhimEntities();

        public ActionResult Index()
        {
            // Kiểm tra đăng nhập và quyền admin
            if (Session["UserID"] == null || Session["Role"] == null)
            {
                return RedirectToAction("Login", "TaiKhoan");
            }

            if (Session["Role"].ToString() != "admin")
            {
                return RedirectToAction("Index", "Cinema");
            }

            DateTime today = DateTime.Today;
            DateTime yesterday = today.AddDays(-1);
            DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            DateTime firstDayOfLastMonth = firstDayOfMonth.AddMonths(-1);
            DateTime lastDayOfLastMonth = firstDayOfMonth.AddDays(-1);

            // Sửa TẤT CẢ "Đã thanh toán" thành "Đã đặt"
            string trangThaiCanThongKe = "Đã đặt";

            // 1. Doanh thu hôm nay
            var doanhThuHomNay = db.DON_DAT_VE
                .Where(d => EntityFunctions.TruncateTime(d.NgayDat) == today &&
                           d.TrangThaiDatVe == trangThaiCanThongKe) // ĐÃ SỬA
                .Sum(d => (decimal?)d.TongTien) ?? 0;

            // 2. Doanh thu hôm qua
            var doanhThuHomQua = db.DON_DAT_VE
                .Where(d => EntityFunctions.TruncateTime(d.NgayDat) == yesterday &&
                           d.TrangThaiDatVe == trangThaiCanThongKe) // ĐÃ SỬA
                .Sum(d => (decimal?)d.TongTien) ?? 0;

            // 3. Tính phần trăm tăng/giảm
            double tyLeThayDoiHomNay = 0;
            if (doanhThuHomQua > 0)
            {
                tyLeThayDoiHomNay = ((double)(doanhThuHomNay - doanhThuHomQua) / (double)doanhThuHomQua) * 100;
            }
            else if (doanhThuHomNay > 0)
            {
                tyLeThayDoiHomNay = 100; // Tăng 100% so với 0
            }

            // 4. Số vé đã bán hôm nay
            var soVeHomNay = db.DON_DAT_VE
                .Where(d => EntityFunctions.TruncateTime(d.NgayDat) == today &&
                           d.TrangThaiDatVe == trangThaiCanThongKe) // ĐÃ SỬA
                .Join(db.CT_DATVE, d => d.IDDonDatVe, ct => ct.IDDonDatVe, (d, ct) => ct)
                .Count();

            // 5. Số vé đã bán hôm qua
            var soVeHomQua = db.DON_DAT_VE
                .Where(d => EntityFunctions.TruncateTime(d.NgayDat) == yesterday &&
                           d.TrangThaiDatVe == trangThaiCanThongKe) // ĐÃ SỬA
                .Join(db.CT_DATVE, d => d.IDDonDatVe, ct => ct.IDDonDatVe, (d, ct) => ct)
                .Count();

            // 6. Doanh thu tháng này
            var doanhThuThangNay = db.DON_DAT_VE
                .Where(d => d.NgayDat >= firstDayOfMonth &&
                           d.NgayDat <= lastDayOfMonth &&
                           d.TrangThaiDatVe == trangThaiCanThongKe) // ĐÃ SỬA
                .Sum(d => (decimal?)d.TongTien) ?? 0;

            // 7. Doanh thu tháng trước
            var doanhThuThangTruoc = db.DON_DAT_VE
                .Where(d => d.NgayDat >= firstDayOfLastMonth &&
                           d.NgayDat <= lastDayOfLastMonth &&
                           d.TrangThaiDatVe == trangThaiCanThongKe) // ĐÃ SỬA
                .Sum(d => (decimal?)d.TongTien) ?? 0;

            // 8. Tính phần trăm thay đổi tháng
            double tyLeThayDoiThang = 0;
            if (doanhThuThangTruoc > 0)
            {
                tyLeThayDoiThang = ((double)(doanhThuThangNay - doanhThuThangTruoc) / (double)doanhThuThangTruoc) * 100;
            }
            else if (doanhThuThangNay > 0)
            {
                tyLeThayDoiThang = 100; // Tăng 100% so với 0
            }

            // 9. Top phim bán chạy trong ngày
            var topPhimHomNay = db.DON_DAT_VE
                .Where(d => EntityFunctions.TruncateTime(d.NgayDat) == today &&
                           d.TrangThaiDatVe == trangThaiCanThongKe) // ĐÃ SỬA
                .Join(db.XUAT_CHIEU, d => d.IDXuatChieu, x => x.IDXuatChieu, (d, x) => x)
                .Join(db.PHIMs, x => x.IDPhim, p => p.IDPhim, (x, p) => new { Phim = p, SoVe = 1 })
                .GroupBy(g => g.Phim)
                .Select(g => new TopPhimViewModel
                {
                    TenPhim = g.Key.TenPhim,
                    SoVeDaBan = g.Count(),
                    DoanhThu = db.DON_DAT_VE
                        .Where(d => EntityFunctions.TruncateTime(d.NgayDat) == today &&
                                  d.TrangThaiDatVe == trangThaiCanThongKe) // ĐÃ SỬA
                        .Join(db.XUAT_CHIEU, d => d.IDXuatChieu, x => x.IDXuatChieu, (d, x) => new { Don = d, XuatChieu = x })
                        .Where(j => j.XuatChieu.IDPhim == g.Key.IDPhim)
                        .Sum(j => (decimal?)j.Don.TongTien) ?? 0
                })
                .OrderByDescending(p => p.SoVeDaBan)
                .Take(5)
                .ToList();

            // Debug: In ra console để kiểm tra
            System.Diagnostics.Debug.WriteLine($"Trạng thái đang tìm: {trangThaiCanThongKe}");
            System.Diagnostics.Debug.WriteLine($"Doanh thu hôm nay: {doanhThuHomNay}");
            System.Diagnostics.Debug.WriteLine($"Số vé hôm nay: {soVeHomNay}");

            // Định dạng dữ liệu để hiển thị
            ViewBag.DoanhThuHomNay = doanhThuHomNay.ToString("N0") + " VNĐ";
            ViewBag.TyLeThayDoiHomNay = tyLeThayDoiHomNay;
            ViewBag.TangGiamHomNay = tyLeThayDoiHomNay >= 0 ? "Tăng" : "Giảm";
            ViewBag.TangGiamHomNayClass = tyLeThayDoiHomNay >= 0 ? "text-success" : "text-danger";

            ViewBag.SoVeHomNay = soVeHomNay;
            ViewBag.SoVeHomQua = soVeHomQua;
            ViewBag.TangGiamVe = soVeHomNay >= soVeHomQua ? "Tăng" : "Giảm";
            ViewBag.TangGiamVeClass = soVeHomNay >= soVeHomQua ? "text-success" : "text-danger";
            ViewBag.ChenhLechVe = Math.Abs(soVeHomNay - soVeHomQua);

            ViewBag.DoanhThuThangNay = doanhThuThangNay.ToString("N0") + " VNĐ";
            ViewBag.TyLeThayDoiThang = Math.Abs(tyLeThayDoiThang);
            ViewBag.TangGiamThang = tyLeThayDoiThang >= 0 ? "Tăng" : "Giảm";
            ViewBag.TangGiamThangClass = tyLeThayDoiThang >= 0 ? "text-success" : "text-danger";

            ViewBag.TopPhimHomNay = topPhimHomNay;

            return View();
        }

        public ActionResult XuatExcel()
        {
            DateTime today = DateTime.Today;
            string trangThaiCanThongKe = "Đã đặt"; // ĐÃ SỬA

            var data = db.DON_DAT_VE
                .Where(d => EntityFunctions.TruncateTime(d.NgayDat) == today &&
                           d.TrangThaiDatVe == trangThaiCanThongKe) // ĐÃ SỬA
                .Join(db.XUAT_CHIEU, d => d.IDXuatChieu, x => x.IDXuatChieu, (d, x) => new { Don = d, XuatChieu = x })
                .Join(db.PHIMs, j => j.XuatChieu.IDPhim, p => p.IDPhim, (j, p) => new
                {
                    TenPhim = p.TenPhim,
                    NgayDat = j.Don.NgayDat,
                    TongTien = j.Don.TongTien,
                    SoVe = db.CT_DATVE.Count(ct => ct.IDDonDatVe == j.Don.IDDonDatVe)
                })
                .ToList();

            string csv = "Tên Phim,Ngày Đặt,Tổng Tiền,Số Vé\n";
            foreach (var item in data)
            {
                csv += $"\"{item.TenPhim}\",{item.NgayDat:dd/MM/yyyy HH:mm},{item.TongTien},{item.SoVe}\n";
            }

            byte[] fileContents = System.Text.Encoding.UTF8.GetBytes(csv);

            return File(fileContents, "text/csv", $"BaoCao_{today:ddMMyyyy}.csv");
        }

        // Thêm action để kiểm tra dữ liệu (tạm thời)
        public ActionResult KiemTraDuLieu()
        {
            if (Session["Role"]?.ToString() != "admin")
                return RedirectToAction("Index", "Cinema");

            var danhSachTrangThai = db.DON_DAT_VE
                .Select(d => d.TrangThaiDatVe)
                .Distinct()
                .ToList();

            ViewBag.DanhSachTrangThai = danhSachTrangThai;

            var donHomNay = db.DON_DAT_VE
                .Where(d => EntityFunctions.TruncateTime(d.NgayDat) == DateTime.Today)
                .ToList();

            ViewBag.DonHomNay = donHomNay;

            return View();
        }
    }

    public class TopPhimViewModel
    {
        public string TenPhim { get; set; }
        public int SoVeDaBan { get; set; }
        public decimal DoanhThu { get; set; }
    }
}