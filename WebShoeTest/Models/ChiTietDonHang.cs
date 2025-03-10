using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebShoeTest.Models
{
    public class ChiTietDonHang
    {
        [Key]
        public int MaChiTietDonHang { get; set; }

        public int MaDonHang { get; set; }
        public DonHang DonHang { get; set; }

        public int MaGiay { get; set; }
        public Giay Giay { get; set; }

        [Required]
        public int SoLuong { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Gia { get; set; }
    }


}
