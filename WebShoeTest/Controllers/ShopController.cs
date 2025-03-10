using Microsoft.AspNetCore.Mvc;

namespace WebShoeTest.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
