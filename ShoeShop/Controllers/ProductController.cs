using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Models;
using System.Linq;
using Newtonsoft.Json;


namespace ShoeShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly NikeShopDbContext _context;

        public ProductController(NikeShopDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách sản phẩm
        public IActionResult Index()
        {
            var products = _context.Products.Include(p => p.Category).ToList();
            return View(products);
        }

        // Xem chi tiết sản phẩm
        public IActionResult Details(int id)
        {
            var product = _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSizes)
                .ThenInclude(ps => ps.Size)
                .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Hiển thị form thêm sản phẩm
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryID", "CategoryName");
            ViewBag.Sizes = new SelectList(_context.Sizes, "SizeID", "SizeValue");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFileCollection images, string ProductSizesJson)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Lưu ảnh
                if (images != null && images.Count > 0)
                {
                    foreach (var image in images.Take(3)) // Tối đa 3 ảnh
                    {
                        var imagePath = "/uploads/" + Path.GetFileName(image.FileName);
                        using (var stream = new FileStream("wwwroot" + imagePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        _context.ProductImages.Add(new ProductImage
                        {
                            ProductID = product.ProductID,
                            ImageURL = imagePath,
                            IsPrimary = false
                        });
                    }
                }

                // Lưu size giày & số lượng tồn kho
                if (!string.IsNullOrEmpty(ProductSizesJson))
                {
                    var sizes = JsonConvert.DeserializeObject<List<ProductSize>>(ProductSizesJson);
                    foreach (var size in sizes)
                    {
                        size.ProductID = product.ProductID;
                        _context.ProductSizes.Add(size);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.Categories, "CategoryID", "CategoryName");
            ViewBag.Sizes = new SelectList(_context.Sizes, "SizeID", "SizeName"); // <-- Đảm bảo có danh sách Sizes
            return View(product);
        }


        // GET: Hiển thị form chỉnh sửa sản phẩm
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        // POST: Cập nhật sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Product product)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        // GET: Xác nhận xóa sản phẩm
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Xóa sản phẩm
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
