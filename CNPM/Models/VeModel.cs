using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNPM.Models
{
    public class VeModel
    {
        public int IDDonDatVe { get; set; }
        public string TenPhim { get; set; }
        public string TenRap { get; set; }
        public DateTime NgayChieu { get; set; }
        public TimeSpan GioChieu { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThaiDatVe { get; set; }
        public string ChuoiGhe { get; set; }
        public DateTime NgayDat { get; set; }
    }
}