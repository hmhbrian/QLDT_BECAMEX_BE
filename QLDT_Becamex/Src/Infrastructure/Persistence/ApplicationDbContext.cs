using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Shared.Helpers;
using System.Security.Claims;

namespace QLDT_Becamex.Src.Infrastructure.Persistence // Ví dụ: bạn có thể đặt nó trong thư mục Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
        : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Định nghĩa các DbSet cho các Model của bạn
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<UserStatus> UserStatus { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<CourseStatus> CourseStatus { get; set; }
        public DbSet<CourseDepartment> CourseDepartment { get; set; }
        public DbSet<CoursePosition> CoursePosition { get; set; }
        public DbSet<CourseAttachedFile> CourseAttachedFile { get; set; }

        public DbSet<UserCourse> UserCourse { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<CourseCategory> CourseCategories { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<LessonProgress> LessonProgresses { get; set; }
        public DbSet<TypeDocument> TypeDocuments { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }



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
            ConfigureUserStatus(modelBuilder);
            ConfigureCourse(modelBuilder);
            ConfigureCourseStatus(modelBuilder);
            ConfigureCourseDepartment(modelBuilder);
            ConfigureCoursePosition(modelBuilder);
            ConfigureCourseAttachedFile(modelBuilder);
            ConfigureUserCourse(modelBuilder);
            ConfigureCourseCategory(modelBuilder);
            ConfigureLecturer(modelBuilder);
            ConfigureLesson(modelBuilder);
            ConfigureTest(modelBuilder);
            ConfigureQuestion(modelBuilder);
            ConfigureTypeDocument(modelBuilder);
            ConfigureLessonProgress(modelBuilder);
            ConfigureFeedback(modelBuilder);
            ConfigureDepartmentStatus(modelBuilder);
            ConfigureAuditLog(modelBuilder);
        }

        private void ConfigureApplicationUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FullName)
                      .HasColumnName("full_name")
                      .HasMaxLength(255);

                entity.Property(u => u.UrlAvatar)
                      .HasColumnName("url_avatar")
                      .HasMaxLength(500);

                entity.Property(u => u.IdCard)
                      .HasColumnName("id_card")
                      .HasMaxLength(100);

                entity.Property(u => u.Code)
                      .HasColumnName("code");

                entity.Property(u => u.StartWork)
                      .HasColumnName("start_work");

                entity.Property(u => u.EndWork)
                      .HasColumnName("end_work");

                entity.Property(u => u.StatusId)
                      .HasColumnName("status_id");

                entity.Property(u => u.CreatedAt)
                      .HasColumnName("created_at");

                entity.Property(u => u.ModifiedAt)
                      .HasColumnName("modified_at");

                entity.Property(u => u.IsDeleted)
                      .HasColumnName("is_deleted");

                entity.Property(u => u.ManagerUId)
                      .HasColumnName("manager_u_id");

                entity.Property(u => u.DepartmentId)
                      .HasColumnName("department_id");

                entity.Property(u => u.PositionId)
                      .HasColumnName("position_id");

                entity.Property(p => p.CreateById)
                   .HasColumnName("created_by_id")
                   .HasMaxLength(450);
                entity.Property(p => p.UpdateById)
                   .HasColumnName("update_by_id")
                   .HasMaxLength(450);

                // 🔗 Relationships

                entity.HasOne(u => u.Department)
                      .WithMany(d => d.Users)
                      .HasForeignKey(u => u.DepartmentId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(u => u.Position)
                      .WithMany(p => p.Users)
                      .HasForeignKey(u => u.PositionId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(u => u.ManagerU)
                      .WithMany(p => p.Children)
                      .HasForeignKey(u => u.ManagerUId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(u => u.CreateBy)
                     .WithMany()
                     .HasForeignKey(u => u.CreateById)
                     .IsRequired(false)
                     .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(u => u.UpdateBy)
                     .WithMany()
                     .HasForeignKey(u => u.UpdateById)
                     .IsRequired(false)
                     .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(u => u.UserStatus)
                      .WithMany()
                      .HasForeignKey(u => u.StatusId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);
            });

        }

        private void ConfigureDepartment(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(entity =>
            {
                // Định nghĩa khóa chính
                entity.HasKey(d => d.DepartmentId);
                entity.Property(d => d.DepartmentId)
                      .HasColumnName("department_id")
                      .ValueGeneratedOnAdd();

                // Cấu hình thuộc tính DepartmentName
                entity.Property(d => d.DepartmentName)
                      .HasColumnName("department_name")
                      .IsRequired()               // Bắt buộc phải có giá trị (không NULL)
                      .HasMaxLength(255);         // Giới hạn độ dài tối đa 255 ký tự

                entity.Property(d => d.DepartmentCode)
                      .HasColumnName("department_code")
                      .IsRequired()               // Bắt buộc phải có giá trị (không NULL)
                      .HasMaxLength(255);

                // Cấu hình mối quan hệ tự tham chiếu (Parent Department -> Children Departments)
                entity.HasOne(d => d.Parent)      // Một Department có MỘT Parent Department
                      .WithMany(d => d.Children)  // Một Parent Department có NHIỀU Children Departments
                                                  // (Bạn sẽ cần thêm 'public ICollection<Department>? Children { get; set; }' vào model Department)
                      .HasForeignKey(d => d.ParentId) // Khóa ngoại là ParentId
                      .IsRequired(false)              // ParentId có thể là NULL (cho các phòng ban gốc)
                      .OnDelete(DeleteBehavior.Restrict); // NGĂN CHẶN xóa một Department nếu nó có các Department con.
                                                          // Điều này đảm bảo cấu trúc cây phòng ban không bị phá vỡ.
                entity.HasOne(d => d.Manager)      // Một Department có 1 quản lý
                      .WithOne()  // 1 qly quản lý 1 department
                      .HasForeignKey<Department>(d => d.ManagerId) // Khóa ngoại là ManagerID
                      .IsRequired(false)              // ParentId có thể là NULL (cho các phòng ban gốc)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(d => d.ManagerId).HasColumnName("manager_id").IsRequired(false);

                entity.Property(d => d.Level)
                      .HasColumnName("level");

                // Cấu hình thuộc tính Description
                entity.Property(d => d.Description)
                      .HasColumnName("description")
                      .HasMaxLength(1000); // Giới hạn độ dài cho Description

                entity.Property(d => d.StatusId)
                      .HasColumnName("status_id");

                entity.Property(d => d.CreatedAt)
                      .HasColumnName("create_at");

                entity.Property(d => d.UpdatedAt)
                      .HasColumnName("update_at");

                entity.HasOne(u => u.Status)
                      .WithMany()
                      .HasForeignKey(u => u.StatusId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }

        private void ConfigureDepartmentStatus(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DepartmentStatus>(entity =>
            {
                entity.ToTable("DepartmentStatus"); // 👉 Đặt tên bảng ở đây
                                                    // Định nghĩa khóa chính
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id)
                      .HasColumnName("id")
                      .ValueGeneratedOnAdd();

                entity.Property(p => p.Name)
                      .HasColumnName("name")
                      .IsRequired()               // Bắt buộc phải có giá trị
                      .HasMaxLength(255);         // Giới hạn độ 
            });
        }

        private void ConfigurePosition(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Position>(entity =>
            {
                // Định nghĩa khóa chính
                entity.HasKey(p => p.PositionId);
                entity.Property(p => p.PositionId)
                      .HasColumnName("position_id")
                      .ValueGeneratedOnAdd();

                entity.Property(p => p.PositionName)
                      .HasColumnName("position_name")
                      .IsRequired()               // Bắt buộc phải có giá trị
                      .HasMaxLength(255);         // Giới hạn độ 
            });
        }

        private void ConfigureUserStatus(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserStatus>(entity =>
            {
                entity.ToTable("UserStatus"); // 👉 Đặt tên bảng ở đây
                                              // Định nghĩa khóa chính
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id)
                      .HasColumnName("id")
                      .ValueGeneratedOnAdd();

                entity.Property(p => p.Name)
                      .HasColumnName("name")
                      .IsRequired()               // Bắt buộc phải có giá trị
                      .HasMaxLength(255);         // Giới hạn độ 
            });
        }

        private void ConfigureCourse(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Courses"); // snake_case

                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).HasColumnName("id");

                entity.Property(p => p.Code)
                      .HasColumnName("code")
                      .HasMaxLength(100);

                entity.Property(p => p.Name)
                      .HasColumnName("name")
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(p => p.Description)
                      .HasColumnName("description")
                      .HasMaxLength(1000);

                entity.Property(p => p.ThumbUrl)
                      .HasColumnName("thumb_url")
                      .HasMaxLength(500);

                entity.Property(p => p.Objectives)
                      .HasColumnName("objectives")
                      .HasMaxLength(1000);

                entity.Property(p => p.Format)
                      .HasColumnName("format")
                      .HasMaxLength(50);

                entity.Property(p => p.Sessions)
                      .HasColumnName("sessions");

                entity.Property(p => p.HoursPerSessions)
                      .HasColumnName("hours_per_sessions");

                entity.Property(p => p.Optional)
                      .HasColumnName("optional")
                      .HasMaxLength(50);

                entity.Property(p => p.MaxParticipant)
                      .HasColumnName("max_participant");

                entity.Property(p => p.Location)
                      .HasColumnName("location")
                      .HasMaxLength(255);

                entity.Property(p => p.StartDate)
                      .HasColumnName("start_date");

                entity.Property(p => p.EndDate)
                      .HasColumnName("end_date");

                entity.Property(p => p.RegistrationStartDate)
                      .HasColumnName("registration_start_date");

                entity.Property(p => p.RegistrationClosingDate)
                      .HasColumnName("registration_closing_date");

                entity.Property(p => p.StatusId)
                      .HasColumnName("status_id");

                entity.Property(p => p.CategoryId)
                      .HasColumnName("category_id");

                entity.Property(p => p.LecturerId)
                      .HasColumnName("lecturer_id");

                entity.Property(p => p.CreatedAt)
                      .HasColumnName("created_at");

                entity.Property(p => p.ModifiedAt)
                      .HasColumnName("modified_at");

                entity.Property(p => p.IsDeleted)
                      .HasColumnName("is_deleted")
                      .HasDefaultValue(false);

                entity.Property(p => p.IsPrivate)
                      .HasColumnName("is_private")
                      .HasDefaultValue(false);

                entity.Property(p => p.CreateById)
                    .HasColumnName("created_by_id")
                    .HasMaxLength(450);

                entity.Property(p => p.UpdateById)
                   .HasColumnName("update_by_id")
                   .HasMaxLength(450);

                // === Relations ===

                entity.HasOne(p => p.Status)
                      .WithMany(s => s.Courses)
                      .HasForeignKey(p => p.StatusId)
                      .OnDelete(DeleteBehavior.SetNull);


                entity.HasOne(u => u.CreateBy)
                     .WithMany()
                     .HasForeignKey(u => u.CreateById)
                     .IsRequired(false)
                     .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(u => u.UpdateBy)
                    .WithMany()
                    .HasForeignKey(u => u.UpdateById)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(p => p.Category)
                      .WithMany(s => s.Courses)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(p => p.Lecturer)
                      .WithMany(s => s.Courses)
                      .HasForeignKey(p => p.LecturerId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(x => x.AttachedFiles)
                      .WithOne(x => x.Course)
                      .HasForeignKey(x => x.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.CourseDepartments)
                      .WithOne(cd => cd.Course)
                      .HasForeignKey(cd => cd.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.CoursePositions)
                      .WithOne(cp => cp.Course)
                      .HasForeignKey(cp => cp.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.UserCourses)
                      .WithOne(uc => uc.Course)
                      .HasForeignKey(uc => uc.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.Lessons)
                      .WithOne(l => l.Course)
                      .HasForeignKey(l => l.CourseId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(c => c.Tests)
                      .WithOne(t => t.Course)
                      .HasForeignKey(t => t.CourseId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }


        private void ConfigureCourseStatus(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CourseStatus>(entity =>
            {
                entity.ToTable("CourseStatus");

                entity.HasKey(s => s.Id);

                entity.Property(s => s.Id)
                      .HasColumnName("id")
                      .IsRequired().ValueGeneratedOnAdd();

                entity.Property(s => s.Name)
                      .HasColumnName("name")
                      .IsRequired()
                      .HasMaxLength(255);

                entity.HasMany(s => s.Courses)
                      .WithOne(c => c.Status)
                      .HasForeignKey(c => c.StatusId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }

        private void ConfigureCourseAttachedFile(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CourseAttachedFile>(entity =>
            {
                entity.ToTable("CourseAttachedFile");

                entity.HasKey(x => x.Id);
                entity.Property(s => s.Id)
                      .HasColumnName("id")
                      .IsRequired().ValueGeneratedOnAdd();

                entity.Property(x => x.CourseId)
                      .HasColumnName("course_id")
                      .IsRequired();

                entity.Property(x => x.Title)
                      .HasColumnName("title")
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(e => e.TypeDocId)
                      .HasColumnName("type_doc_id");

                entity.Property(x => x.Link)
                      .HasColumnName("link")
                      .HasMaxLength(450);

                entity.Property(x => x.PublicIdUrlPdf)
                      .HasColumnName("public_id_url_pdf")
                      .HasMaxLength(450);



                entity.Property(x => x.UserId)
                      .HasColumnName("created_by_id")
                      .IsRequired();

                entity.Property(x => x.CreatedAt)
                      .HasColumnName("created_at");

                entity.Property(x => x.ModifiedTime)
                      .HasColumnName("modified_at");

                entity.HasOne(f => f.Course)
                      .WithMany(c => c.AttachedFiles)
                      .HasForeignKey(f => f.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(f => f.UserCreated)
                      .WithMany()
                      .HasForeignKey(f => f.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.TypeDoc)
                      .WithMany()
                      .HasForeignKey(e => e.TypeDocId)
                      .HasConstraintName("fk_AttachedFile_type_document")
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }


        private void ConfigureCourseDepartment(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CourseDepartment>(entity =>
            {
                entity.ToTable("CourseDepartment");

                entity.HasKey(e => e.Id);

                entity.Property(s => s.Id)
                      .IsRequired().HasColumnName("id").ValueGeneratedOnAdd();

                entity.Property(cd => cd.CourseId)
                      .IsRequired().HasColumnName("course_id");

                entity.Property(cd => cd.DepartmentId)
                      .IsRequired().HasColumnName("department_id");

                entity.HasOne(cd => cd.Course)
                      .WithMany(c => c.CourseDepartments)
                      .HasForeignKey(cd => cd.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cd => cd.Department)
                      .WithMany(d => d.CourseDepartments)
                      .HasForeignKey(cd => cd.DepartmentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigureCoursePosition(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CoursePosition>(entity =>
            {
                entity.ToTable("CoursePosition");

                entity.HasKey(e => e.Id);

                entity.Property(s => s.Id)
                      .IsRequired().HasColumnName("id").ValueGeneratedOnAdd();

                entity.Property(cd => cd.CourseId)
                      .IsRequired().HasColumnName("course_id");

                entity.Property(cd => cd.PositionId)
                      .IsRequired().HasColumnName("position_id");

                entity.HasOne(cp => cp.Course)
                      .WithMany(c => c.CoursePositions)
                      .HasForeignKey(cp => cp.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cp => cp.Position)
                      .WithMany(p => p.CoursePositions)
                      .HasForeignKey(cp => cp.PositionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigureUserCourse(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserCourse>(entity =>
            {
                entity.ToTable("UserCourse"); // tên bảng

                entity.HasKey(e => e.Id);

                entity.Property(s => s.Id)
                      .HasColumnName("id")
                      .IsRequired()
                      .ValueGeneratedOnAdd();

                entity.Property(s => s.AssignedAt)
                      .HasColumnName("assigned_at");

                entity.Property(s => s.IsMandatory)
                      .HasColumnName("is_mandatory");

                entity.Property(s => s.Status)
                      .HasColumnName("status");

                entity.Property(s => s.CreatedAt)
                      .HasColumnName("created_at");

                entity.Property(s => s.ModifiedAt)
                      .HasColumnName("modified_at");

                entity.Property(s => s.CourseId)
                      .HasColumnName("course_id");

                entity.Property(s => s.UserId)
                      .HasColumnName("user_id");

                entity.HasOne(cp => cp.Course)
                      .WithMany(c => c.UserCourses)
                      .HasForeignKey(cp => cp.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cp => cp.User)
                      .WithMany(p => p.UserCourse)
                      .HasForeignKey(cp => cp.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }


        private void ConfigureCourseCategory(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CourseCategory>(entity =>
            {
                // Định nghĩa khóa chính
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).HasColumnName("id").ValueGeneratedOnAdd();

                entity.Property(p => p.Name)
                      .HasColumnName("name")
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(p => p.Description)
                      .HasColumnName("description")
                      .HasMaxLength(1000);

                entity.HasMany(s => s.Courses)
                      .WithOne(c => c.Category)
                      .HasForeignKey(c => c.CategoryId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }

        private void ConfigureLecturer(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lecturer>(entity =>
            {
                // Định nghĩa khóa chính
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).HasColumnName("id").ValueGeneratedOnAdd();

                entity.Property(p => p.FullName)
                      .HasColumnName("full_name")
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(p => p.Email)
                       .HasColumnName("email")
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(p => p.ProfileImageUrl)
                      .HasColumnName("profile_image_url")
                      .HasMaxLength(255);

                entity.Property(p => p.PhoneNumber)
                      .HasColumnName("phone_number")
                      .IsRequired()
                      .HasMaxLength(255);

                entity.HasMany(s => s.Courses)
                      .WithOne(c => c.Lecturer)
                      .HasForeignKey(c => c.LecturerId)
                      .OnDelete(DeleteBehavior.SetNull);

            });
        }

        private void ConfigureTypeDocument(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TypeDocument>(entity =>
            {
                entity.ToTable("TypeDocument");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                      .IsRequired()
                      .ValueGeneratedOnAdd()
                      .HasColumnName("id");

                entity.Property(e => e.NameType)
                      .IsRequired()
                      .HasMaxLength(500)
                      .HasColumnName("name_type");

                entity.Property(e => e.CreatedAt)
                      .HasColumnName("created_at")
                      .HasDefaultValueSql("GETDATE()");
            });
        }

        private void ConfigureLesson(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.ToTable("Lessons");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                      .IsRequired()
                      .ValueGeneratedOnAdd()
                      .HasColumnName("id");

                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasMaxLength(255)
                      .HasColumnName("title");

                entity.Property(e => e.Position)
                      .IsRequired()
                      .HasColumnName("position");


                entity.Property(e => e.FileUrl)
                      .IsRequired()
                      .HasMaxLength(500)
                      .HasColumnName("file_url");

                entity.Property(e => e.PublicIdUrlPdf)
                      .HasMaxLength(255)
                      .HasColumnName("public_id_url_pdf")
                      .IsRequired(false); ;

                entity.Property(e => e.CourseId)
                      .HasColumnName("course_id");

                entity.Property(e => e.TypeDocId)
                      .HasColumnName("type_doc_id");

                entity.Property(e => e.TotalDurationSeconds)
                      .HasColumnName("total_duration_seconds");

                entity.Property(e => e.TotalPages)
                      .HasColumnName("total_pages");

                entity.Property(e => e.CreatedById)
                      .HasColumnName("created_by_id");

                entity.Property(e => e.UpdatedById)
                      .HasColumnName("updated_by_id");

                entity.Property(e => e.CreatedAt)
                      .HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt)
                      .HasColumnName("updated_at");

                entity.HasOne(e => e.Course)
                      .WithMany(c => c.Lessons)
                      .HasForeignKey(e => e.CourseId)
                      .HasConstraintName("fk_lessons_courses")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.CreatedBy)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedById)
                      .HasConstraintName("fk_lessons_user_created")
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.UpdatedBy)
                      .WithMany()
                      .HasForeignKey(e => e.UpdatedById)
                      .HasConstraintName("fk_lessons_user_edited")
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.TypeDoc)
                      .WithMany()
                      .HasForeignKey(e => e.TypeDocId)
                      .HasConstraintName("fk_lessons_type_document")
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureTest(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Test>(entity =>
            {
                entity.ToTable("Tests"); // table name snake_case

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                      .IsRequired()
                      .ValueGeneratedOnAdd()
                      .HasColumnName("id");

                entity.Property(e => e.Position)
                      .IsRequired()
                      .HasColumnName("position");

                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasMaxLength(255)
                      .HasColumnName("title");

                entity.Property(e => e.PassThreshold)
                      .HasColumnName("pass_threshold");

                entity.Property(e => e.TimeTest)
                      .HasColumnName("time_test");

                entity.Property(e => e.CreatedAt)
                      .HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt)
                      .HasColumnName("updated_at");

                entity.Property(e => e.CourseId)
                      .HasColumnName("course_id");

                entity.Property(e => e.CreatedById)
                      .HasColumnName("created_by_id");

                entity.Property(e => e.UpdatedById)
                      .HasColumnName("updated_by_id");

                entity.HasOne(e => e.Course)
                      .WithMany(c => c.Tests)
                      .HasForeignKey(e => e.CourseId)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName("fk_tests_courses");

                entity.HasOne(e => e.CreatedBy)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedById)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName("fk_tests_user_created");

                entity.HasOne(e => e.UpdatedBy)
                      .WithMany()
                      .HasForeignKey(e => e.UpdatedById)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName("fk_tests_user_edited");
            });
        }

        private void ConfigureQuestion(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Question>(entity =>
            {
                entity.ToTable("Questions"); // snake_case

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                      .ValueGeneratedOnAdd()
                      .HasColumnName("id");

                entity.Property(e => e.TestId)
                      .IsRequired()
                      .HasColumnName("test_id");

                entity.Property(e => e.Position)
                      .IsRequired()
                      .HasColumnName("position");

                entity.Property(e => e.QuestionText)
                      .HasMaxLength(255)
                      .HasColumnName("question_text");

                entity.Property(e => e.CorrectOption)
                      .HasMaxLength(255)
                      .HasColumnName("correct_option");

                entity.Property(e => e.QuestionType)
                      .HasColumnName("question_type");

                entity.Property(e => e.Explanation)
                      .HasMaxLength(255)
                      .HasColumnName("explanation");

                entity.Property(e => e.A)
                      .HasMaxLength(255)
                      .HasColumnName("a");

                entity.Property(e => e.B)
                      .HasMaxLength(255)
                      .HasColumnName("b");

                entity.Property(e => e.C)
                      .HasMaxLength(255)
                      .HasColumnName("c");

                entity.Property(e => e.D)
                      .HasMaxLength(255)
                      .HasColumnName("d");

                entity.Property(e => e.CreatedAt)
                      .HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt)
                      .HasColumnName("updated_at");

                entity.Property(p => p.CreateById)
                    .HasColumnName("created_by_id")
                    .HasMaxLength(450);

                entity.Property(p => p.UpdateById)
                   .HasColumnName("update_by_id")
                   .HasMaxLength(450);


                entity.HasOne(u => u.CreateBy)
                     .WithMany()
                     .HasForeignKey(u => u.CreateById)
                     .IsRequired(false)
                     .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(u => u.UpdateBy)
                    .WithMany()
                    .HasForeignKey(u => u.UpdateById)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Test)
                      .WithMany(t => t.Questions) // sửa lại navigation property nếu đang sai
                      .HasForeignKey(e => e.TestId)
                      .HasConstraintName("fk_questions_tests")
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigureFeedback(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("Feedbacks"); // table name snake_case

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                      .IsRequired()
                      .ValueGeneratedOnAdd()
                      .HasColumnName("id");

                entity.Property(e => e.CourseId)
                      .HasColumnName("course_id");

                entity.Property(e => e.UserId)
                      .HasColumnName("user_id");

                entity.Property(e => e.Q1_relevance)
                      .HasColumnName("q1_relevance");

                entity.Property(e => e.Q2_clarity)
                      .HasColumnName("q2_clarity");

                entity.Property(e => e.Q3_structure)
                      .HasColumnName("q3_structure");

                entity.Property(e => e.Q4_duration)
                      .HasColumnName("q4_duration");

                entity.Property(e => e.Q5_material)
                      .HasColumnName("q5_material");
                entity.Property(e => e.Comment)
                      .HasColumnName("comment");
                entity.Property(e => e.SubmissionDate)
                      .HasColumnName("feedback_at");
            });
        }

        private void ConfigureLessonProgress(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LessonProgress>(entity =>
            {
                entity.ToTable("LessonProgress", t =>
                      {
                          t.HasCheckConstraint("CK_Progress_Type",
                                "(current_time_seconds IS NOT NULL AND current_page IS NULL) OR " +
                                "(current_time_seconds IS NULL AND current_page IS NOT NULL) OR " +
                                "(current_time_seconds IS NULL AND current_page IS NULL)");
                      });

                entity.HasKey(e => new { e.UserId, e.LessonId });

                entity.Property(e => e.UserId)
                      .IsRequired()
                      .HasColumnName("user_id");

                entity.Property(e => e.LessonId)
                      .IsRequired()
                      .HasColumnName("lesson_id");

                entity.Property(e => e.CurrentTimeSeconds)
                      .HasColumnName("current_time_seconds");

                entity.Property(e => e.CurrentPage)
                      .HasColumnName("current_page");

                entity.Property(e => e.IsCompleted)
                      .IsRequired()
                      .HasColumnName("is_completed")
                      .HasDefaultValue(false);

                entity.Property(e => e.LastUpdated)
                      .IsRequired()
                      .HasColumnName("last_accessed")
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.User)
                      .WithMany(u => u.LessonProgress)
                      .HasForeignKey(e => e.UserId)
                      .HasConstraintName("fk_student_lesson_progress_user")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Lesson)
                      .WithMany(l => l.LessonProgress)
                      .HasForeignKey(e => e.LessonId)
                      .HasConstraintName("fk_student_lesson_progress_lesson")
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigureAuditLog(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("audit_logs");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                      .IsRequired()
                      .ValueGeneratedOnAdd()
                      .HasColumnName("id");

                entity.Property(e => e.EntityName)
                      .HasColumnName("entity_name");

                entity.Property(e => e.EntityId)
                      .HasColumnName("entity_id");

                entity.Property(e => e.Action)
                      .HasColumnName("action");

                entity.Property(e => e.Changes)
                      .HasColumnName("changes");

                entity.Property(e => e.Timestamp)
                      .HasColumnName("time_stamp");

                entity.Property(e => e.UserId)
                      .HasColumnName("user_id");

                entity.HasOne(u => u.User)
                     .WithMany()
                     .HasForeignKey(u => u.UserId)
                     .IsRequired(false)
                     .OnDelete(DeleteBehavior.NoAction);
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditEntries = OnBeforeSaveChanges();
            var result = await base.SaveChangesAsync(cancellationToken);
            if(auditEntries.Any())
            {
                await OnAfterSaveChanges(auditEntries);
            }
            return result;
        }

        private List<AuditEntry> OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var (currentUserId, _) = _userService.GetCurrentUserAuthenticationInfo();
            Console.WriteLine($"Current User ID: {userId}");

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                if(userId != null)
                {
                    var audit = new AuditEntry(entry)
                    {
                        TableName = entry.Metadata.GetTableName() ?? "Unknown",
                        Action = entry.State.ToString(),
                        UserId = userId
                    };

                    foreach (var prop in entry.Properties)
                    {
                        var propName = prop.Metadata.Name;

                        if (prop.Metadata.IsPrimaryKey() && prop.IsTemporary)
                        {
                            audit.TemporaryProperties.Add(prop);  // lưu lại prop cần lấy sau
                        }

                        if (entry.State == EntityState.Added)
                        {
                            audit.NewValues[propName] = prop.CurrentValue ?? "Unknown";
                        }
                        else if (entry.State == EntityState.Deleted)
                        {
                            audit.OldValues[propName] = prop.OriginalValue ?? "Unknown";
                        }
                        else if (entry.State == EntityState.Modified && prop.IsModified)
                        {
                            audit.OldValues[propName] = prop.OriginalValue ?? "Unknown";
                            audit.NewValues[propName] = prop.CurrentValue ?? "Unknown";
                        }
                        Console.WriteLine($"{prop.Metadata.Name}: IsModified = {prop.IsModified}");
                    }

                    auditEntries.Add(audit);
                }
                
            }

            return auditEntries;
        }

        private async Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            foreach (var audit in auditEntries)
            {
                foreach (var prop in audit.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        audit.NewValues[prop.Metadata.Name] = prop.CurrentValue!;
                    }
                }
                AuditLogs.Add(new AuditLog
                {
                    EntityName = audit.TableName,
                    Action = audit.Action,
                    EntityId = audit.GetPrimaryKeyAsString(),
                    UserId = audit.UserId,
                    Changes = audit.ToJsonChanges(),
                    Timestamp = DateTime.UtcNow
                });
            }

            await SaveChangesWithoutAuditAsync();
        }

        private async Task SaveChangesWithoutAuditAsync(CancellationToken cancellationToken = default)
        {
            await base.SaveChangesAsync(cancellationToken);
        }
    }
}
