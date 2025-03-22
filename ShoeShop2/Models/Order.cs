using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Models
{
    public class Order
    {
        public int OrderID { get; set; }

        [Required]
        public int UserID { get; set; }
        public User User { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
