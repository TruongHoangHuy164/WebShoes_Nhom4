using Microsoft.AspNetCore.Mvc;

namespace WebShoeTest.Controllers
{
    public class AboutUsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
