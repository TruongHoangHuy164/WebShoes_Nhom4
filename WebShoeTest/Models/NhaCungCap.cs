using System.ComponentModel.DataAnnotations;

namespace WebShoeTest.Models
{
    public class NhaCungCap
    {
        [Key]
        public int MaNhaCungCap { get; set; }

        [Required, StringLength(50)]
        public string TenNhaCungCap { get; set; }

        [Required, StringLength(100)]
        public string DiaChi { get; set; }

        [EmailAddress, StringLength(50)]
        public string Email { get; set; }

        [StringLength(15)]
        public string SoDienThoai { get; set; }

        public int MaXuatXu { get; set; }
        public XuatXu XuatXu { get; set; }

        public ICollection<KhoHang> KhoHangs { get; set; }
    }


}
