
using System.ComponentModel.DataAnnotations;
namespace WebShoeTest.Models
{


    public class KhoHang
    {
        [Key] // Định nghĩa khóa chính
        public int MaKho { get; set; }

        public string ViTri { get; set; }

        public int MaGiay { get; set; }
        public Giay Giay { get; set; }

        public int MaNhaCungCap { get; set; }
        public NhaCungCap NhaCungCap { get; set; }

        public int MaNhanVien { get; set; }
        public NhanVien NhanVien { get; set; }
    }

}
