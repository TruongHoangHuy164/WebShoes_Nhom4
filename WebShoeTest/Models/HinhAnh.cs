using System.ComponentModel.DataAnnotations;

namespace WebShoeTest.Models
{
    public class HinhAnh
    {
        [Key]
        public int MaHinhAnh { get; set; }

        [Required]
        public int MaGiay { get; set; }

        [Required, StringLength(255)]
        public string DuongDan { get; set; }

        public Giay Giay { get; set; }
    }


}
