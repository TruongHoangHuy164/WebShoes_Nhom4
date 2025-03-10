using Microsoft.AspNetCore.Identity;

namespace WebShoeTest.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Address { get; set; } = string.Empty; // Đảm bảo cột Address đã được thêm
    }
}
