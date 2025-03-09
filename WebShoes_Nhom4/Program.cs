var builder = WebApplication.CreateBuilder(args);

// Thêm các dịch vụ vào container (DI - Dependency Injection)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Cấu hình pipeline xử lý yêu cầu HTTP
if (!app.Environment.IsDevelopment()) // Nếu không phải môi trường phát triển (production)
{
    app.UseExceptionHandler("/Home/Error"); // Chuyển hướng đến trang lỗi nếu có exception
    app.UseHsts(); // Bật HTTP Strict Transport Security (HSTS) để tăng cường bảo mật
}

// Chuyển hướng HTTP sang HTTPS
app.UseHttpsRedirection();

// Thiết lập định tuyến yêu cầu HTTP
app.UseRouting();

// Xác thực và phân quyền (nếu có sử dụng Authentication & Authorization)
app.UseAuthorization();

// Cho phép sử dụng tài nguyên tĩnh (Static Assets)
app.MapStaticAssets();

// Định nghĩa tuyến đường (route) mặc định: Controller = Home, Action = Index, Id là tùy chọn
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets(); // Kết hợp với static assets nếu có

// Chạy ứng dụng
app.Run();
