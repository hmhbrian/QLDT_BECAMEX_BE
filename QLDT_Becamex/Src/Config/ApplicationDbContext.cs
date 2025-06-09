// Src/Config/ApplicationDbContext.cs (hoặc Data/ApplicationDbContext.cs)
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Models;


namespace QLDT_Becamex.Src.Config
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Tùy chỉnh schema hoặc tên bảng nếu cần
        }
    }
}