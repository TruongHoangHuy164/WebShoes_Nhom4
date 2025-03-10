using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebShoeTest.Models
{
    public class NhanVien
    {
        [Key] // Định nghĩa khóa chính
        public int MaNhanVien { get; set; }

        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string ChucVu { get; set; }

        public ICollection<DonHang> DonHangs { get; set; }
        public ICollection<KhoHang> KhoHangs { get; set; }
    }
}
