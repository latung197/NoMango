using System;
using System.Collections.Generic;

namespace AIOI.Models
{
    public class PhieuNhapKho
    {
        public string SoPhieu { get; set; }
        public DateTime NgayNhap { get; set; }
        public string NhaCungCap { get; set; }
        public string KhoNhap { get; set; }
        public string NguoiGiao { get; set; }
        public string LyDoNhap { get; set; }
        public string NguoiTao { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }
        public string GhiChu { get; set; }
        public List<ChiTietPhieuNhap> ChiTiet { get; set; }

        public PhieuNhapKho()
        {
            ChiTiet = new List<ChiTietPhieuNhap>();
            NgayNhap = DateTime.Now;
            TrangThai = "Mới";
            NguoiTao = "admin";
        }
    }

    public class ChiTietPhieuNhap
    {
        public string MaVatTu { get; set; }
        public string TenVatTu { get; set; }
        public string DonViTinh { get; set; }
        public decimal SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string ViTri { get; set; }
        public string GhiChu { get; set; }

        public void TinhThanhTien()
        {
            ThanhTien = SoLuong * DonGia;
        }
    }
}