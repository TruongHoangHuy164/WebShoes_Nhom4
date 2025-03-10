using System.ComponentModel.DataAnnotations;

namespace WebShoeTest.Models
{
    public class ChinhSachHoanTra
    {
        [Key]
        public int MaHoanTra { get; set; }

        [Required]
        public int MaDonHang { get; set; }
        public DonHang DonHang { get; set; }

        [Required]
        public int MaGiay { get; set; }
        public Giay Giay { get; set; }

        [Required]
        public DateTime NgayTra { get; set; }

        [Required]
        public DateTime NgayHoanTra { get; set; }

        [Required]
        public string LyDo { get; set; }
    }

}
