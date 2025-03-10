using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebShoeTest.Models
{
    public class KhachHang
    {
        [Key] // Định nghĩa khóa chính
        public int MaKhachHang { get; set; }

        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string GioiTinh { get; set; }

        public ICollection<DonHang> DonHangs { get; set; }
        public ICollection<DanhGia> DanhGias { get; set; }
    }
}
