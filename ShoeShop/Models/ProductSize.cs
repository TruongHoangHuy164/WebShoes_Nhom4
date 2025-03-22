using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Models
{
    public class ProductSize
    {
        public int ProductSizeID { get; set; }

        [Required]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [Required]
        public int SizeID { get; set; }
        public Size Size { get; set; }

        [Required]
        public int Stock { get; set; }
    }
}
