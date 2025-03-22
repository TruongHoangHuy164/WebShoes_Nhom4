using System.ComponentModel.DataAnnotations;
using ShoeShop.Models;

public class OrderDetail
{
    [Key]  // Định nghĩa khóa chính
    public int DetailID { get; set; }

    [Required]
    public int OrderID { get; set; }
    public Order Order { get; set; }

    [Required]
    public int ProductID { get; set; }
    public Product Product { get; set; }

    [Required]
    public int SizeID { get; set; }
    public Size Size { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public decimal Price { get; set; }
}
