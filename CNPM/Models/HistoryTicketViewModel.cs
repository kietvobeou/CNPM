using System;
using System.Collections.Generic;

namespace CNPM.Models
{

    public class HistoryTicketViewModel
    {
        public int IDDonDatVe { get; set; }
        public string TenPhim { get; set; }
        public string TenRap { get; set; }
        public string TenPhong { get; set; }
        public DateTime NgayChieu { get; set; }
        public TimeSpan GioChieu { get; set; }
        public string ChuoiGhe { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }
        public string AnhBia { get; set; } 
    }
}