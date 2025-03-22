using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoeShop.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;

namespace ShoeShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NikeShopDbContext _context;

        public HomeController(ILogger<HomeController> logger, NikeShopDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Where(p => p.ProductSizes.Any(ps => ps.Stock > 0)) // Chỉ hiển thị sản phẩm có size còn hàng
                .Include(p => p.ProductImages) // Load hình ảnh
                .OrderByDescending(p => p.CreatedAt)
                .Take(12)
                .ToListAsync();

            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
