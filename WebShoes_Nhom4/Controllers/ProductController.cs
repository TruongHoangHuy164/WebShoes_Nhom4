using Microsoft.AspNetCore.Mvc;

namespace WebShoes_Nhom4.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Product()
        {
            return View();
        }
    }
}
