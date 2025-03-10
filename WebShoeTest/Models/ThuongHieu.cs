using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebShoeTest.Models
{
    public class ThuongHieu
    {
        [Key]
        public int MaThuongHieu { get; set; }

        [Required, StringLength(50)]
        public string TenThuongHieu { get; set; }

        public int MaXuatXu { get; set; }
        [ForeignKey("MaXuatXu")]
        public XuatXu XuatXu { get; set; }

        public ICollection<Giay> Giays { get; set; }
    }

}
