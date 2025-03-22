using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ShoeShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly NikeShopDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(NikeShopDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .ToListAsync();
            return View(products);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSizes)
                .ThenInclude(ps => ps.Size)
                .FirstOrDefaultAsync(m => m.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(_context.Categories, "CategoryID", "CategoryName");
            //ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName");
            ViewData["Sizes"] = _context.Sizes.ToList();
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, List<int> selectedSizes, List<int> stockQuantities, List<IFormFile> imageFiles, int mainImageIndex)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName");
                    ViewData["Sizes"] = _context.Sizes.ToList();
                    return View(product);
                }

                product.CreatedAt = DateTime.Now;
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Xử lý kích cỡ và số lượng
                if (selectedSizes != null && stockQuantities != null && selectedSizes.Count == stockQuantities.Count)
                {
                    for (int i = 0; i < selectedSizes.Count; i++)
                    {
                        if (stockQuantities[i] > 0)
                        {
                            _context.ProductSizes.Add(new ProductSize
                            {
                                ProductID = product.ProductID,
                                SizeID = selectedSizes[i],
                                Stock = stockQuantities[i]
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // Xử lý hình ảnh sản phẩm
                if (imageFiles != null && imageFiles.Any())
                {
                    string uploadPath = Path.Combine(_hostEnvironment.WebRootPath, "images/products");

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    for (int i = 0; i < imageFiles.Count; i++)
                    {
                        var imageFile = imageFiles[i];

                        if (imageFile.Length > 0)
                        {
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                            string filePath = Path.Combine(uploadPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(stream);
                            }

                            _context.ProductImages.Add(new ProductImage
                            {
                                ProductID = product.ProductID,
                                ImageURL = "/images/products/" + fileName,
                                IsPrimary = (i == mainImageIndex) // Đánh dấu ảnh chính theo index được chọn
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName");
                ViewData["Sizes"] = _context.Sizes.ToList();
                return View(product);
            }
        }


        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductSizes)
                .ThenInclude(ps => ps.Size)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryID);
            ViewData["Sizes"] = _context.Sizes.ToList();
            ViewData["ExistingSizes"] = product.ProductSizes.Select(ps => ps.SizeID).ToList();
            return View(product);
        }



        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, List<int> selectedSizes, List<int> stockQuantities,
            List<IFormFile> imageFiles, List<int> deletedImageIds, int? primaryImageId)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            try
            {
                // Kiểm tra các trường bắt buộc
                if (string.IsNullOrEmpty(product.ProductName))
                {
                    ModelState.AddModelError("ProductName", "Tên sản phẩm là bắt buộc.");
                }

                if (product.CategoryID <= 0)
                {
                    ModelState.AddModelError("CategoryID", "Vui lòng chọn danh mục.");
                }

                if (product.Price <= 0)
                {
                    ModelState.AddModelError("Price", "Giá phải lớn hơn 0.");
                }

                // Lấy thông tin sản phẩm cần cập nhật
                var productToUpdate = await _context.Products
                    .Include(p => p.ProductSizes)
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.ProductID == id);

                if (productToUpdate == null)
                {
                    return NotFound();
                }

                // Xử lý xóa ảnh
                if (deletedImageIds != null && deletedImageIds.Any())
                {
                    foreach (var imageId in deletedImageIds)
                    {
                        var image = await _context.ProductImages.FindAsync(imageId);
                        if (image != null)
                        {
                            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, image.ImageURL.TrimStart('/'));
                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }

                            _context.ProductImages.Remove(image);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // Kiểm tra xem còn ảnh nào không
                var existingImages = await _context.ProductImages
                    .Where(pi => pi.ProductID == product.ProductID)
                    .ToListAsync();

                if (!existingImages.Any() && (imageFiles == null || !imageFiles.Any()))
                {
                    ModelState.AddModelError("ProductImages", "Vui lòng tải lên ít nhất một hình ảnh.");
                    ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryID);
                    ViewData["Sizes"] = _context.Sizes.ToList();
                    return View(product);
                }

                // Cập nhật thông tin sản phẩm
                productToUpdate.CategoryID = product.CategoryID;
                productToUpdate.ProductName = product.ProductName;
                productToUpdate.Description = product.Description;
                productToUpdate.Price = product.Price;

                // Cập nhật kích thước sản phẩm
                if (selectedSizes != null && stockQuantities != null)
                {
                    _context.ProductSizes.RemoveRange(productToUpdate.ProductSizes);
                    await _context.SaveChangesAsync();

                    for (int i = 0; i < Math.Min(selectedSizes.Count, stockQuantities.Count); i++)
                    {
                        if (stockQuantities[i] > 0)
                        {
                            _context.ProductSizes.Add(new ProductSize
                            {
                                ProductID = product.ProductID,
                                SizeID = selectedSizes[i],
                                Stock = stockQuantities[i]
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // Xử lý thêm ảnh mới
                if (imageFiles != null && imageFiles.Any())
                {
                    foreach (var imageFile in imageFiles)
                    {
                        if (imageFile.Length > 0)
                        {
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                            string uploadPath = Path.Combine(_hostEnvironment.WebRootPath, "images", "products");

                            if (!Directory.Exists(uploadPath))
                            {
                                Directory.CreateDirectory(uploadPath);
                            }

                            string filePath = Path.Combine(uploadPath, fileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(fileStream);
                            }

                            _context.ProductImages.Add(new ProductImage
                            {
                                ProductID = product.ProductID,
                                ImageURL = "/images/products/" + fileName,
                                IsPrimary = !existingImages.Any() // Nếu chưa có ảnh thì ảnh đầu tiên là chính
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // Cập nhật ảnh chính
                if (primaryImageId.HasValue)
                {
                    foreach (var img in existingImages)
                    {
                        img.IsPrimary = (img.ImageID == primaryImageId.Value);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật sản phẩm: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Chi tiết lỗi: {ex.InnerException.Message}");
                }

                ModelState.AddModelError("", $"Có lỗi xảy ra: {ex.Message}");

                ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryID);
                ViewData["Sizes"] = _context.Sizes.ToList();
                return View(product);
            }
        }


        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSizes)
                .ThenInclude(ps => ps.Size)
                .FirstOrDefaultAsync(m => m.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        // POST: Product/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null) return NotFound();

            try
            {
                // Xóa ảnh sản phẩm khỏi thư mục
                foreach (var image in product.ProductImages)
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath, image.ImageURL.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                // Xóa sản phẩm khỏi database
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index"); // Điều hướng về danh sách sản phẩm
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa sản phẩm: {ex.Message}");
                ModelState.AddModelError("", "Không thể xóa sản phẩm. Vui lòng thử lại.");
                return View("Delete", product); // Nếu lỗi, quay lại trang xóa
            }
        }



        // POST: Product/SetPrimaryImage/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPrimaryImage(int imageId, int productId)
        {
            try
            {
                var images = await _context.ProductImages
                    .Where(pi => pi.ProductID == productId)
                    .ToListAsync();

                foreach (var image in images)
                {
                    image.IsPrimary = (image.ImageID == imageId);
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Edit), new { id = productId });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                Console.WriteLine($"Lỗi khi đặt ảnh chính: {ex.Message}");
                TempData["ErrorMessage"] = $"Có lỗi xảy ra: {ex.Message}";
                return RedirectToAction(nameof(Edit), new { id = productId });
            }
        }

        // POST: Product/DeleteImage/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int imageId, int productId)
        {
            try
            {
                var image = await _context.ProductImages.FindAsync(imageId);
                if (image != null)
                {
                    // Delete physical file
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath, image.ImageURL.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    // Remove from database
                    _context.ProductImages.Remove(image);
                    await _context.SaveChangesAsync();

                    // If this was the primary image, set another image as primary
                    if (image.IsPrimary)
                    {
                        var newPrimaryImage = await _context.ProductImages
                            .FirstOrDefaultAsync(pi => pi.ProductID == productId);
                        if (newPrimaryImage != null)
                        {
                            newPrimaryImage.IsPrimary = true;
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                return RedirectToAction(nameof(Edit), new { id = productId });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                Console.WriteLine($"Lỗi khi xóa ảnh: {ex.Message}");
                TempData["ErrorMessage"] = $"Có lỗi xảy ra: {ex.Message}";
                return RedirectToAction(nameof(Edit), new { id = productId });
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }
    }
}