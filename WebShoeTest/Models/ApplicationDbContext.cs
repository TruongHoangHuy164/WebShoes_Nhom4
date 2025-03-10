using Microsoft.EntityFrameworkCore;
using WebShoeTest.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<BaoHanh> BaoHanhs { get; set; }
    public DbSet<ChinhSachHoanTra> ChinhSachHoanTras { get; set; }
    public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
    public DbSet<DanhGia> DanhGias { get; set; }
    public DbSet<DanhMuc> DanhMucs { get; set; }
    public DbSet<DonHang> DonHangs { get; set; }
    public DbSet<Giay> Giays { get; set; }
    public DbSet<HinhAnh> HinhAnhs { get; set; }
    public DbSet<KhachHang> KhachHangs { get; set; }
    public DbSet<KhoHang> KhoHangs { get; set; }
    public DbSet<NhaCungCap> NhaCungCaps { get; set; }
    public DbSet<NhanVien> NhanViens { get; set; }
    public DbSet<ThanhToan> ThanhToans { get; set; }

    public DbSet<XuatXu> XuatXus { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 🔹 Thiết lập khóa chính cho các bảng không tự động nhận diện
        modelBuilder.Entity<BaoHanh>().HasKey(b => b.MaBaoHanh);
        modelBuilder.Entity<ChinhSachHoanTra>().HasKey(c => c.MaHoanTra);
        modelBuilder.Entity<ChiTietDonHang>().HasKey(c => c.MaChiTietDonHang);
        modelBuilder.Entity<DanhGia>().HasKey(d => d.MaDanhGia);
        modelBuilder.Entity<DonHang>().HasKey(d => d.MaDonHang);
        modelBuilder.Entity<HinhAnh>().HasKey(h => h.MaHinhAnh);
        modelBuilder.Entity<KhoHang>().HasKey(k => k.MaKho);

        // 🔹 Thiết lập quan hệ giữa các bảng

        // Bảo hành - Sản phẩm (Giày)
        modelBuilder.Entity<BaoHanh>()
            .HasOne(b => b.Giay)
            .WithMany(g => g.BaoHanhs)
            .HasForeignKey(b => b.MaGiay);

        //// Bảo hành - Đơn hàng
        //modelBuilder.Entity<BaoHanh>()
        //    .HasOne(b => b.DonHang)
        //    .WithMany(d => d.BaoHanhs)
        //    .HasForeignKey(b => b.MaDonHang);

        // Đánh giá - Giày
        modelBuilder.Entity<DanhGia>()
            .HasOne(d => d.Giay)
            .WithMany(g => g.DanhGias)
            .HasForeignKey(d => d.MaGiay);

        // Đánh giá - Khách hàng
        modelBuilder.Entity<DanhGia>()
            .HasOne(d => d.KhachHang)
            .WithMany(k => k.DanhGias)
            .HasForeignKey(d => d.MaKhachHang);

        // Đơn hàng - Khách hàng
        modelBuilder.Entity<DonHang>()
            .HasOne(d => d.KhachHang)
            .WithMany(k => k.DonHangs)
            .HasForeignKey(d => d.MaKhachHang);

        // Đơn hàng - Nhân viên
        modelBuilder.Entity<DonHang>()
            .HasOne(d => d.NhanVien)
            .WithMany(n => n.DonHangs)
            .HasForeignKey(d => d.MaNhanVien);

        // Chi tiết đơn hàng - Đơn hàng
        modelBuilder.Entity<ChiTietDonHang>()
            .HasOne(c => c.DonHang)
            .WithMany(d => d.ChiTietDonHangs)
            .HasForeignKey(c => c.MaDonHang);

        // Chi tiết đơn hàng - Giày
        modelBuilder.Entity<ChiTietDonHang>()
            .HasOne(c => c.Giay)
            .WithMany(g => g.ChiTietDonHangs)
            .HasForeignKey(c => c.MaGiay);

        // Thanh toán - Đơn hàng
        modelBuilder.Entity<ThanhToan>()
            .HasOne(t => t.DonHang)
            .WithOne(d => d.ThanhToan)
            .HasForeignKey<ThanhToan>(t => t.MaDonHang);

     

        // Giày - Danh mục
        modelBuilder.Entity<Giay>()
            .HasOne(g => g.DanhMuc)
            .WithMany(d => d.Giays)
            .HasForeignKey(g => g.MaDanhMuc);

        // Hình ảnh - Giày
        modelBuilder.Entity<HinhAnh>()
            .HasOne(h => h.Giay)
            .WithMany(g => g.HinhAnhs)
            .HasForeignKey(h => h.MaGiay);

        //// Kho hàng - Giày
        //modelBuilder.Entity<KhoHang>()
        //    .HasOne(k => k.Giay)
        //    .WithMany(g => g.KhoHangs)
        //    .HasForeignKey(k => k.MaGiay);

        // Kho hàng - Nhà cung cấp
        modelBuilder.Entity<KhoHang>()
            .HasOne(k => k.NhaCungCap)
            .WithMany(n => n.KhoHangs)
            .HasForeignKey(k => k.MaNhaCungCap);

        // Kho hàng - Nhân viên
        modelBuilder.Entity<KhoHang>()
            .HasOne(k => k.NhanVien)
            .WithMany(n => n.KhoHangs)
            .HasForeignKey(k => k.MaNhanVien);

        modelBuilder.Entity<KhoHang>()
    .HasOne(k => k.NhaCungCap)
    .WithMany(n => n.KhoHangs)
    .HasForeignKey(k => k.MaNhaCungCap)
    .OnDelete(DeleteBehavior.NoAction); // 🔹 Thêm dòng này để tránh lỗi

        modelBuilder.Entity<KhoHang>()
            .HasOne(k => k.Giay)
            .WithMany(g => g.KhoHangs)
            .HasForeignKey(k => k.MaGiay)
            .OnDelete(DeleteBehavior.NoAction); // 🔹 Tránh xóa dây chuyền

        //// Chính sách hoàn trả - Đơn hàng
        //modelBuilder.Entity<ChinhSachHoanTra>()
        //    .HasOne(c => c.DonHang)
        //    .WithMany(d => d.ChinhSachHoanTras)
        //    .HasForeignKey(c => c.MaDonHang);

        //// Chính sách hoàn trả - Giày
        //modelBuilder.Entity<ChinhSachHoanTra>()
        //    .HasOne(c => c.Giay)
        //    .WithMany(g => g.ChinhSachHoanTras)
        //    .HasForeignKey(c => c.MaGiay);

        // Nhà cung cấp - Xuất xứ
        modelBuilder.Entity<NhaCungCap>()
            .HasOne(n => n.XuatXu)
            .WithMany(x => x.NhaCungCaps)
            .HasForeignKey(n => n.MaXuatXu);

   
    }
}
