
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QLDT_Becamex.Src.Application.Common.Mappings;
using QLDT_Becamex.Src.Application.Features.Users.Commands;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Infrastructure.Persistence;
using QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories;
using QLDT_Becamex.Src.Infrastructure.Services;


using System.Text;
using QLDT_Becamex.Src.Infrastructure.Services.CloudinaryServices;
using QLDT_Becamex.Src.Infrastructure.Services.UserServices;
using QLDT_Becamex.Src.Infrastructure.Services.JwtServices;
using QLDT_Becamex.Src.Infrastructure.Services.DepartmentServices;
using QLDT_Becamex.Src.Infrastructure.Services.CourseServices;
using QLDT_Becamex.Src.Infrastructure.Services.BackgroundServices;
using QLDT_Becamex.Src.Application.Common.Mappings.AuditLogs;


var builder = WebApplication.CreateBuilder(args);

// --- Cấu hình Services ---
// Các dịch vụ được thêm vào container Dependency Injection.

// 1. Cấu hình Database Context và Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Cấu hình các policy về mật khẩu
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Cấu hình lockout (khóa tài khoản)
    options.Lockout.AllowedForNewUsers = false;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(0);
    options.Lockout.MaxFailedAccessAttempts = int.MaxValue;

    // Cấu hình User
    options.User.RequireUniqueEmail = true;

    // Cấu hình Signin
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 2. Cấu hình Authentication (JWT) và Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Để trả về 401 Unauthorized thay vì redirect
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = builder.Configuration["Jwt:Issuer"],
         ValidAudience = builder.Configuration["Jwt:Audience"],
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
     };
 });

builder.Services.AddAuthorization(); // Đăng ký dịch vụ ủy quyền

// 3. Đăng ký HttpContextAccessor (Cần thiết cho UserService lấy thông tin user hiện tại)
builder.Services.AddHttpContextAccessor(); // <-- Đã thêm

// Add Cors dành cho môi trường dev khác domain
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Chỉ cho phép frontend này
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Nếu có sử dụng cookie, session
    });
});

// Add Cors dành cho môi trường dev khác domain
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Chỉ cho phép frontend này
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Nếu có sử dụng cookie, session
    });
});

// 5. Cấu hình AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// 6. Cấu hình MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly);
});

// 4. Đăng ký Unit of Work, Repositories và Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IEmployeeLevelRepository, EmployeeLevelRepository>();
builder.Services.AddScoped<ICourseCategoryRepository, CourseCategoryRepository>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<ITestRepository, TestRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<ILessonProgressRepository, LessonProgressRepository>();
builder.Services.AddScoped<IDepartmentStatusRepository, DepartmentStatusRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IReportsRepository, ReportsRepository>();
// Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IAuditLogMapper, AuditLogMapper>();
builder.Services.AddHostedService<CourseStatusUpdateBackgroundService>();

builder.Services.AddHttpContextAccessor();



// 6. Cấu hình Controllers và Swagger/OpenAPI
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Khám phá các endpoint cho Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "QLDT Becamex API", Version = "v1" });
    // Tùy chọn: Thêm hỗ trợ JWT cho Swagger UI để có thể thử API được bảo vệ
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập 'Bearer ' và token JWT của bạn vào đây (ví dụ: 'Bearer YOUR_TOKEN')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();


// --- Cấu hình HTTP Request Pipeline (Middleware) ---
// Thứ tự của các middleware rất quan trọng.

// 1. Cấu hình cho môi trường phát triển (Swagger)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowSpecificOrigin"); // 🔥 Bật CORS ở middleware
}

// 2. Middleware chuyển hướng HTTPS (Tùy chọn, hiện đang bị comment)
// app.UseHttpsRedirection();
// --- 🔥 2. Exception Handling Middleware (custom) ---
app.UseMiddleware<ExceptionHandlingMiddleware>(); // 👈 THÊM Ở ĐÂY
// 3. Middleware định tuyến
app.UseRouting(); // Cần thiết nếu bạn muốn các middleware Authorization/Authentication hoạt động trước khi chọn endpoint

app.UseCors(option =>
{
    option.AllowAnyHeader();
    option.AllowAnyMethod();
    option.AllowAnyOrigin();
});
// 4. Middleware Authentication và Authorization
app.UseAuthentication(); // Xác thực người dùng (đọc token, cookie, v.v.)
app.UseAuthorization();  // Ủy quyền (kiểm tra quyền truy cập dựa trên [Authorize] attributes)

// 5. Định tuyến các Controller (Map endpoints)
app.MapControllers();

// 6. Chạy ứng dụng
app.Run();
