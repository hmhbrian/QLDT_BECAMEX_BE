using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Models; // Đảm bảo namespace này khớp với nơi bạn định nghĩa model

namespace QLDT_Becamex.Src.Config // Ví dụ: bạn có thể đặt nó trong thư mục Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Định nghĩa các DbSet cho các Model của bạn
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        // DbSet cho ApplicationUser đã được kế thừa từ IdentityDbContext

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // LUÔN LUÔN gọi phương thức OnModelCreating của lớp cơ sở cho IdentityDbContext
            base.OnModelCreating(modelBuilder);

            // --- Cấu hình Fluent API ở đây ---
            // Gọi các phương thức cấu hình riêng biệt để giữ cho OnModelCreating gọn gàng và dễ đọc
            ConfigureApplicationUser(modelBuilder);
            ConfigureDepartment(modelBuilder);
            ConfigurePosition(modelBuilder);
        }

        private void ConfigureApplicationUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                // Cấu hình bảng và tên cột nếu cần (mặc định IdentityDbContext sẽ tự xử lý)
                // entity.ToTable("Users");

                // Cấu hình thuộc tính
                entity.Property(u => u.FullName)
                      .HasMaxLength(255); // Giới hạn độ dài tối đa 255 ký tự

                entity.Property(u => u.UrlAvatar)
                      .HasMaxLength(500); // Giới hạn độ dài URL avatar

                entity.Property(u => u.IdCard)
                      .HasMaxLength(20); // Giới hạn độ dài ID Card (ví dụ: CCCD)

                entity.Property(u => u.Status)
                      .HasMaxLength(50); // Giới hạn độ dài trạng thái

                // Cấu hình mối quan hệ khóa ngoại với Department
                entity.HasOne(u => u.Department)      // Một ApplicationUser có MỘT Department
                      .WithMany(d => d.Users)         // Một Department có NHIỀU ApplicationUser
                      .HasForeignKey(u => u.DepartmentId) // Khóa ngoại là DepartmentId
                      .IsRequired(false)              // DepartmentId có thể là NULL (tức là không bắt buộc User phải thuộc phòng ban)
                      .OnDelete(DeleteBehavior.SetNull); // Nếu một Department bị xóa, DepartmentId của các User liên quan sẽ được đặt thành NULL

                // Cấu hình mối quan hệ khóa ngoại với Position
                entity.HasOne(u => u.Position)        // Một ApplicationUser có MỘT Position
                      .WithMany(p => p.Users)         // Một Position có NHIỀU ApplicationUser
                      .HasForeignKey(u => u.PositionId)   // Khóa ngoại là PositionId
                      .IsRequired(false)              // PositionId có thể là NULL (tức là không bắt buộc User phải có vị trí)
                      .OnDelete(DeleteBehavior.SetNull); // Nếu một Position bị xóa, PositionId của các User liên quan sẽ được đặt thành NULL

                // Cấu hình mối quan hệ tự tham chiếu cho ManagerId (Một người quản lý nhiều người)
                entity.HasOne<ApplicationUser>()      // Một ApplicationUser có thể có một Manager (cũng là ApplicationUser)
                      .WithMany()                     // Một Manager có thể quản lý nhiều người (không cần navigation property ngược lại cụ thể ở đây)
                      .HasForeignKey(u => u.ManagerId)
                      .IsRequired(false)              // ManagerId có thể là NULL (ví dụ: người đứng đầu công ty không có manager)
                      .OnDelete(DeleteBehavior.Restrict); // NGĂN CHẶN xóa một User nếu họ đang là Manager của User khác.
                                                          // Điều này giúp bảo vệ dữ liệu khỏi việc xóa nhầm Manager.
            });
        }

        private void ConfigureDepartment(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(entity =>
            {
                // Định nghĩa khóa chính
                entity.HasKey(d => d.DepartmentId);
                // Vì DepartmentId là string, EF Core sẽ không tự động tạo giá trị Identity.
                // Nếu bạn muốn nó được tạo tự động dưới dạng GUID, bạn có thể thêm:
                // entity.Property(d => d.DepartmentId).ValueGeneratedOnAdd();

                // Cấu hình thuộc tính DepartmentName
                entity.Property(d => d.DepartmentName)
                      .IsRequired()               // Bắt buộc phải có giá trị (không NULL)
                      .HasMaxLength(255);         // Giới hạn độ dài tối đa 255 ký tự

                // Cấu hình mối quan hệ tự tham chiếu (Parent Department -> Children Departments)
                entity.HasOne(d => d.Parent)      // Một Department có MỘT Parent Department
                      .WithMany(d => d.Children)  // Một Parent Department có NHIỀU Children Departments
                                                  // (Bạn sẽ cần thêm 'public ICollection<Department>? Children { get; set; }' vào model Department)
                      .HasForeignKey(d => d.ParentId) // Khóa ngoại là ParentId
                      .IsRequired(false)              // ParentId có thể là NULL (cho các phòng ban gốc)
                      .OnDelete(DeleteBehavior.Restrict); // NGĂN CHẶN xóa một Department nếu nó có các Department con.
                                                          // Điều này đảm bảo cấu trúc cây phòng ban không bị phá vỡ.

                // Cấu hình thuộc tính Description
                entity.Property(d => d.Description)
                      .HasMaxLength(1000); // Giới hạn độ dài cho Description
            });
        }

        private void ConfigurePosition(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Position>(entity =>
            {
                // Định nghĩa khóa chính
                entity.HasKey(p => p.PositionId);

                entity.HasOne(p => p.Role)
                      .WithMany()
                      .HasForeignKey(p => p.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(p => p.PositionName)
                      .IsRequired()               // Bắt buộc phải có giá trị
                      .HasMaxLength(255);         // Giới hạn độ 
            });
        }
    }
}