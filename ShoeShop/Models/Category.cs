using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Models
{
    public class Category
    {
        public int CategoryID { get; set; }

        [Required, StringLength(100)]
        public string CategoryName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
