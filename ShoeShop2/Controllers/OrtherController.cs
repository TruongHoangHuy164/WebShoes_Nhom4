using Microsoft.AspNetCore.Mvc;
using ShoeShop.Models;
using System.Linq;

namespace ShoeShop.Controllers
{
    public class OrderController : Controller
    {
        private readonly NikeShopDbContext _context;

        public OrderController(NikeShopDbContext context)
        {
            _context = context;
        }

        public IActionResult Checkout()
        {
            var cartItems = _context.Carts.ToList();
            if (!cartItems.Any()) return RedirectToAction("Index", "Cart");

            var order = new Order { OrderDate = DateTime.Now, Status = "Pending" };
            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var item in cartItems)
            {
                _context.OrderDetails.Add(new OrderDetail
                {
                    OrderID = order.OrderID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                });
            }

            _context.Carts.RemoveRange(cartItems);
            _context.SaveChanges();

            return RedirectToAction("OrderSuccess");
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }
    }
}
