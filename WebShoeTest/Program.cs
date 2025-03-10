using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebShoeTest.Models; // Đảm bảo namespace đúng

var builder = WebApplication.CreateBuilder(args);

// Thêm DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication(); // ⚠️ Quan trọng
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();

