using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNPM.Models;

namespace CNPM.Controllers
{
    public class TaiKhoanController : Controller
    {
        private string connectionString = @"Data Source=LAPTOP-UDIS316D\SQLEXPRESS;Initial Catalog=QuanLyRapPhim;Integrated Security=True";

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT tk.*, kh.* 
                        FROM TAI_KHOAN tk
                        LEFT JOIN KHACH_HANG kh ON tk.IDKhachHang = kh.IDKhachHang
                        WHERE tk.TrangThai = 1 
                        AND tk.MatKhau = @Password
                        AND (
                            tk.TenDangNhap = @Username 
                            OR kh.Email = @Username 
                            OR kh.SoDienThoai = @Username
                        )";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Session["UserID"] = reader["IDTaiKhoan"];
                                Session["Role"] = reader["Loai"];

                                if (reader["IDKhachHang"] != DBNull.Value)
                                {
                                    Session["KhachHangID"] = Convert.ToInt32(reader["IDKhachHang"]);
                                    Session["HoTen"] = reader["HoTen"].ToString();
                                }

                                return RedirectToAction("Index", "Cinema");
                            }
                            else
                            {
                                ViewBag.Error = "Tài khoản hoặc mật khẩu không đúng";
                                return View();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Đã xảy ra lỗi: " + ex.Message;
                return View();
            }
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string fullname, string phone, string email, string password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string checkEmail = "SELECT COUNT(*) FROM KHACH_HANG WHERE Email = @Email";
                            using (SqlCommand checkCmd = new SqlCommand(checkEmail, conn, transaction))
                            {
                                checkCmd.Parameters.AddWithValue("@Email", email);
                                int emailCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                                if (emailCount > 0)
                                {
                                    transaction.Rollback();
                                    ViewBag.Error = "Email đã được sử dụng!";
                                    return View();
                                }
                            }

                            string insertKhachHang = @"
                                INSERT INTO KHACH_HANG (HoTen, SoDienThoai, Email, MaKhach, NgayDangKy)
                                VALUES (@HoTen, @SoDienThoai, @Email, '', GETDATE());
                                SELECT SCOPE_IDENTITY();";

                            int idKhachHang;
                            using (SqlCommand cmdKhachHang = new SqlCommand(insertKhachHang, conn, transaction))
                            {
                                cmdKhachHang.Parameters.AddWithValue("@HoTen", fullname);
                                cmdKhachHang.Parameters.AddWithValue("@SoDienThoai", phone);
                                cmdKhachHang.Parameters.AddWithValue("@Email", email);

                                idKhachHang = Convert.ToInt32(cmdKhachHang.ExecuteScalar());
                            }

                            string insertTaiKhoan = @"
                                INSERT INTO TAI_KHOAN (IDKhachHang, TenDangNhap, MatKhau, Loai, TrangThai, NgayTao)
                                VALUES (@IDKhachHang, @TenDangNhap, @MatKhau, @Loai, 1, GETDATE())";

                            using (SqlCommand cmdTaiKhoan = new SqlCommand(insertTaiKhoan, conn, transaction))
                            {
                                cmdTaiKhoan.Parameters.AddWithValue("@IDKhachHang", idKhachHang);
                                cmdTaiKhoan.Parameters.AddWithValue("@TenDangNhap", email);
                                cmdTaiKhoan.Parameters.AddWithValue("@MatKhau", password);
                                cmdTaiKhoan.Parameters.AddWithValue("@Loai", "khach");

                                cmdTaiKhoan.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            Session["UserID"] = email;
                            Session["Role"] = "khach";
                            Session["HoTen"] = fullname;
                            Session["KhachHangID"] = idKhachHang;

                            return RedirectToAction("Index", "Cinema");
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            ViewBag.Error = "Đăng ký thất bại: " + ex.Message;
                            return View();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Đã xảy ra lỗi: " + ex.Message;
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Cinema");
        }

        public ActionResult MyTickets()
        {
            if (Session["UserID"] == null || Session["Role"] == null)
            {
                return RedirectToAction("Login", "TaiKhoan");
            }
            if (Session["Role"].ToString() != "khach")
            {
                return RedirectToAction("Index", "Cinema");
            }

            List<VeModel> veSapChieu = new List<VeModel>();
            List<VeModel> veLichSu = new List<VeModel>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                SELECT 
                    ddv.IDDonDatVe,
                    p.TenPhim,
                    r.TenRap,
                    xc.NgayChieu,
                    xc.GioChieu,
                    ddv.TongTien,
                    ddv.TrangThaiDatVe,
                    STUFF((
                        SELECT ', ' + ct.ViTriGhe
                        FROM CT_DATVE ct
                        WHERE ct.IDDonDatVe = ddv.IDDonDatVe
                        FOR XML PATH('')
                    ), 1, 2, '') AS ChuoiGhe,
                    ddv.NgayDat
                FROM DON_DAT_VE ddv
                JOIN XUAT_CHIEU xc ON ddv.IDXuatChieu = xc.IDXuatChieu
                JOIN PHIM p ON xc.IDPhim = p.IDPhim
                JOIN PHONG_CHIEU pc ON xc.IDPhong = pc.IDPhong
                JOIN RAP_PHIM r ON pc.IDRap = r.IDRap
                WHERE ddv.IDKhachHang = @IDKhachHang
                ORDER BY ddv.NgayDat DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@IDKhachHang", Session["KhachHangID"]);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            DateTime now = DateTime.Now;

                            while (reader.Read())
                            {
                                var ve = new VeModel
                                {
                                    IDDonDatVe = Convert.ToInt32(reader["IDDonDatVe"]),
                                    TenPhim = reader["TenPhim"].ToString(),
                                    TenRap = reader["TenRap"].ToString(),
                                    NgayChieu = Convert.ToDateTime(reader["NgayChieu"]),
                                    GioChieu = TimeSpan.Parse(reader["GioChieu"].ToString()),
                                    TongTien = Convert.ToDecimal(reader["TongTien"]),
                                    TrangThaiDatVe = reader["TrangThaiDatVe"].ToString(),
                                    ChuoiGhe = reader["ChuoiGhe"] != DBNull.Value ? reader["ChuoiGhe"].ToString() : "",
                                    NgayDat = Convert.ToDateTime(reader["NgayDat"])
                                };

                                DateTime ngayGioChieu = ve.NgayChieu.Date.Add(ve.GioChieu);

                                if (ngayGioChieu >= now && ve.TrangThaiDatVe != "Đã hủy")
                                {
                                    veSapChieu.Add(ve);
                                }
                                else
                                {
                                    veLichSu.Add(ve);
                                }
                            }
                        }
                    }
                }

                // Chuyển sang ViewBag hoặc ViewModel
                ViewBag.VeSapChieu = veSapChieu;
                ViewBag.VeLichSu = veLichSu;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi tải danh sách vé: " + ex.Message;
                return View();
            }
        }

        public ActionResult TicketDetail(int id)
        {
            if (Session["UserID"] == null || Session["Role"] == null)
            {
                return RedirectToAction("Login", "TaiKhoan");
            }
            if (Session["Role"].ToString() != "khach")
            {
                return RedirectToAction("Index", "Cinema");
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Query lấy chi tiết vé
                    string query = @"
                        SELECT 
                            ddv.IDDonDatVe,
                            p.TenPhim,
                            r.TenRap,
                            pc.TenPhong,
                            xc.NgayChieu,
                            xc.GioChieu,
                            ddv.TrangThaiDatVe AS TrangThai,
                            ddv.TongTien,
                            p.AnhBia,
                            STUFF((
                                SELECT ', ' + ct.ViTriGhe
                                FROM CT_DATVE ct
                                WHERE ct.IDDonDatVe = ddv.IDDonDatVe
                                FOR XML PATH('')
                            ), 1, 2, '') AS ChuoiGhe
                        FROM DON_DAT_VE ddv
                        INNER JOIN XUAT_CHIEU xc ON ddv.IDXuatChieu = xc.IDXuatChieu
                        INNER JOIN PHIM p ON xc.IDPhim = p.IDPhim
                        INNER JOIN PHONG_CHIEU pc ON xc.IDPhong = pc.IDPhong
                        INNER JOIN RAP_PHIM r ON pc.IDRap = r.IDRap
                        WHERE ddv.IDDonDatVe = @IDDonDatVe
                        AND ddv.IDKhachHang = @IDKhachHang";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@IDDonDatVe", id);
                        cmd.Parameters.AddWithValue("@IDKhachHang", Session["KhachHangID"]);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var model = new TicketDetailModel
                                {
                                    IDDonDatVe = Convert.ToInt32(reader["IDDonDatVe"]),
                                    TenPhim = reader["TenPhim"].ToString(),
                                    TenRap = reader["TenRap"].ToString(),
                                    TenPhong = reader["TenPhong"].ToString(),
                                    NgayChieu = Convert.ToDateTime(reader["NgayChieu"]),
                                    GioChieu = TimeSpan.Parse(reader["GioChieu"].ToString()),
                                    TrangThai = reader["TrangThai"].ToString(),
                                    TongTien = Convert.ToDecimal(reader["TongTien"]),
                                    AnhBia = reader["AnhBia"] != DBNull.Value ? reader["AnhBia"].ToString() : "default_poster.jpg",
                                    ChuoiGhe = reader["ChuoiGhe"] != DBNull.Value ? reader["ChuoiGhe"].ToString() : ""
                                };

                                return View(model);
                            }
                            else
                            {
                                ViewBag.Error = "Không tìm thấy thông tin vé hoặc vé không thuộc về bạn.";
                                return RedirectToAction("MyTickets");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi tải chi tiết vé: " + ex.Message;
                return RedirectToAction("MyTickets");
            }
        }

        [HttpPost]
        public ActionResult CancelTicket(int id)
        {
            try
            {
                if (Session["UserID"] == null || Session["Role"] == null)
                {
                    return RedirectToAction("Login", "TaiKhoan");
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string checkQuery = @"
                SELECT COUNT(*) 
                FROM DON_DAT_VE 
                WHERE IDDonDatVe = @IDDonDatVe 
                AND IDKhachHang = @IDKhachHang 
                AND TrangThaiDatVe != N'Đã hủy'";

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@IDDonDatVe", id);
                        checkCmd.Parameters.AddWithValue("@IDKhachHang", Session["KhachHangID"]);

                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count == 0)
                        {
                            TempData["ErrorMessage"] = "Không thể hủy vé hoặc vé không tồn tại!";
                            return RedirectToAction("TicketDetail", "TaiKhoan", new { id = id });
                        }
                    }

                    // Cập nhật trạng thái hủy
                    string updateQuery = @"
                UPDATE DON_DAT_VE 
                SET TrangThaiDatVe = N'Đã hủy' 
                WHERE IDDonDatVe = @IDDonDatVe 
                AND IDKhachHang = @IDKhachHang";

                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@IDDonDatVe", id);
                        updateCmd.Parameters.AddWithValue("@IDKhachHang", Session["KhachHangID"]);

                        int rowsAffected = updateCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            TempData["SuccessMessage"] = "Hủy vé thành công!";
                            return RedirectToAction("Index", "Cinema");
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Không thể hủy vé!";
                            return RedirectToAction("TicketDetail", "TaiKhoan", new { id = id });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
                return RedirectToAction("TicketDetail", "TaiKhoan", new { id = id });
            }
        }

        public class TicketDetailModel
        {
            public int IDDonDatVe { get; set; }
            public string TenPhim { get; set; }
            public string TenRap { get; set; }
            public string TenPhong { get; set; }
            public DateTime NgayChieu { get; set; }
            public TimeSpan GioChieu { get; set; }
            public string TrangThai { get; set; }
            public decimal TongTien { get; set; }
            public string AnhBia { get; set; }
            public string ChuoiGhe { get; set; }
        }
    }
}