using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Infrastructure.Persistence // Ví dụ: bạn có thể đặt nó trong thư mục Data
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
            public DbSet<Feedback> Feedbacks { get; set; } // Ví dụ về DbSet cho Feedback


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
                  ConfigureFeedback(modelBuilder);
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
                              .HasMaxLength(100); // Giới hạn độ dài ID Card (ví dụ: CCCD)

                        entity.Property(u => u.StatusId)
                              .HasMaxLength(100); // Giới hạn độ dài trạng thái

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

                        entity.HasOne(u => u.ManagerU)        // Một User có MỘT quản lý trực tiếp
                              .WithMany(p => p.Children)         // Một quản lý qly NHIỀU User
                              .HasForeignKey(u => u.ManagerUId)   // Khóa ngoại là ManagerUId
                              .IsRequired(false)              // ManagerUId có thể là NULL (tức là không bắt buộc User phải có qly)
                              .OnDelete(DeleteBehavior.NoAction);

                        // User status
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
                        entity.Property(d => d.DepartmentId).ValueGeneratedOnAdd();

                        // Cấu hình thuộc tính DepartmentName
                        entity.Property(d => d.DepartmentName)
                              .IsRequired()               // Bắt buộc phải có giá trị (không NULL)
                              .HasMaxLength(255);         // Giới hạn độ dài tối đa 255 ký tự

                        entity.Property(d => d.DepartmentCode)
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

                        entity.Property(d => d.ManagerId).HasColumnName("ManagerId").IsRequired(false);

                        entity.Property(d => d.Level);

                        // Cấu hình thuộc tính Description
                        entity.Property(d => d.Description)
                              .HasMaxLength(1000); // Giới hạn độ dài cho Description

                        entity.Property(d => d.Status);
                        entity.Property(d => d.CreatedAt);
                        entity.Property(d => d.UpdatedAt);
                  });
            }

            private void ConfigurePosition(ModelBuilder modelBuilder)
            {
                  modelBuilder.Entity<Position>(entity =>
                  {
                        // Định nghĩa khóa chính
                        entity.HasKey(p => p.PositionId);
                        entity.Property(p => p.PositionId).ValueGeneratedOnAdd();

                        entity.Property(p => p.PositionName)
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
                        entity.Property(p => p.Id).ValueGeneratedOnAdd();

                        entity.Property(p => p.Name)
                              .IsRequired()               // Bắt buộc phải có giá trị
                              .HasMaxLength(255);         // Giới hạn độ 
                  });
            }

            private void ConfigureCourse(ModelBuilder modelBuilder)
            {
                  modelBuilder.Entity<Course>(entity =>
                  {
                        entity.ToTable("Course");

                        entity.HasKey(p => p.Id);

                        entity.Property(p => p.Id)
                              .IsRequired();

                        entity.Property(p => p.Code)
                              .HasMaxLength(100);

                        entity.Property(p => p.Name)
                              .IsRequired()
                              .HasMaxLength(255);

                        entity.Property(p => p.Description)
                              .HasMaxLength(1000);

                        entity.Property(p => p.ThumbUrl)
                              .HasMaxLength(200);

                        entity.Property(p => p.Objectives)
                              .HasMaxLength(255);

                        entity.Property(p => p.Format)
                              .HasMaxLength(255);

                        entity.Property(p => p.Sessions)
                              .HasMaxLength(100);
                        entity.Property(p => p.HoursPerSessions)
                              .HasMaxLength(100);
                        entity.Property(p => p.Optional)
                              .HasMaxLength(100);
                        entity.Property(p => p.MaxParticipant)
                        .HasMaxLength(100);

                        entity.Property(p => p.Location)
                              .HasMaxLength(255);

                        entity.Property(p => p.StartDate)
                              .HasColumnType("datetime");

                        entity.Property(p => p.EndDate)
                              .HasColumnType("datetime");

                        entity.Property(p => p.RegistrationStartDate)
                              .HasColumnType("datetime");

                        entity.Property(p => p.RegistrationClosingDate)
                              .HasColumnType("datetime");


                        entity.Property(p => p.StatusId);

                        entity.Property(p => p.CategoryId);

                        entity.Property(p => p.LecturerId);

                        entity.Property(p => p.CreatedAt)
                              .HasColumnType("datetime");

                        entity.Property(p => p.ModifiedAt)
                              .HasColumnType("datetime");

                        entity.Property(p => p.IsDeleted)
                              .HasDefaultValue(false);

                        entity.HasOne(p => p.Status)
                              .WithMany(s => s.Courses)
                              .HasForeignKey(p => p.StatusId)
                              .OnDelete(DeleteBehavior.SetNull);

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
                              .HasForeignKey(x => x.CourseId);
                  });
            }

            private void ConfigureCourseStatus(ModelBuilder modelBuilder)
            {
                  modelBuilder.Entity<CourseStatus>(entity =>
                  {
                        entity.ToTable("CourseStatus");

                        entity.HasKey(s => s.Id);

                        entity.Property(s => s.Id)
                              .IsRequired().ValueGeneratedOnAdd();

                        entity.Property(s => s.Name)
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

                        entity.Property(x => x.CourseId)
                              .IsRequired();

                        entity.Property(x => x.Title)
                              .HasMaxLength(200)
                              .IsRequired();

                        entity.Property(x => x.Type)
                              .IsRequired()
                              .HasMaxLength(100);

                        entity.Property(x => x.Link)
                              .HasMaxLength(200);

                        entity.Property(x => x.PublicIdUrlPdf)
                              .HasMaxLength(200);



                        entity.Property(x => x.UserId)
                              .IsRequired();

                        entity.Property(x => x.CreatedAt);

                        entity.Property(x => x.ModifiedTime);

                        entity.HasOne(f => f.Course)
                              .WithMany(c => c.AttachedFiles)
                              .HasForeignKey(f => f.CourseId)
                              .OnDelete(DeleteBehavior.Cascade);

                        entity.HasOne(f => f.UserCreated)
                              .WithMany()
                              .HasForeignKey(f => f.UserId)
                              .OnDelete(DeleteBehavior.Cascade);
                  });
            }


            private void ConfigureCourseDepartment(ModelBuilder modelBuilder)
            {
                  modelBuilder.Entity<CourseDepartment>(entity =>
                  {
                        entity.ToTable("CourseDepartment");

                        entity.HasKey(e => e.Id);

                        entity.Property(s => s.Id)
                              .IsRequired().ValueGeneratedOnAdd();

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
                              .IsRequired().ValueGeneratedOnAdd();

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
                        entity.ToTable("UserCourse");

                        entity.HasKey(e => e.Id);

                        entity.Property(s => s.Id)
                              .IsRequired().ValueGeneratedOnAdd();

                        entity.Property(s => s.AssignedAt);
                        entity.Property(s => s.IsMandatory);
                        entity.Property(s => s.Status);



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
                        entity.Property(p => p.Id).ValueGeneratedOnAdd();

                        entity.Property(p => p.Name)
                              .IsRequired()
                              .HasMaxLength(255);

                        entity.Property(p => p.Description)
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
                        entity.Property(p => p.Id).ValueGeneratedOnAdd();

                        entity.Property(p => p.FullName)
                              .IsRequired()
                              .HasMaxLength(255);

                        entity.Property(p => p.Email)
                              .IsRequired()
                              .HasMaxLength(255);

                        entity.Property(p => p.ProfileImageUrl)
                              .HasMaxLength(255);

                        entity.Property(p => p.PhoneNumber)
                              .IsRequired()
                              .HasMaxLength(255);

                        entity.HasMany(s => s.Courses)
                              .WithOne(c => c.Lecturer)
                              .HasForeignKey(c => c.LecturerId)
                              .OnDelete(DeleteBehavior.SetNull);

                  });
            }

            private void ConfigureLesson(ModelBuilder modelBuilder)
            {
                  modelBuilder.Entity<Lesson>(entity =>
                  {
                        entity.ToTable("Lessons"); // ✅ table snake_case

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

                        entity.Property(e => e.UrlPdf)
                              .IsRequired()
                              .HasMaxLength(255)
                              .HasColumnName("url_pdf");

                        entity.Property(e => e.PublicIdUrlPdf)
                              .IsRequired()
                              .HasMaxLength(255)
                              .HasColumnName("public_id_url_pdf");

                        entity.Property(e => e.CourseId)
                              .HasColumnName("course_id");

                        entity.Property(e => e.UserIdCreated)
                              .HasColumnName("user_id_created");

                        entity.Property(e => e.UserIdEdited)
                              .HasColumnName("user_id_edited");

                        entity.Property(e => e.CreatedAt)
                              .HasColumnName("created_at");

                        entity.Property(e => e.UpdatedAt)
                              .HasColumnName("updated_at");

                        entity.HasOne(e => e.Course)
                              .WithMany(c => c.Lessons)
                              .HasForeignKey(e => e.CourseId)
                              .HasConstraintName("fk_lessons_courses") // ✅ snake_case constraint
                              .OnDelete(DeleteBehavior.NoAction);

                        entity.HasOne(e => e.UserCreated)
                              .WithMany(u => u.CreatedLesson)
                              .HasForeignKey(e => e.UserIdCreated)
                              .HasConstraintName("fk_lessons_user_created") // ✅ snake_case constraint
                              .OnDelete(DeleteBehavior.NoAction);

                        entity.HasOne(e => e.UserEdited)
                              .WithMany(u => u.UpdatedLesson)
                              .HasForeignKey(e => e.UserIdEdited)
                              .HasConstraintName("fk_lessons_user_edited") // ✅ snake_case constraint
                              .OnDelete(DeleteBehavior.NoAction);
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

                        entity.Property(e => e.UserIdCreated)
                              .HasColumnName("user_id_created");

                        entity.Property(e => e.UserIdEdited)
                              .HasColumnName("user_id_edited");

                        entity.HasOne(e => e.Course)
                              .WithMany(c => c.Tests)
                              .HasForeignKey(e => e.CourseId)
                              .OnDelete(DeleteBehavior.NoAction)
                              .HasConstraintName("fk_tests_courses");

                        entity.HasOne(e => e.UserCreated)
                              .WithMany(u => u.CreatedTest)
                              .HasForeignKey(e => e.UserIdCreated)
                              .OnDelete(DeleteBehavior.NoAction)
                              .HasConstraintName("fk_tests_user_created");

                        entity.HasOne(e => e.UserEdited)
                              .WithMany(u => u.UpdatedTest)
                              .HasForeignKey(e => e.UserIdEdited)
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

                        entity.Property(e => e.q1_revelance)
                              .HasColumnName("q1_revelance");

                        entity.Property(e => e.q2_clarity)
                              .HasColumnName("q2_clarity");

                        entity.Property(e => e.q3_structure)
                              .HasColumnName("q3_structure");

                        entity.Property(e => e.q4_duration)
                              .HasColumnName("q4_duration");

                        entity.Property(e => e.q5_material)
                              .HasColumnName("q5_material");
                        entity.Property(e => e.Comment)
                              .HasColumnName("comment");
                        entity.Property(e => e.SubmissionDate)
                              .HasColumnType("datetime")
                              .HasColumnName("feedback_at");
                  });
            }
      }
}