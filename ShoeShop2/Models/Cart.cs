using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Models
{
    public class Cart
    {
        public int CartID { get; set; }

        [Required]
        public int UserID { get; set; }
        public User User { get; set; }

        [Required]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [Required]
        public int SizeID { get; set; }
        public Size Size { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
