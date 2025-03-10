using System.ComponentModel.DataAnnotations;

namespace WebShoeTest.Models
{
    public class BaoHanh
    {
        [Key]
        public int MaBaoHanh { get; set; }

        [Required]
        public int MaGiay { get; set; }

        [Required]
        public int MaDonHang { get; set; }

        [Required]
        public int ThoiGianBaoHanh { get; set; }

        public DateTime NgayKetThuc { get; set; }

        public Giay Giay { get; set; }
        public DonHang DonHang { get; set; }
    }

}
