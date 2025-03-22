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
        public async Task<IActionResult> Create(Product product, List<int> selectedSizes, List<int> stockQuantities, List<IFormFile> imageFiles)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (product == null)
                {
                    ModelState.AddModelError("", "Dữ liệu sản phẩm không hợp lệ");
                    ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName");
                    ViewData["Sizes"] = _context.Sizes.ToList();
                    return View(product);
                }

                // Kiểm tra các trường bắt buộc
                if (string.IsNullOrEmpty(product.ProductName))
                {
                    ModelState.AddModelError("ProductName", "Tên sản phẩm là bắt buộc");
                }

                if (product.CategoryID <= 0)
                {
                    ModelState.AddModelError("CategoryID", "Vui lòng chọn danh mục");
                }

                if (product.Price <= 0)
                {
                    ModelState.AddModelError("Price", "Giá phải lớn hơn 0");
                }

                if (!ModelState.IsValid)
                {
                    ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryID);
                    ViewData["Sizes"] = _context.Sizes.ToList();
                    return View(product);
                }

                // Set creation date
                product.CreatedAt = DateTime.Now;

                // Add product to database
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Ghi log để debug
                Console.WriteLine($"Đã lưu sản phẩm với ID: {product.ProductID}");

                // Handle product sizes
                if (selectedSizes != null && stockQuantities != null)
                {
                    for (int i = 0; i < Math.Min(selectedSizes.Count, stockQuantities.Count); i++)
                    {
                        if (stockQuantities[i] > 0)
                        {
                            ProductSize productSize = new ProductSize
                            {
                                ProductID = product.ProductID,
                                SizeID = selectedSizes[i],
                                Stock = stockQuantities[i]
                            };
                            _context.ProductSizes.Add(productSize);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // Handle product images
                if (imageFiles != null && imageFiles.Count > 0)
                {
                    foreach (var imageFile in imageFiles)
                    {
                        if (imageFile != null && imageFile.Length > 0)
                        {
                            // Create unique filename
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                            string uploadPath = Path.Combine(_hostEnvironment.WebRootPath, "images", "products");

                            // Create directory if it doesn't exist
                            if (!Directory.Exists(uploadPath))
                            {
                                Directory.CreateDirectory(uploadPath);
                            }

                            string filePath = Path.Combine(uploadPath, fileName);

                            // Save file
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(fileStream);
                            }

                            // Save image info to database
                            ProductImage productImage = new ProductImage
                            {
                                ProductID = product.ProductID,
                                ImageURL = "/images/products/" + fileName,
                                IsPrimary = !_context.ProductImages.Any(pi => pi.ProductID == product.ProductID) // First image is primary
                            };
                            _context.ProductImages.Add(productImage);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                Console.WriteLine($"Tên sản phẩm: {product?.ProductName}");
                Console.WriteLine($"Danh mục ID: {product?.CategoryID}");
                Console.WriteLine($"Giá sản phẩm: {product?.Price}");
                Console.WriteLine($"Số lượng sizes: {selectedSizes?.Count}");
                Console.WriteLine($"Số lượng ảnh: {imageFiles?.Count}");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                Console.WriteLine($"Lỗi khi tạo sản phẩm: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                ModelState.AddModelError("", $"Có lỗi xảy ra: {ex.Message}");
                ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryID);
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
                    ModelState.AddModelError("ProductName", "Tên sản phẩm là bắt buộc");
                }

                if (product.CategoryID <= 0)
                {
                    ModelState.AddModelError("CategoryID", "Vui lòng chọn danh mục");
                }

                if (product.Price <= 0)
                {
                    ModelState.AddModelError("Price", "Giá phải lớn hơn 0");
                }

                if (!ModelState.IsValid || selectedSizes == null || selectedSizes.Count == 0 || imageFiles == null || imageFiles.Count == 0)
                {
                    if (selectedSizes == null || selectedSizes.Count == 0)
                    {
                        ModelState.AddModelError("ProductSizes", "Vui lòng chọn ít nhất một kích cỡ.");
                    }
                    if (imageFiles == null || imageFiles.Count == 0)
                    {
                        ModelState.AddModelError("ProductImages", "Vui lòng tải lên ít nhất một hình ảnh.");
                    }

                    ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryID);
                    ViewData["Sizes"] = _context.Sizes.ToList();
                    return View(product);
                }


                //if (!ModelState.IsValid)
                //{
                //    var existingProduct = await _context.Products
                //        .Include(p => p.ProductSizes)
                //        .ThenInclude(ps => ps.Size)
                //        .Include(p => p.ProductImages)
                //        .FirstOrDefaultAsync(p => p.ProductID == id);

                //    ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryID);
                //    ViewData["Sizes"] = _context.Sizes.ToList();
                //    ViewData["ExistingSizes"] = existingProduct.ProductSizes.Select(ps => ps.SizeID).ToList();
                //    return View(existingProduct);
                //}

                // Get existing product to update
                var productToUpdate = await _context.Products
                    .Include(p => p.ProductSizes)
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.ProductID == id);

                if (productToUpdate == null)
                {
                    return NotFound();
                }

                // Update product properties
                productToUpdate.CategoryID = product.CategoryID;
                productToUpdate.ProductName = product.ProductName;
                productToUpdate.Description = product.Description;
                productToUpdate.Price = product.Price;

                // Cập nhật sản phẩm
                _context.Update(productToUpdate);
                await _context.SaveChangesAsync();

                // Update product sizes
                if (selectedSizes != null && stockQuantities != null)
                {
                    // Remove existing product sizes
                    _context.ProductSizes.RemoveRange(productToUpdate.ProductSizes);
                    await _context.SaveChangesAsync();

                    // Add updated product sizes
                    for (int i = 0; i < Math.Min(selectedSizes.Count, stockQuantities.Count); i++)
                    {
                        if (stockQuantities[i] > 0)
                        {
                            ProductSize productSize = new ProductSize
                            {
                                ProductID = product.ProductID,
                                SizeID = selectedSizes[i],
                                Stock = stockQuantities[i]
                            };
                            _context.ProductSizes.Add(productSize);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // Handle product images
                if (imageFiles != null)
                {
                    foreach (var imageFile in imageFiles)
                    {
                        if (imageFile != null && imageFile.Length > 0)
                        {
                            // Create unique filename
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                            string uploadPath = Path.Combine(_hostEnvironment.WebRootPath, "images", "products");

                            // Create directory if it doesn't exist
                            if (!Directory.Exists(uploadPath))
                            {
                                Directory.CreateDirectory(uploadPath);
                            }

                            string filePath = Path.Combine(uploadPath, fileName);

                            // Save file
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(fileStream);
                            }

                            // Save image info to database
                            ProductImage productImage = new ProductImage
                            {
                                ProductID = product.ProductID,
                                ImageURL = "/images/products/" + fileName,
                                IsPrimary = !productToUpdate.ProductImages.Any() // First image is primary if no images exist
                            };
                            _context.ProductImages.Add(productImage);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // Handle deleted images
                if (deletedImageIds != null && deletedImageIds.Any())
                {
                    foreach (var imageId in deletedImageIds)
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
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // Handle primary image
                if (primaryImageId.HasValue)
                {
                    var images = await _context.ProductImages
                        .Where(pi => pi.ProductID == product.ProductID)
                        .ToListAsync();

                    foreach (var image in images)
                    {
                        image.IsPrimary = (image.ImageID == primaryImageId);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.ProductID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                Console.WriteLine($"Lỗi khi cập nhật sản phẩm: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                ModelState.AddModelError("", $"Có lỗi xảy ra: {ex.Message}");

                var existingProduct = await _context.Products
                    .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.ProductID == id);

                ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryID);
                ViewData["Sizes"] = _context.Sizes.ToList();
                ViewData["ExistingSizes"] = existingProduct.ProductSizes.Select(ps => ps.SizeID).ToList();
                return View(existingProduct);
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductSizes)
                    .FirstOrDefaultAsync(p => p.ProductID == id);

                if (product == null)
                {
                    return NotFound();
                }

                // Delete product images
                foreach (var image in product.ProductImages)
                {
                    // Delete physical file
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath, image.ImageURL.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                // Delete product from database (cascade delete will handle related entities)
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                Console.WriteLine($"Lỗi khi xóa sản phẩm: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                ModelState.AddModelError("", $"Có lỗi xảy ra khi xóa sản phẩm: {ex.Message}");

                var product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
                    .FirstOrDefaultAsync(m => m.ProductID == id);

                return View(product);
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