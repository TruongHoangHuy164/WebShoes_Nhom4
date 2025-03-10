using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace WebShoeTest.Controllers
{
    public class DetailProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DetailProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _context.Giays
                .Include(g => g.HinhAnhs) // Lấy danh sách hình ảnh
                .Select(g => new
                {
                    g.TenMau,
                    g.Gia,
                })
                .ToList();

            return Json(products);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
