using CNPM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CNPM.Controllers
{
    public class AccountController : Controller
    {
        private QuanLyRapPhimEntities db = new QuanLyRapPhimEntities();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return View();
            }

            try
            {
                string passHash = GetMD5(password);
                var user = db.sp_DangNhap(username, passHash).FirstOrDefault();

                if (user != null)
                {
                    Session["UserID"] = user.IDTaiKhoan.ToString();
                    Session["Username"] = user.TenDangNhap;
                    Session["Role"] = user.Loai;

                    if (user.HoTen != null)
                    {
                        Session["UserFullName"] = user.HoTen;
                        Session["IDKhachHang"] = user.IDKhachHang.ToString();
                    }

                    if (user.Loai == "admin")
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không chính xác!";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi hệ thống: " + ex.Message;
            }

            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string username, string password, string confirmPassword, string fullname, string phone, string email)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp.";
                return View();
            }


            KHACH_HANG kh = new KHACH_HANG();

            try
            {

                kh.HoTen = fullname;
                kh.SoDienThoai = phone;
                kh.Email = email;
                kh.MaKhach = ""; 
                kh.NgayDangKy = DateTime.Now;

                db.KHACH_HANG.Add(kh);
                db.SaveChanges(); 

                int status = db.sp_DangKyTaiKhoan(kh.IDKhachHang, username, GetMD5(password), "khach");


                if (status == -1)
                {

                    db.KHACH_HANG.Remove(kh);
                    db.SaveChanges();

                    ViewBag.Error = "Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác.";
                    return View();
                }

                TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {

                if (kh.IDKhachHang > 0)
                {
                    try
                    {
                        var rac = db.KHACH_HANG.Find(kh.IDKhachHang);
                        if (rac != null) { db.KHACH_HANG.Remove(rac); db.SaveChanges(); }
                    }
                    catch { }
                }

                ViewBag.Error = "Đăng ký thất bại: " + ex.Message;
                return View();
            }
        }


        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        private string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }
            return byte2String;
        }


        public ActionResult MyTickets()
        {
   
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login");
            }

            int idKhachHang = 0;
            if (!int.TryParse(Session["IDKhachHang"]?.ToString(), out idKhachHang))
            {
                return RedirectToAction("Login");
            }

            var listResult = db.sp_GetVeCuaToi(idKhachHang).ToList();

   
            List<HistoryTicketViewModel> veSapChieu = new List<HistoryTicketViewModel>();
            List<HistoryTicketViewModel> veLichSu = new List<HistoryTicketViewModel>();

            foreach (var item in listResult)
            {

                HistoryTicketViewModel ticket = new HistoryTicketViewModel()
                {
                    IDDonDatVe = item.IDDonDatVe,
                    TenPhim = item.TenPhim,
                    TenRap = item.TenRap,
                    NgayChieu = item.NgayChieu,
                    GioChieu = item.GioChieu,
                    ChuoiGhe = item.ChuoiGhe, 
                    TongTien = (decimal)(item.TongTien ?? 0),
                    TrangThai = item.TrangThaiDatVe
                };

                DateTime thoiGianChieu = item.NgayChieu.Add(item.GioChieu);

                if (item.TrangThaiDatVe == "Đã hủy" || thoiGianChieu < DateTime.Now)
                {
                    veLichSu.Add(ticket);
                }
                else
                {
                    veSapChieu.Add(ticket);
                }
            }

            ViewBag.VeSapChieu = veSapChieu;
            ViewBag.VeLichSu = veLichSu;

            return View();
        }


        public ActionResult TicketDetail(int id)
        {
            if (Session["UserID"] == null) return RedirectToAction("Login");
            int idKhachHang = int.Parse(Session["IDKhachHang"].ToString());


            var ticket = (from ddv in db.DON_DAT_VE
                          join xc in db.XUAT_CHIEU on ddv.IDXuatChieu equals xc.IDXuatChieu
                          join p in db.PHIMs on xc.IDPhim equals p.IDPhim
                          join pc in db.PHONG_CHIEU on xc.IDPhong equals pc.IDPhong
                          join r in db.RAP_PHIM on pc.IDRap equals r.IDRap
                          where ddv.IDDonDatVe == id && ddv.IDKhachHang == idKhachHang
                          select new HistoryTicketViewModel
                          {
                              IDDonDatVe = ddv.IDDonDatVe,
                              TenPhim = p.TenPhim,
                              AnhBia = p.AnhBia, 
                              TenRap = r.TenRap,
                              TenPhong = pc.TenPhong,
                              NgayChieu = xc.NgayChieu,
                              GioChieu = xc.GioChieu,
                              TrangThai = ddv.TrangThaiDatVe
                          }).FirstOrDefault();

            if (ticket == null) return RedirectToAction("MyTickets");


            var listGhe = db.CT_DATVE.Where(g => g.IDDonDatVe == id).Select(g => g.ViTriGhe).ToList();
            ticket.ChuoiGhe = string.Join(", ", listGhe);

            return View(ticket);
        }


        [HttpPost]
        public ActionResult CancelTicket(int id)
        {
            if (Session["UserID"] == null) return RedirectToAction("Login");
            int idKhachHang = int.Parse(Session["IDKhachHang"].ToString());

            try
            {

                var result = db.sp_HuyVeCuaToi(id, idKhachHang).FirstOrDefault();

                if (result == 1)
                {
                    TempData["Success"] = "Đã hủy vé thành công!";
                }
                else
                {
                    TempData["Error"] = "Không thể hủy vé này (Vé không tồn tại hoặc đã hủy).";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi hệ thống: " + ex.Message;
            }

            return RedirectToAction("MyTickets");
        }


    }
}