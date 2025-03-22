using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeShop.Models
{
    public class ProductImage
    {
        [Key] // Xác định đây là khóa chính
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Tự động tăng
        public int ImageID { get; set; }

        [Required]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [Required, StringLength(500)]
        public string ImageURL { get; set; }

        public bool IsPrimary { get; set; } = false;
    }
}
