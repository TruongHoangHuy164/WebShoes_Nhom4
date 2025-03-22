using Microsoft.AspNetCore.Mvc;

namespace WebShoes_Nhom4.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
