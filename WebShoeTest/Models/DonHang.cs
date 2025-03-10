using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using WebShoeTest.Models;

public class DonHang
{
    [Key] // Định nghĩa khóa chính
    public int MaDonHang { get; set; }

    public int MaKhachHang { get; set; }
    public KhachHang KhachHang { get; set; }

    public int? MaNhanVien { get; set; }
    public NhanVien NhanVien { get; set; }

    public DateTime NgayDat { get; set; }
    public DateTime NgayNhan { get; set; }
    public string TrangThai { get; set; }

    // Quan hệ 1 - nhiều với ChiTietDonHang
    public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }

    // Quan hệ 1 - 1 với ThanhToan
    public ThanhToan ThanhToan { get; set; }
}
