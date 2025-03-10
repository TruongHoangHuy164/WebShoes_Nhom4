using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebShoeTest.Models
{
    public class Giay
    {
        [Key]
        public int MaGiay { get; set; }

        [Required, StringLength(50)]
        public string TenMau { get; set; }

        [StringLength(20)]
        public string MauSac { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Gia { get; set; }

        [Required]
        public int KichThuoc { get; set; }

        [Required]
        public int SoLuong { get; set; }

        [Range(0, 5)]
        public float DanhGia { get; set; } = 0;

        public int MaThuongHieu { get; set; }
        public ThuongHieu ThuongHieu { get; set; }

        public int MaDanhMuc { get; set; }
        public DanhMuc DanhMuc { get; set; }

        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public ICollection<HinhAnh> HinhAnhs { get; set; }
        public ICollection<BaoHanh> BaoHanhs { get; set; }
        public ICollection<DanhGia> DanhGias { get; set; }

        public ICollection<KhoHang> KhoHangs { get; set; }
    }


}
