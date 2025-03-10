using System.ComponentModel.DataAnnotations;
namespace WebShoeTest.Models
{


    public class ThanhToan
    {
        [Key] // Định nghĩa khóa chính
        public int MaThanhToan { get; set; }

        public int MaDonHang { get; set; }
        public DonHang DonHang { get; set; }

        public string PhuongThucThanhToan { get; set; }
        public decimal SoTien { get; set; }
        public DateTime NgayThanhToan { get; set; }
    }
}