using System.ComponentModel.DataAnnotations;

namespace WebShoeTest.Models
{
    public class DanhMuc
    {
        [Key]
        public int MaDanhMuc { get; set; }

        [Required, StringLength(50)]
        public string TenDanhMuc { get; set; }

        [StringLength(50)]
        public string PhongCach { get; set; }

        public ICollection<Giay> Giays { get; set; }
    }


}
