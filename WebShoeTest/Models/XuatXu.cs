using System.ComponentModel.DataAnnotations;

namespace WebShoeTest.Models
{
    public class XuatXu
    {
        [Key]
        public int MaXuatXu { get; set; }

        [Required, StringLength(50)]
        public string TenXuatXu { get; set; }

        [StringLength(10)]
        public string MaBuuChinh { get; set; }

        [StringLength(100)]
        public string DiaChi { get; set; }

        [EmailAddress, StringLength(50)]
        public string Email { get; set; }

        public ICollection<ThuongHieu> ThuongHieus { get; set; }
        public ICollection<NhaCungCap> NhaCungCaps { get; set; }
    }


}
