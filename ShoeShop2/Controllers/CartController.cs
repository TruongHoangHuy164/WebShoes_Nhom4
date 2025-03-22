using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Models;
using System.Linq;

namespace ShoeShop.Controllers
{
    public class CartController : Controller
    {
        private readonly NikeShopDbContext _context;

        public CartController(NikeShopDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cartItems = _context.Carts.Include(c => c.Product).ToList();
            return View(cartItems);
        }

        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = _context.Products.Find(productId);
            if (product == null) return NotFound();

            var cartItem = _context.Carts.FirstOrDefault(c => c.ProductID == productId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                _context.Carts.Add(new Cart { ProductID = productId, Quantity = quantity });
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int id)
        {
            var cartItem = _context.Carts.Find(id);
            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
