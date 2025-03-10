namespace WebShoeTest.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class DanhGia
    {
        [Key]
        public int MaDanhGia { get; set; }

        [Required]
        public int MaGiay { get; set; }
        public Giay Giay { get; set; }

        [Required]
        public int MaKhachHang { get; set; }
        public KhachHang KhachHang { get; set; }

        public string NoiDung { get; set; }

        [Range(0, 5)]
        public float SoSao { get; set; }

        public DateTime NgayDanhGia { get; set; }
    }


}
